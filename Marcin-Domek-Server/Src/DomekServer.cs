using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Xml.Linq;
using System.Linq;
using Marcin_Domek_Server.Src.Users;
using Marcin_Domek_Server.Src.Extension;
using Marcin_Domek_Server.Src.Requests;

namespace Marcin_Domek_Server.Src
{
    class DomekServer
    {
        private IPHostEntry IPHost { get; }
        private IPAddress IPAddress { get; }
        private IPEndPoint LocalEndPoint { get; }
        private Socket ListnenerSocket { get; }

        private HashSet<Session> Sessions { get; }

        public DomekServer()
        {
            IPHost = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress = IPHost.AddressList[0];
            LocalEndPoint = new IPEndPoint(IPAddress, 12121);

            ListnenerSocket = new Socket(IPAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            Sessions = new HashSet<Session>();

            Open();
        }

        private void Open()
        {
            try
            {
                ListnenerSocket.Bind(LocalEndPoint);
                ListnenerSocket.Listen(10);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private void Close()
        {
            try
            {
                ListnenerSocket.Shutdown(SocketShutdown.Both);
                ListnenerSocket.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public void Loop()
        {
            try
            {
                while (true)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Awaiting connection...");
                    Console.ForegroundColor = ConsoleColor.White;

                    Socket clientSocket = ListnenerSocket.Accept();

                    byte[] bytes = new byte[1024];
                    string data = null;

                    while (true)
                    {
                        int numByte = clientSocket.Receive(bytes);

                        data += Encoding.UTF8.GetString(bytes, 0, numByte);

                        if (data.IndexOf("<EOF>") > -1)
                        {
                            break;
                        }
                    }

                    data = data[0..^5];

                    string response = "<EOF>";

                    if (!(data is null) && data.Length > 0)
                    {
                        Console.WriteLine("Text received: " + data);

                        response = ParseRequestAndGetResponse(data);
                    }

                    Console.WriteLine("Response prepared: ");
                    Console.WriteLine(response + "<EOF>");

                    byte[] message = Encoding.UTF8.GetBytes(response + "<EOF>");

                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine("Response sent.");
                    Console.ForegroundColor = ConsoleColor.White;

                    clientSocket.Send(message);
                    clientSocket.Shutdown(SocketShutdown.Both);
                    clientSocket.Close();

                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Connection terminated.");
                    Console.ForegroundColor = ConsoleColor.White;
                }
            }
            catch (Exception e)
            {
                Close();
                Console.WriteLine(e.ToString());
            }
        }

        private void ClearSessions()
        {
            foreach (Session session in Sessions)
            {
                DateTime now = DateTime.Now;
                now.AddMinutes(-30);

                if (session.LastUpdate.CompareTo(now) == -1)
                {
                    Sessions.Remove(session);
                }
            }
        }

        private string ParseRequestAndGetResponse(string request)
        {
            XDocument requestXML;

            try
            {
                requestXML = XDocument.Parse(request);
            }
            catch (Exception)
            {
                return "";
            }

            if (requestXML == null || requestXML.Root == null || requestXML.Root.Descendants().ToList().Count == 0 || requestXML.Root.Element("sessionid") == null) return "<response><reset>1</reset><reason>Empty request</reason></response>";

            Guid sessionid = Guid.Empty;
            sessionid = sessionid.ParseOrEmpty(requestXML.Root.Element("sessionid").Value);

            if (sessionid == Guid.Empty)
            {
                if (requestXML.Root.Element("login") != null && requestXML.Root.Element("password") != null)
                {
                    return ParseLogin(requestXML);
                }
                else
                {
                    return "<response><reset>1</reset><reason>Empty request</reason></response>";
                }
            }
            else
            {
                if (SessionExists(sessionid) && requestXML.Root.Element("type") != null)
                {
                    Session session = GetSession(sessionid);
                    UpdateSessionTime(session);

                    RequestType requestType = Enum.Parse<RequestType>(requestXML.Root.Element("type").Value, true);

                    if (requestType == RequestType.Logout)
                    {
                        RemoveSession(session);
                        return "<response><reset>1</reset><reason>Logged out</reason></response>";
                    }
                    else
                    {
                        return "<response><sessionid>" + sessionid + "</sessionid>" + (session.User switch
                        {
                            Admin user      => ParseAdminRequest(requestXML, user, requestType),
                            Helpdesk user   => ParseHelpdeskRequest(requestXML, user, requestType),
                            User user       => ParseUserRequest(requestXML, user, requestType),
                            _ => throw new Exception()
                        }) + "</response>";
                    }
                }
                else
                {
                    return "<response><reset>1</reset><reason>Session timed out</reason></response>";
                }
            }
        }

        private bool SessionExists(Guid sessionid)
        {
            return Sessions.Any(s => s.Sessionid == sessionid);
        }

        private Session GetSession(Guid sessionid)
        {
            return Sessions.First(s => s.Sessionid == sessionid);
        }

        private void UpdateSessionTime(Session session)
        {
            session.LastUpdate = DateTime.Now;
        }

        private string ParseAdminRequest(XDocument requestXML, Admin user, RequestType requestType)
        {
            return requestType switch
            {
                RequestType.ListUsers           => user.ListUsers(requestXML),
                RequestType.AddUser             => user.AddUser(requestXML),
                RequestType.DeleteUser          => user.DeleteUser(requestXML),
                RequestType.ChangeUserPassword  => user.ChangeUserPassword(requestXML),
                RequestType.ChangeUserType      => user.ChangeUserType(requestXML),
                _ => "<error>Non existing admin task</error>"
            };
        }

        private string ParseHelpdeskRequest(XDocument requestXML, Helpdesk user, RequestType requestType)
        {
            return requestType switch
            {
                RequestType.ApplyTicketToSelf   => user.ApplyTicketToSelf(requestXML),
                RequestType.ReleaseTicket       => user.ReleaseTicket(requestXML),
                RequestType.RejectTicket        => user.RejectTicket(requestXML),
                RequestType.CompleteTicket      => user.CompleteTicket(requestXML),
                RequestType.ListTickets         => user.ListTickets(requestXML),
                RequestType.ListMyTickets       => user.ListMyTickets(requestXML),
                RequestType.ListUnclaimedTickets=> user.ListUnclaimedTickets(requestXML),
                _ => "<error>Non existing helpdesk task</error>"
            };
        }

        private string ParseUserRequest(XDocument requestXML, User user, RequestType requestType)
        {
            return requestType switch
            {
                RequestType.ListTickets         => user.ListTickets(requestXML),
                RequestType.CreateTicket        => user.CreateTicket(requestXML),
                RequestType.CreateExpense       => user.CreateExpense(requestXML),
                RequestType.DeleteExpense       => user.DeleteExpense(requestXML),
                RequestType.EditExpense         => user.EditExpense(requestXML),
                RequestType.ListExpenses        => user.ListExpenses(requestXML),
                RequestType.CreateIncome        => user.CreateIncome(requestXML),
                RequestType.DeleteIncome        => user.DeleteIncome(requestXML),
                RequestType.EditIncome          => user.EditIncome(requestXML),
                RequestType.ListIncome          => user.ListIncome(requestXML),
                RequestType.ImportExpenses      => user.ImportExpenses(requestXML),
                RequestType.ImportIncome        => user.ImportIncome(requestXML),
                RequestType.SearchExpenses      => user.SearchExpenses(requestXML),
                RequestType.SearchIncome        => user.SearchIncome(requestXML),
                _ => "<error>Non existing user task</error>"
            };
        }

        private string ParseLogin(XDocument requestXML)
        {
            string login = requestXML.Root.Element("login").Value;
            string password = requestXML.Root.Element("password").Value;

            Console.WriteLine("Attempting to log in user " + login);
            switch (DatabaseHandler.Instance().CheckUserPassword(login, password))
            {
                case 1:
                    Session newSession = CreateSession(login);

                    Console.WriteLine("User " + login + " was successfully logged in. Session created. User permission level is: " + newSession.User.UserType.ToString());

                    return "<response><sessionid>" + newSession.Sessionid + "</sessionid>" + new User(newSession.User).ToXmlString() + "</response>";
                case 0:
                    Console.WriteLine("User " + login + " failed to log in. Wrong password.");
                    return "<response><reset>1</reset><reason>User failed to log in. Wrong password</reason></response>";
                default:
                    Console.WriteLine("User " + login + " doesn't exist.");
                    return "<response><reset>1</reset><reason>User failed to log in. Wrong login</reason></response>";
            }
        }

        private Session CreateSession(string login)
        {
            Session session = new Session(Guid.NewGuid(), UserFactory.CreateUser(login), DateTime.Now);
            Sessions.Add(session);
            return session;
        }

        private void RemoveSession(Session session)
        {
            Sessions.Remove(session);
        }
    }
}