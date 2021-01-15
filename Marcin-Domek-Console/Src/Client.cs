using Marcin_Domek_Console.Src;
using Marcin_Domek_Server.Src.Extension;
using Marcin_Domek_Server.Src.Requests;
using Marcin_Domek_Server.Src.Users;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Xml.Linq;

namespace Marcin_Domek.Src
{
    class Client
    {
        private IPHostEntry IPHost { get; }
        private IPAddress IPAddress { get; }
        private IPEndPoint LocalEndPoint { get; }
        private Socket SenderSocket { get; set; }

        public UserType UserType { get; private set; }
        public string UserFirstName { get; private set; }
        public string UserLastName { get; private set; }
        private Guid SessionID { get; set; }
        public bool ValidSession => SessionID != Guid.Empty;

        public Client()
        {
            UserType = UserType.None;
            SessionID = Guid.Empty;

            IPHost = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress = IPHost.AddressList[0];
            LocalEndPoint = new IPEndPoint(IPAddress, 12121);
        }

        private void Open()
        {
            try
            {
                SenderSocket = new Socket(IPAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                SenderSocket.Connect(LocalEndPoint);
            }
            catch (ArgumentNullException ane)
            {

                Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
            }
            catch (Exception)
            {
                //Ignore, let send message handle it
            }
        }

        private void Close()
        {
            try
            {
                SenderSocket.Shutdown(SocketShutdown.Both);
                SenderSocket.Close();
            }
            catch (Exception)
            {
                //Ignore, nothing bad will happen
            }
        }

        public void Clear()
        {
            UserType = UserType.None;
            UserFirstName = null;
            UserLastName = null;
            SessionID = Guid.Empty;
        }

        private string SendMessage(string message)
        {
            Open();
            if (SenderSocket.Connected)
            {
                Trace.WriteLine("Socket connected to: " + SenderSocket.RemoteEndPoint.ToString());

                byte[] messageSent = Encoding.UTF8.GetBytes(message);
                int byteSent = SenderSocket.Send(messageSent);

                byte[] messageReceived = new byte[1024];

                int byteRecv = SenderSocket.Receive(messageReceived);
                string response = Encoding.UTF8.GetString(messageReceived, 0, byteRecv);

                if (response != null && response.Length > 5) response = response[0..^5];

                Close();
                return response;
            }
            else
            {
                Close();
                return "<response><internalreset>1</internalreset><reason>Session timed out</reason></response>";
            }
        }
        public bool TryConnection()
        {
            try
            {
                return ResponseContainsReset(SendMessage("<request></request><EOF>"), false);
            }
            catch (Exception)
            {
                return false;
            }

        }

        public bool TryLogin()
        {
            Console.WriteLine("Please enter your account information:");

            string login = Utility.GetStringFromConsole("Enter your login:", "Entered login was empty.");
            string password = Utility.GetStringFromConsole("Enter your password:", "Entered password was empty.");

            string request = "<request><sessionid>0</sessionid>" + "<login>" + login + "</login><password>" + password + "</password>" + "</request><EOF>";
            string response = SendMessage(request);

            if (!ResponseContainsReset(response))
            {
                ParseLoginResponse(response);
                return UserType != UserType.None;
            }
            else return false;
        }

        private void ParseLoginResponse(string response)
        {

            XDocument responseXml;

            try
            {
                responseXml = XDocument.Parse(response);
            }
            catch (Exception)
            {
                return;
            }

            SessionID = responseXml?.Root?.Element("sessionid") != null ? Guid.Parse(responseXml.Root.Element("sessionid").Value) : Guid.Empty;

            if (SessionID != Guid.Empty)
            {
                UserFirstName = responseXml?.Root?.Element("User")?.Element("FirstName")?.Value;
                UserLastName = responseXml?.Root?.Element("User")?.Element("FirstName")?.Value;
                UserType = responseXml?.Root?.Element("User")?.Element("UserType") != null ? Enum.Parse<UserType>(responseXml.Root.Element("User").Element("UserType").Value) : UserType.None;
            }
        }

        public void Logout()
        {
            if (TryConnection())
            {
                string request = "<request><sessionid>" + SessionID.ToString() + "</sessionid>" + "<type>" + RequestType.Logout.ToString() + "</type>" + "</request><EOF>";
                string response = SendMessage(request);
                if (ResponseContainsReset(response))
                {
                    XDocument responseXml;

                    try
                    {
                        responseXml = XDocument.Parse(response);
                    }
                    catch (Exception)
                    {
                        return;
                    }

                    if (responseXml?.Root?.Element("reason") != null)
                    {
                        Utility.WriteMessage(responseXml.Root.Element("reason").Value);
                    }

                    Clear();
                }
            }
            else
            {
                Clear();
            }
        }
        public Tuple<MessageType, string> GetMessageOrError(string response)
        {
            XDocument responseXml;

            try
            {
                responseXml = XDocument.Parse(response);
            }
            catch (Exception)
            {
                return new Tuple<MessageType, string>(MessageType.Error, "Unknown error");
            }

            if (responseXml?.Root != null)
            {
                if (responseXml.Root.Element("Message") != null)
                {
                    return new Tuple<MessageType, string>(MessageType.Message, responseXml.Root.Element("Message").Value);
                }
                else if (responseXml.Root.Element("Error") != null)
                {
                    return new Tuple<MessageType, string>(MessageType.Error, responseXml.Root.Element("Error").Value);
                }
                else return new Tuple<MessageType, string>(MessageType.None, null);
            }
            else return new Tuple<MessageType, string>(MessageType.Error, "Unknown eror");
        }

        private bool ResponseContainsReset(string response, bool internalreset = true)
        {
            XDocument responseXml;

            try
            {
                responseXml = XDocument.Parse(response);
            }
            catch (Exception)
            {
                return false;
            }

            if (responseXml?.Root != null)
            {
                if (responseXml.Root.Element("reset") != null) return true;
                if (internalreset && responseXml.Root.Element("internalreset") != null) return true;
                return false;
            }
            else return false;
        }
        internal Tuple<MessageType, string, HashSet<Ticket>> ListTickets(TicketListType ticketListType)
        {
            RequestType requestType = ticketListType switch
            {
                TicketListType.my => RequestType.ListMyTickets,
                TicketListType.unclaimed => RequestType.ListUnclaimedTickets,
                TicketListType.all => RequestType.ListTickets,
                _ => throw new NotImplementedException(),
            };

            string request = "<request><sessionid>" + SessionID.ToString() + "</sessionid>" + "<type>" + requestType.ToString() + "</type></request><EOF>";
            string response = SendMessage(request);
            

            if (!ResponseContainsReset(response))
            {
                Tuple<MessageType, string> message = GetMessageOrError(response);
                if (message.Item1 == MessageType.None)
                {
                    XDocument responseXml;

                    try
                    {
                        responseXml = XDocument.Parse(response);
                    }
                    catch (Exception)
                    {
                        return new Tuple<MessageType, string, HashSet<Ticket>>(MessageType.Error, "Unknown eror", null);
                    }

                    if (responseXml?.Root?.Element("Tickets") != null)
                    {
                        return new Tuple<MessageType, string, HashSet<Ticket>>(MessageType.None, "", responseXml.Root.Element("Tickets").Descendants("Ticket").Select(ticketElement =>
                        {
                            Ticket newTicket = new Ticket();
                            return newTicket.FromXmlString(ticketElement.ToString());
                        }).ToHashSet());
                    }
                    else return new Tuple<MessageType, string, HashSet<Ticket>>(MessageType.Error, "Unknown eror", null);
                }
                else return new Tuple<MessageType, string, HashSet<Ticket>>(message.Item1, message.Item2, null);

            }
            else
            {
                Logout();
                return new Tuple<MessageType, string, HashSet<Ticket>>(MessageType.Error, "Unknown eror", null);
            }
        }

        internal Tuple<MessageType, string, HashSet<Ticket>> ListUserTickets()
        {
            return ListTickets(TicketListType.all);
        }

        internal Tuple<MessageType, string> CreateTicket(string ticket)
        {
            string request = "<request><sessionid>" + SessionID.ToString() + "</sessionid>" + "<type>" + RequestType.CreateTicket.ToString() + "</type>" + ticket + "</request><EOF>";
            string response = SendMessage(request);
            if (ResponseContainsReset(response)) Logout();
            return GetMessageOrError(response);
        }

        internal Tuple<MessageType, string> ApplyTicket(int idToApply)
        {
            string request = "<request><sessionid>" + SessionID.ToString() + "</sessionid>" + "<type>" + RequestType.ApplyTicketToSelf.ToString() + "</type><ticket>" + idToApply + "</ticket></request><EOF>";
            string response = SendMessage(request);
            if (ResponseContainsReset(response)) Logout();
            return GetMessageOrError(response);
        }

        internal Tuple<MessageType, string> ReleaseTicket(int idToRelease)
        {
            string request = "<request><sessionid>" + SessionID.ToString() + "</sessionid>" + "<type>" + RequestType.ReleaseTicket.ToString() + "</type><ticket>" + idToRelease + "</ticket></request><EOF>";
            string response = SendMessage(request);
            if (ResponseContainsReset(response)) Logout();
            return GetMessageOrError(response);
        }

        internal Tuple<MessageType, string> CompleteTicket(int idToComplete)
        {
            string request = "<request><sessionid>" + SessionID.ToString() + "</sessionid>" + "<type>" + RequestType.CompleteTicket.ToString() + "</type><ticket>" + idToComplete + "</ticket></request><EOF>";
            string response = SendMessage(request);
            if (ResponseContainsReset(response)) Logout();
            return GetMessageOrError(response);
        }

        internal Tuple<MessageType, string> RejectTicket(int idToReject)
        {
            string request = "<request><sessionid>" + SessionID.ToString() + "</sessionid>" + "<type>" + RequestType.RejectTicket.ToString() + "</type><ticket>" + idToReject + "</ticket></request><EOF>";
            string response = SendMessage(request);
            if (ResponseContainsReset(response)) Logout();
            return GetMessageOrError(response);
        }

        internal Tuple<MessageType, string, HashSet<Expense>> ListExpenses()
        {
            string request = "<request><sessionid>" + SessionID.ToString() + "</sessionid>" + "<type>" + RequestType.ListExpenses.ToString() + "</type></request><EOF>";
            string response = SendMessage(request);
            
            if (!ResponseContainsReset(response))
            {
                Tuple<MessageType, string> message = GetMessageOrError(response);
                if (message.Item1 == MessageType.None)
                {
                    XDocument responseXml;

                    try
                    {
                        responseXml = XDocument.Parse(response);
                    }
                    catch (Exception)
                    {
                        return new Tuple<MessageType, string, HashSet<Expense>>(MessageType.Error, "Unknown eror", null);
                    }

                    if (responseXml?.Root?.Element("Expenses") != null)
                    {
                        return new Tuple<MessageType, string, HashSet<Expense>>(MessageType.None, "", responseXml.Root.Element("Expenses").Descendants("Expense").Select(ticketElement =>
                        {
                            Expense newExpense = new Expense();
                            return newExpense.FromXmlString(ticketElement.ToString());
                        }).ToHashSet());
                    }
                    else return new Tuple<MessageType, string, HashSet<Expense>>(MessageType.Error, "Unknown eror", null);
                }
                else return new Tuple<MessageType, string, HashSet<Expense>>(message.Item1, message.Item2, null);

            }
            else
            {
                Logout();
                return new Tuple<MessageType, string, HashSet<Expense>>(MessageType.Error, "Unknown eror", null);
            }
        }

        internal Tuple<MessageType, string> EditExpense(string expense)
        {
            string request = "<request><sessionid>" + SessionID.ToString() + "</sessionid>" + "<type>" + RequestType.EditExpense.ToString() + "</type>" + expense + "</request><EOF>";
            string response = SendMessage(request);
            if (ResponseContainsReset(response)) Logout();
            return GetMessageOrError(response);
        }
        internal Tuple<MessageType, string> DeleteExpense(int idToDelete)
        {
            string request = "<request><sessionid>" + SessionID.ToString() + "</sessionid>" + "<type>" + RequestType.DeleteExpense.ToString() + "</type><expense><id>" + idToDelete + "</id></expense></request><EOF>";
            string response = SendMessage(request);
            if (ResponseContainsReset(response)) Logout();
            return GetMessageOrError(response);
        }

        internal Tuple<MessageType, string> ImportExpenses(string filestring)
        {
            string request = "<request><sessionid>" + SessionID.ToString() + "</sessionid>" + "<type>" + RequestType.ImportExpenses.ToString() + "</type>" + filestring + "</request><EOF>";
            string response = SendMessage(request);
            if (ResponseContainsReset(response)) Logout();
            return GetMessageOrError(response);
        }

        internal Tuple<MessageType, string, HashSet<Expense>> SearchExpenses(string searchString, SearchExpenseBy searchExpenseBy)
        {
            string request = "<request><sessionid>" + SessionID.ToString() + "</sessionid>" + "<type>" + RequestType.SearchExpenses.ToString() + "</type>" + "<search>" + searchString + "</search>" + "<searchby>" + searchExpenseBy.ToString() + "</searchby>" + "</request><EOF>";
            string response = SendMessage(request);

            if (!ResponseContainsReset(response))
            {
                Tuple<MessageType, string> message = GetMessageOrError(response);
                if (message.Item1 == MessageType.None)
                {
                    XDocument responseXml;

                    try
                    {
                        responseXml = XDocument.Parse(response);
                    }
                    catch (Exception)
                    {
                        return new Tuple<MessageType, string, HashSet<Expense>>(MessageType.Error, "Unknown eror", null);
                    }

                    if (responseXml?.Root?.Element("Expenses") != null)
                    {
                        return new Tuple<MessageType, string, HashSet<Expense>>(MessageType.None, "", responseXml.Root.Element("Expenses").Descendants("Expense").Select(ticketElement =>
                        {
                            Expense newExpense = new Expense();
                            return newExpense.FromXmlString(ticketElement.ToString());
                        }).ToHashSet());
                    }
                    else return new Tuple<MessageType, string, HashSet<Expense>>(MessageType.Error, "Unknown eror", null);
                }
                else return new Tuple<MessageType, string, HashSet<Expense>>(message.Item1, message.Item2, null);

            }
            else
            {
                Logout();
                return new Tuple<MessageType, string, HashSet<Expense>>(MessageType.Error, "Unknown eror", null);
            }
        }

        internal Tuple<MessageType, string> CreateExpense(string expense)
        {
            string request = "<request><sessionid>" + SessionID.ToString() + "</sessionid>" + "<type>" + RequestType.CreateExpense.ToString() + "</type>" + expense + "</request><EOF>";
            string response = SendMessage(request);
            if (ResponseContainsReset(response)) Logout();
            return GetMessageOrError(response);
        }

        internal Tuple<MessageType, string, HashSet<Income>> ListIncomes()
        {
            string request = "<request><sessionid>" + SessionID.ToString() + "</sessionid>" + "<type>" + RequestType.ListIncome.ToString() + "</type></request><EOF>";
            string response = SendMessage(request);
            

            if (!ResponseContainsReset(response))
            {
                Tuple<MessageType, string> message = GetMessageOrError(response);
                if (message.Item1 == MessageType.None)
                {
                    XDocument responseXml;

                    try
                    {
                        responseXml = XDocument.Parse(response);
                    }
                    catch (Exception)
                    {
                        return new Tuple<MessageType, string, HashSet<Income>>(MessageType.Error, "Unknown eror", null);
                    }

                    if (responseXml?.Root?.Element("Incomes") != null)
                    {
                        return new Tuple<MessageType, string, HashSet<Income>>(MessageType.None, "", responseXml.Root.Element("Incomes").Descendants("Income").Select(ticketElement =>
                        {
                            Income newIncome = new Income();
                            return newIncome.FromXmlString(ticketElement.ToString());
                        }).ToHashSet());
                    }
                    else return new Tuple<MessageType, string, HashSet<Income>>(MessageType.Error, "Unknown eror", null);
                }
                else return new Tuple<MessageType, string, HashSet<Income>>(message.Item1, message.Item2, null);

            }
            else
            {
                Logout();
                return new Tuple<MessageType, string, HashSet<Income>>(MessageType.Error, "Unknown eror", null);
            }
        }

        internal Tuple<MessageType, string> EditIncome(string income)
        {
            string request = "<request><sessionid>" + SessionID.ToString() + "</sessionid>" + "<type>" + RequestType.EditIncome.ToString() + "</type>" + income + "</request><EOF>";
            string response = SendMessage(request);
            if (ResponseContainsReset(response)) Logout();
            return GetMessageOrError(response);
        }
        internal Tuple<MessageType, string> DeleteIncome(int idToDelete)
        {
            string request = "<request><sessionid>" + SessionID.ToString() + "</sessionid>" + "<type>" + RequestType.DeleteIncome.ToString() + "</type><income><id>" + idToDelete + "</id></income></request><EOF>";
            string response = SendMessage(request);
            if (ResponseContainsReset(response)) Logout();
            return GetMessageOrError(response);
        }
        internal Tuple<MessageType, string> ImportIncomes(string filestring)
        {
            string request = "<request><sessionid>" + SessionID.ToString() + "</sessionid>" + "<type>" + RequestType.ImportIncome.ToString() + "</type>" + filestring + "</request><EOF>";
            string response = SendMessage(request);
            if (ResponseContainsReset(response)) Logout();
            return GetMessageOrError(response);
        }

        internal Tuple<MessageType, string, HashSet<Income>> SearchIncomes(string searchString, SearchIncomeBy searchIncomeBy)
        {
            string request = "<request><sessionid>" + SessionID.ToString() + "</sessionid>" + "<type>" + RequestType.SearchIncome.ToString() + "</type>" + "<search>" + searchString + "</search>" + "<searchby>" + searchIncomeBy.ToString() + "</searchby>" + "</request><EOF>";
            string response = SendMessage(request);

            if (!ResponseContainsReset(response))
            {
                Tuple<MessageType, string> message = GetMessageOrError(response);
                if (message.Item1 == MessageType.None)
                {
                    XDocument responseXml;

                    try
                    {
                        responseXml = XDocument.Parse(response);
                    }
                    catch (Exception)
                    {
                        return new Tuple<MessageType, string, HashSet<Income>>(MessageType.Error, "Unknown eror", null);
                    }

                    if (responseXml?.Root?.Element("Incomes") != null)
                    {
                        return new Tuple<MessageType, string, HashSet<Income>>(MessageType.None, "", responseXml.Root.Element("Incomes").Descendants("Income").Select(ticketElement =>
                        {
                            Income newIncome = new Income();
                            return newIncome.FromXmlString(ticketElement.ToString());
                        }).ToHashSet());
                    }
                    else return new Tuple<MessageType, string, HashSet<Income>>(MessageType.Error, "Unknown eror", null);
                }
                else return new Tuple<MessageType, string, HashSet<Income>>(message.Item1, message.Item2, null);
            }
            else
            {
                Logout();
                return new Tuple<MessageType, string, HashSet<Income>>(MessageType.Error, "Unknown eror", null);
            }
        }
        internal Tuple<MessageType, string> CreateIncome(string income)
        {
            string request = "<request><sessionid>" + SessionID.ToString() + "</sessionid>" + "<type>" + RequestType.CreateIncome.ToString() + "</type>" + income + "</request><EOF>";
            string response = SendMessage(request);
            if (ResponseContainsReset(response)) Logout();
            return GetMessageOrError(response);
        }
        public List<User> ListUsers()
        {
            string request = "<request><sessionid>" + SessionID.ToString() + "</sessionid>" + "<type>" + RequestType.ListUsers.ToString() + "</type>" + "</request><EOF>";
            string response = SendMessage(request);
            if (!ResponseContainsReset(response))
            {
                XDocument responseXml;

                try
                {
                    responseXml = XDocument.Parse(response);
                }
                catch (Exception)
                {
                    return new List<User>();
                }

                if (responseXml?.Root?.Element("Users") != null)
                {
                    return responseXml.Root.Element("Users").Descendants("User").Select(userElement =>
                    {
                        User newUser = new User();
                        return newUser.FromXmlString(userElement.ToString());
                    }).ToList();
                }
                else return new List<User>();
            }
            else
            {
                Logout();
                return new List<User>();
            }
        }

        public Tuple<MessageType, string> DeleteUser(int idToDelete)
        {
            string request = "<request><sessionid>" + SessionID.ToString() + "</sessionid>" + "<type>" + RequestType.DeleteUser.ToString() + "</type><User><Id>" + idToDelete + "</Id></User></request><EOF>";
            string response = SendMessage(request);
            if (ResponseContainsReset(response)) Logout();
            return GetMessageOrError(response);
        }

        public Tuple<MessageType, string> CreateUser(string user)
        {
            string request = "<request><sessionid>" + SessionID.ToString() + "</sessionid>" + "<type>" + RequestType.AddUser.ToString() + "</type>" + user + "</request><EOF>";
            string response = SendMessage(request);
            if (ResponseContainsReset(response)) Logout();
            return GetMessageOrError(response);
        }

        internal Tuple<MessageType, string> ChangeUserType(int idToChange, UserType userType)
        {
            string request = "<request><sessionid>" + SessionID.ToString() + "</sessionid>" + "<type>" + RequestType.ChangeUserType.ToString() + "</type>" + "<User><Id>" + idToChange + "</Id><Type>" + userType.ToString() + "</Type></User>" + "</request><EOF>";
            string response = SendMessage(request);
            if (ResponseContainsReset(response)) Logout();
            return GetMessageOrError(response);
        }

        internal Tuple<MessageType, string> ChangeUserPassword(int idToChange, string password)
        {
            string request = "<request><sessionid>" + SessionID.ToString() + "</sessionid>" + "<type>" + RequestType.ChangeUserPassword.ToString() + "</type>" + "<User><Id>" + idToChange + "</Id><Password>" + password + "</Password></User>" + "</request><EOF>";
            string response = SendMessage(request);
            if (ResponseContainsReset(response)) Logout();
            return GetMessageOrError(response);
        }
    }
}