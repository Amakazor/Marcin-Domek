using Marcin_Domek.Src;
using Marcin_Domek_Server.Src.Extension;
using Marcin_Domek_Server.Src.Requests;
using Marcin_Domek_Server.Src.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Marcin_Domek_Console.Src
{
    internal class Menu
    {
        int CurrentCommand;
        int PreviousCommand;
        readonly Client Client;
        bool ShouldQuit;

        private readonly Dictionary<UserType, Tuple<Dictionary<int, int>, Dictionary<int, Tuple<Action, string, Dictionary<int, int>>>>> MenuStructure;

        public Menu(Client client)
        {
            Client = client;
            CurrentCommand = 0;
            PreviousCommand = 0;
            ShouldQuit = false; 

            MenuStructure = new Dictionary<UserType, Tuple<Dictionary<int, int>, Dictionary<int, Tuple<Action, string, Dictionary<int, int>>>>>
            {
                {
                    UserType.Admin,
                    new Tuple<Dictionary<int, int>, Dictionary<int, Tuple<Action, string, Dictionary<int, int>>>> 
                    (
                        new Dictionary<int, int>{ {1, 1}, {2, 2}, {3, 3}, {4, 4}, {5, 5}, {98, 98}, { 99, 99} },
                        new Dictionary<int, Tuple<Action, string, Dictionary<int, int>>>
                        {
                            {1,  new Tuple<Action, string, Dictionary<int, int>>(ListUsers, "List users", new Dictionary<int, int>{ { 99, 97 } }) },

                            {2,  new Tuple<Action, string, Dictionary<int, int>>(CreateUser, "Create new user", new Dictionary<int, int>()) },
                            {3,  new Tuple<Action, string, Dictionary<int, int>>(DeleteUser, "Delete existing user", new Dictionary<int, int>()) },
                            {4,  new Tuple<Action, string, Dictionary<int, int>>(ChangeUserPassword, "Change users password", new Dictionary<int, int>()) },
                            {5,  new Tuple<Action, string, Dictionary<int, int>>(ChangeUserType, "Change users type", new Dictionary<int, int>()) },

                            {97, new Tuple<Action, string, Dictionary<int, int>>(Return, "Return", new Dictionary<int, int>()) },
                            {98, new Tuple<Action, string, Dictionary<int, int>>(Logout, "Logout", new Dictionary<int, int>()) },
                            {99, new Tuple<Action, string, Dictionary<int, int>>(Quit, "Quit", new Dictionary<int, int>()) },
                        }
                    )
                },
                {
                    UserType.Helpdesk,
                    new Tuple<Dictionary<int, int>, Dictionary<int, Tuple<Action, string, Dictionary<int, int>>>> 
                    (
                        new Dictionary<int, int>{ { 1, 1 }, { 2, 5 }, { 3, 6 }, { 4, 7 }, { 5, 8 }, {98, 98}, { 99, 99} },
                        new Dictionary<int, Tuple<Action, string, Dictionary<int, int>>>
                        {
                            {1,  new Tuple<Action, string, Dictionary<int, int>>(null, "List tickets", new Dictionary<int, int>{ { 1, 2 }, { 2, 3 }, { 3, 4 }, { 99, 97 } }) },
                            {2,  new Tuple<Action, string, Dictionary<int, int>>(ListMyTickets, "List my tickets", new Dictionary<int, int>{ { 99, 97 } }) },
                            {3,  new Tuple<Action, string, Dictionary<int, int>>(ListUnclaimedTickets, "List unclaimed tickets", new Dictionary<int, int>{ { 99, 97 } }) },
                            {4,  new Tuple<Action, string, Dictionary<int, int>>(ListAllTickets, "List my and unclaimed tickets", new Dictionary<int, int>{ { 99, 97 } }) },

                            {5,  new Tuple<Action, string, Dictionary<int, int>>(ApplyTicket, "Apply ticket to me", new Dictionary<int, int>()) },
                            {6,  new Tuple<Action, string, Dictionary<int, int>>(ReleaseTicket, "Release my ticket", new Dictionary<int, int>()) },
                            {7,  new Tuple<Action, string, Dictionary<int, int>>(CompleteTicket, "Complete my ticket", new Dictionary<int, int>()) },
                            {8,  new Tuple<Action, string, Dictionary<int, int>>(RejectTicket, "Reject ticket", new Dictionary<int, int>()) },

                            {97, new Tuple<Action, string, Dictionary<int, int>>(Return, "Return", new Dictionary<int, int>()) },
                            {98, new Tuple<Action, string, Dictionary<int, int>>(Logout, "Logout", new Dictionary<int, int>()) },
                            {99, new Tuple<Action, string, Dictionary<int, int>>(Quit, "Quit", new Dictionary<int, int>()) },
                        }
                    )
                },
                {
                    UserType.User,
                    new Tuple<Dictionary<int, int>, Dictionary<int, Tuple<Action, string, Dictionary<int, int>>>> 
                    (
                        new Dictionary<int, int>{ { 1, 1 }, { 2, 2 }, { 3, 3 }, {98, 98}, { 99, 99} },
                        new Dictionary<int, Tuple<Action, string, Dictionary<int, int>>>
                        {
                            {1,  new Tuple<Action, string, Dictionary<int, int>>(null, "Expenses", new Dictionary<int, int>{ { 1, 11 }, { 2, 12 }, { 3, 13 }, { 4, 14 }, { 5, 15 }, { 6, 16 }, { 7, 17 }, { 99, 97 }}) },
                            {2,  new Tuple<Action, string, Dictionary<int, int>>(null, "Inomes", new Dictionary<int, int>{ { 1, 21 }, { 2, 22 }, { 3, 23 }, { 4, 24 }, { 5, 25 }, { 6, 26 }, { 7, 27 }, { 99, 97 }}) },
                            {3,  new Tuple<Action, string, Dictionary<int, int>>(null, "Tickets", new Dictionary<int, int>{ { 1, 51 }, { 2, 52 }, { 99, 97 }}) },

                            {11, new Tuple<Action, string, Dictionary<int, int>>(ListExpenses, "List my expenses", new Dictionary<int, int>{ { 99, 97 } }) },
                            {12, new Tuple<Action, string, Dictionary<int, int>>(null, "Search expenses", new Dictionary<int, int>{ { 1, 31 }, { 2, 32 }, { 3, 33 }, { 4, 34 }, { 99, 97 } }) },
                            {13, new Tuple<Action, string, Dictionary<int, int>>(CreateExpense, "Create new expense", new Dictionary<int, int>()) },
                            {14, new Tuple<Action, string, Dictionary<int, int>>(DeleteExpense, "Delete expense", new Dictionary<int, int>()) },
                            {15, new Tuple<Action, string, Dictionary<int, int>>(EditExpense, "Edit expense", new Dictionary<int, int>()) },
                            {16, new Tuple<Action, string, Dictionary<int, int>>(ExportExpenses, "Export expenses", new Dictionary<int, int>()) },
                            {17, new Tuple<Action, string, Dictionary<int, int>>(ImportExpenses, "Import expenses", new Dictionary<int, int>()) },

                            {21, new Tuple<Action, string, Dictionary<int, int>>(ListIncomes, "List my incomes", new Dictionary<int, int>{ { 99, 97 } }) },
                            {22, new Tuple<Action, string, Dictionary<int, int>>(null, "Search incomes", new Dictionary<int, int>{ { 1, 41 }, { 2, 42 }, { 3, 43 }, { 99, 97 } }) },
                            {23, new Tuple<Action, string, Dictionary<int, int>>(CreateIncome, "Create new income", new Dictionary<int, int>()) },
                            {24, new Tuple<Action, string, Dictionary<int, int>>(DeleteIncome, "Delete income", new Dictionary<int, int>()) },
                            {25, new Tuple<Action, string, Dictionary<int, int>>(EditIncome, "Edit income", new Dictionary<int, int>()) },
                            {26, new Tuple<Action, string, Dictionary<int, int>>(ExportIncomes, "Export incomes", new Dictionary<int, int>()) },
                            {27, new Tuple<Action, string, Dictionary<int, int>>(ImportIncomes, "Import incomes", new Dictionary<int, int>()) },

                            {31, new Tuple<Action, string, Dictionary<int, int>>(SearchExpensesByName, "Search expenses by name", new Dictionary<int, int>{ { 99, 97 } }) },
                            {32, new Tuple<Action, string, Dictionary<int, int>>(SearchExpensesByType, "Search expenses by type", new Dictionary<int, int>{ { 99, 97 } }) },
                            {33, new Tuple<Action, string, Dictionary<int, int>>(SearchExpensesByPlace, "Search expenses by place", new Dictionary<int, int>{ { 99, 97 } }) },
                            {34, new Tuple<Action, string, Dictionary<int, int>>(SearchExpensesBySource, "Search expenses by source", new Dictionary<int, int>{ { 99, 97 } }) },

                            {41, new Tuple<Action, string, Dictionary<int, int>>(SearchIncomesByName , "Search incomes by name", new Dictionary<int, int>{ { 99, 97 } }) },
                            {42, new Tuple<Action, string, Dictionary<int, int>>(SearchIncomesBySource , "Search incomes by source", new Dictionary<int, int>{ { 99, 97 } }) },
                            {43, new Tuple<Action, string, Dictionary<int, int>>(SearchIncomesByDestination , "Search incomes by destination", new Dictionary<int, int>{ { 99, 97 } }) },

                            {51, new Tuple<Action, string, Dictionary<int, int>>(ListUserTickets, "List my tickets", new Dictionary<int, int>{ { 99, 97 } }) },
                            {52, new Tuple<Action, string, Dictionary<int, int>>(CreateTicket, "Create new ticket", new Dictionary<int, int>()) },

                            {97, new Tuple<Action, string, Dictionary<int, int>>(Return, "Return", new Dictionary<int, int>()) },
                            {98, new Tuple<Action, string, Dictionary<int, int>>(Logout, "Logout", new Dictionary<int, int>()) },
                            {99, new Tuple<Action, string, Dictionary<int, int>>(Quit, "Quit", new Dictionary<int, int>()) },
                        }
                    )
                }
            };

            Reset();
        }

        private void Return() => CurrentCommand = PreviousCommand;

        private void Quit()
        {
            if (Client.ValidSession)
            {
                Client.Logout();
            }
            ShouldQuit = true;
        }

        public void Loop()
        {
            while (!ShouldQuit)
            {
                Dictionary<int, int> currentlyPossibleInputs = Display();
                if (currentlyPossibleInputs != null)
                {
                    ParseInput(currentlyPossibleInputs);
                    InvokeActionFromInput();
                }
                CheckSession();
            }
        }

        public Dictionary<int, int> Display()
        {
            Dictionary<int, int> CurrentlyPossibleInputs;

            if (Client.UserType != UserType.None)
            {
                if (CurrentCommand == 0)
                {
                    CurrentlyPossibleInputs = MenuStructure[Client.UserType].Item1;
                }
                else
                {
                    CurrentlyPossibleInputs = MenuStructure[Client.UserType].Item2[CurrentCommand].Item3;
                }

                if (CurrentlyPossibleInputs.Any())
                {
                    Console.WriteLine();
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Available operations:");
                    Console.ResetColor();

                    CurrentlyPossibleInputs.ToList().ForEach(possibleInput =>
                    {
                        Console.WriteLine(possibleInput.Key.ToString() + ". " + MenuStructure[Client.UserType].Item2[possibleInput.Value].Item2);
                    });
                }

                return CurrentlyPossibleInputs;
            }
            else
            {
                return null;
            }
        }

        private void ParseInput(Dictionary<int, int> currentlyPossibleInputs)
        {
            int currentInput;
            bool successfullInput = true;

            do
            {
                Console.WriteLine();
                if (!successfullInput) Utility.WriteError("Invalid option");

                Console.WriteLine("Please select option from menu:");
                successfullInput = int.TryParse(Console.ReadLine(), out currentInput) && currentlyPossibleInputs.ContainsKey(currentInput);

            } while (!successfullInput);

            PreviousCommand = !(new List<int> { 97, 98, 99, CurrentCommand }.Contains(CurrentCommand)) ? CurrentCommand : PreviousCommand;
            CurrentCommand = currentlyPossibleInputs[currentInput];
        }

        private void InvokeActionFromInput()
        {
            MenuStructure[Client.UserType].Item2[CurrentCommand].Item1?.Invoke();
        }

        private void Logout()
        {
            Client.Logout();
            Reset();
        }

        private void CheckSession()
        {
            if (!Client.ValidSession) Reset();
        }

        private void Reset()
        {
            CurrentCommand = 0;
            PreviousCommand = 0;
            Console.Clear();

            bool canConnect = true;
            do
            {
                if (!canConnect)
                {
                    Utility.WriteError("Connection failed.");
                    Utility.WriteError("Server seems to be offline. Press any key to try to connect again.");
                    Console.ReadKey();
                    Console.Clear();
                }
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Trying to connect to a server...");
                Console.ResetColor();
                canConnect = Client.TryConnection();
            } while (!canConnect);

            Utility.WriteMessage("Server is online.");

            if (Client.UserType == UserType.None)
            {
                do
                {
                    if (Client.TryLogin()) break;
                } while (true);
            }
        }

        private void SearchIncomesByDestination() => SearchIncome(SearchIncomeBy.Destination);

        private void SearchIncomesBySource() => SearchIncome(SearchIncomeBy.Source);

        private void SearchIncomesByName() => SearchIncome(SearchIncomeBy.Name);

        private void SearchIncome(SearchIncomeBy searchIncomeBy)
        {
            string searchString = Utility.GetStringFromConsole("Enter phrase to search for", "Phrase canot be empty", 4, "Phrase is too short");

            Tuple<MessageType, string, HashSet<Income>> incomes = Client.SearchIncomes(searchString, searchIncomeBy);

            if (incomes.Item1 == MessageType.None)
            {
                DisplayIncomes(incomes.Item3);
            }
            else
            {
                ShowMessageOrError(new Tuple<MessageType, string>(incomes.Item1, incomes.Item2));
                Return();
            }
        }

        private void SearchExpensesBySource() => SearchExpense(SearchExpenseBy.Source);

        private void SearchExpensesByPlace() => SearchExpense(SearchExpenseBy.Place);

        private void SearchExpensesByType() => SearchExpense(SearchExpenseBy.Type);

        private void SearchExpensesByName() => SearchExpense(SearchExpenseBy.Name);

        private void SearchExpense(SearchExpenseBy searchExpenseBy)
        {
            string searchString = Utility.GetStringFromConsole("Enter phrase to search for", "Phrase canot be empty", 4, "Phrase is too short");

            Tuple<MessageType, string, HashSet<Expense>> expenses = Client.SearchExpenses(searchString, searchExpenseBy);

            if (expenses.Item1 == MessageType.None)
            {
                DisplayExpenses(expenses.Item3);
            }
            else
            {
                ShowMessageOrError(new Tuple<MessageType, string>(expenses.Item1, expenses.Item2));
                Return();
            }
        }

        private void ExportExpenses()
        {
            Tuple<MessageType, string, HashSet<Expense>> expenses = Client.ListExpenses();

            if (expenses.Item1 == MessageType.None)
            {
                List<Expense> incomesList = expenses.Item3.ToList();

                StringBuilder stringBuilder = new StringBuilder("<Expenses>");
                incomesList.ForEach(income => stringBuilder.Append(income.ToXmlString()));
                stringBuilder.Append("</Expenses>");

                string filename = FileManager.SaveXMLFile("expense", stringBuilder.ToString());
                if (filename != null && filename.Length > 0)
                {
                    Utility.WriteMessage("Exponses exported to: " + filename);
                }
                else
                {
                    Utility.WriteError("Unkown error during expenses export");
                }
            }
            else ShowMessageOrError(new Tuple<MessageType, string>(expenses.Item1, expenses.Item2));

            Return();
        }

        private void ImportExpenses()
        {
            string filename = Utility.GetStringFromConsole("Enter a valid expenses file path", "File path cannot be empty");

            string filestring = FileManager.LoadXMLFile(filename);

            if (filestring != null && filestring.Length > 0)
            {
                Tuple<MessageType, string> response = Client.ImportExpenses(filestring);
                ShowMessageOrError(response);
                Return();
            }
            else
            {
                Utility.WriteError("Incomes file does not exist or is empty");
            }
        }

        private void ExportIncomes()
        {
            Tuple<MessageType, string, HashSet<Income>> incomes = Client.ListIncomes();

            if (incomes.Item1 == MessageType.None)
            {
                DisplayIncomes(incomes.Item3);

                List<Income> incomesList = incomes.Item3.ToList();

                StringBuilder stringBuilder = new StringBuilder("<Incomes>");
                incomesList.ForEach(income => stringBuilder.Append(income.ToXmlString()));
                stringBuilder.Append("</Incomes>");

                string filename = FileManager.SaveXMLFile("income", stringBuilder.ToString());
                if (filename != null && filename.Length > 0)
                {
                    Utility.WriteMessage("Incomes exported to: " + filename);
                }
                else
                {
                    Utility.WriteError("Unkown error during incomes export");
                }
            }
            else ShowMessageOrError(new Tuple<MessageType, string>(incomes.Item1, incomes.Item2));

            Return();
        }

        private void ImportIncomes()
        {
            string filename = Utility.GetStringFromConsole("Enter a valid incomes file path", "File path cannot be empty");

            string filestring = FileManager.LoadXMLFile(filename);

            if (filestring != null && filestring.Length > 0)
            {
                Tuple<MessageType, string> response = Client.ImportIncomes(filestring);
                ShowMessageOrError(response);
                Return();
            }
            else 
            {
                Utility.WriteError("Incomes file does not exist or is empty");
            }

        }

        private void EditIncome()
        {
            Tuple<MessageType, string, HashSet<Income>> incomes = Client.ListIncomes();

            if (incomes.Item1 == MessageType.None)
            {
                DisplayIncomes(incomes.Item3);

                List<Income> incomesList = incomes.Item3.ToList();

                int idToEdit = Utility.GetIntInListFromConsoleOrCancel("Enter valid income ID to edit, or press 'q' to cancel", "This is not valid income ID", incomesList.Select(income => income.Id).ToHashSet());

                if (idToEdit != -1)
                {
                    Income incomeToEdit = incomesList.First(income => income.Id == idToEdit);
                    incomeToEdit.Edit();
                    ShowMessageOrError(Client.EditIncome(incomeToEdit.ToXmlString()));
                }

            }
            else ShowMessageOrError(new Tuple<MessageType, string>(incomes.Item1, incomes.Item2));

            Return();
        }

        private void EditExpense()
        {
            Tuple<MessageType, string, HashSet<Expense>> expenses = Client.ListExpenses();

            if (expenses.Item1 == MessageType.None)
            {
                DisplayExpenses(expenses.Item3);

                List<Expense> expensesList = expenses.Item3.ToList();

                int idToEdit = Utility.GetIntInListFromConsoleOrCancel("Enter valid expense ID to edit, or press 'q' to cancel", "This is not valid expense ID", expensesList.Select(expense => expense.Id).ToHashSet());

                if (idToEdit != -1)
                {
                    Expense expenseToEdit = expensesList.First(expense => expense.Id == idToEdit);
                    expenseToEdit.Edit();
                    ShowMessageOrError(Client.EditExpense(expenseToEdit.ToXmlString()));
                }

            }
            else ShowMessageOrError(new Tuple<MessageType, string>(expenses.Item1, expenses.Item2));

            Return();
        }

        private void DeleteIncome()
        {
            Tuple<MessageType, string, HashSet<Income>> incomes = Client.ListIncomes();

            if (incomes.Item1 == MessageType.None)
            {
                DisplayIncomes(incomes.Item3);

                List<Income> incomesList = incomes.Item3.ToList();

                int idToDelete = Utility.GetIntInListFromConsoleOrCancel("Enter valid income ID to edit, or press 'q' to cancel", "This is not valid income ID", incomesList.Select(income => income.Id).ToHashSet());

                if (idToDelete != -1)
                {
                    ShowMessageOrError(Client.DeleteIncome(idToDelete));
                }

            }
            else ShowMessageOrError(new Tuple<MessageType, string>(incomes.Item1, incomes.Item2));

            Return();
        }

        private void DeleteExpense()
        {
            Tuple<MessageType, string, HashSet<Expense>> expenses = Client.ListExpenses();

            if (expenses.Item1 == MessageType.None)
            {
                DisplayExpenses(expenses.Item3);

                List<Expense> expensesList = expenses.Item3.ToList();

                int idToDelete = Utility.GetIntInListFromConsoleOrCancel("Enter valid expense ID to delete, or press 'q' to cancel", "This is not valid expense ID", expensesList.Select(expense => expense.Id).ToHashSet());

                if (idToDelete != -1)
                {
                    ShowMessageOrError(Client.DeleteExpense(idToDelete));
                }

            }
            else ShowMessageOrError(new Tuple<MessageType, string>(expenses.Item1, expenses.Item2));

            Return();
        }

        private void ListIncomes()
        {
            Tuple<MessageType, string, HashSet<Income>> incomes = Client.ListIncomes();

            if (incomes.Item1 == MessageType.None)
            {
                DisplayIncomes(incomes.Item3);
            }
            else
            {
                ShowMessageOrError(new Tuple<MessageType, string>(incomes.Item1, incomes.Item2));
                Return();
            }
        }

        private void ListExpenses()
        {
            Tuple<MessageType, string, HashSet<Expense>> expenses = Client.ListExpenses();

            if (expenses.Item1 == MessageType.None)
            {
                DisplayExpenses(expenses.Item3);
            }
            else
            {
                ShowMessageOrError(new Tuple<MessageType, string>(expenses.Item1, expenses.Item2));
                Return();
            }
        }

        private void DisplayExpenses(HashSet<Expense> expenses)
        {
            expenses.ToList().ForEach(expenses =>
            {
                Console.WriteLine();
                Console.WriteLine("ID: " + expenses.Id);
                Console.WriteLine("Name: " + expenses.Name);
                Console.WriteLine("Amount: " + expenses.Amount);
                Console.WriteLine("Type: " + expenses.Type);
                Console.WriteLine("Place: " + expenses.Place);
                Console.WriteLine("Source: " + expenses.Source);
                Console.WriteLine("Date: " + expenses.Date);
            });
        }
        
        private void DisplayIncomes(HashSet<Income> incomes)
        {
            incomes.ToList().ForEach(income =>
            {
                Console.WriteLine();
                Console.WriteLine("ID: " + income.Id);
                Console.WriteLine("Name: " + income.Name);
                Console.WriteLine("Amount: " + income.Amount);
                Console.WriteLine("Source: " + income.Source);
                Console.WriteLine("Destination: " + income.Destination);
                Console.WriteLine("Date: " + income.Date);
            });
        }

        private void CreateIncome()
        {
            ShowMessageOrError(Client.CreateIncome(Income.CreateIncome().ToXmlString()));
            Return();
        }

        private void CreateExpense()
        {
            ShowMessageOrError(Client.CreateExpense(Expense.CreateExpense().ToXmlString()));
            Return();
        }

        private void CreateTicket()
        {
            ShowMessageOrError(Client.CreateTicket(Ticket.CreateTicket().ToXmlString()));
            Return();
        }

        private void ApplyTicket()
        {
            Tuple<MessageType, string, HashSet<Ticket>> tickets = Client.ListTickets(TicketListType.unclaimed);
            ParseTickets(tickets);

            if (tickets.Item1 == MessageType.None)
            {
                int idToApply = Utility.GetIntInListFromConsoleOrCancel("Enter valid ticket ID to apply, or press 'q' to cancel", "This is not valid ticket ID", tickets.Item3.Select(ticket => ticket.Id).ToHashSet());
                
                if (idToApply != -1)
                {
                    ShowMessageOrError(Client.ApplyTicket(idToApply));
                }

                Return();
            }
        }

        private void ReleaseTicket()
        {
            Tuple<MessageType, string, HashSet<Ticket>> tickets = Client.ListTickets(TicketListType.my);
            ParseTickets(tickets);

            if (tickets.Item1 == MessageType.None)
            {
                int idToRelease = Utility.GetIntInListFromConsoleOrCancel("Enter valid ticket ID to release, or press 'q' to cancel", "This is not valid ticket ID", tickets.Item3.Select(ticket => ticket.Id).ToHashSet());

                if (idToRelease != -1)
                {
                    ShowMessageOrError(Client.ReleaseTicket(idToRelease));
                }

                Return();
            }
        }

        private void CompleteTicket()
        {
            Tuple<MessageType, string, HashSet<Ticket>> tickets = Client.ListTickets(TicketListType.my);
            ParseTickets(tickets);

            if (tickets.Item1 == MessageType.None)
            {
                int idToComplete = Utility.GetIntInListFromConsoleOrCancel("Enter valid ticket ID to complete, or press 'q' to cancel", "This is not valid ticket ID", tickets.Item3.Select(ticket => ticket.Id).ToHashSet());

                if (idToComplete != -1)
                {
                    ShowMessageOrError(Client.CompleteTicket(idToComplete));
                }

                Return();
            }
        }

        private void RejectTicket()
        {
            Tuple<MessageType, string, HashSet<Ticket>> tickets = Client.ListTickets(TicketListType.all);
            ParseTickets(tickets);

            if (tickets.Item1 == MessageType.None)
            {
                int idToReject = Utility.GetIntInListFromConsoleOrCancel("Enter valid ticket ID to reject, or press 'q' to cancel", "This is not valid ticket ID", tickets.Item3.Select(ticket => ticket.Id).ToHashSet());

                if (idToReject != -1)
                {
                    ShowMessageOrError(Client.RejectTicket(idToReject));
                }

                Return();
            }
        }

        private void ListUserTickets() => ParseTickets(Client.ListUserTickets());

        private void ListAllTickets() => ParseTickets(Client.ListTickets(TicketListType.all));

        private void ListUnclaimedTickets() => ParseTickets(Client.ListTickets(TicketListType.unclaimed));

        private void ListMyTickets() => ParseTickets(Client.ListTickets(TicketListType.my));

        private void ParseTickets(Tuple<MessageType, string, HashSet<Ticket>> tickets)
        {
            if (tickets.Item1 == MessageType.None)
            {
                DisplayTickets(tickets.Item3);
            }
            else ShowMessageOrError(new Tuple<MessageType, string>(tickets.Item1, tickets.Item2));
        }

        private void DisplayTickets(HashSet<Ticket> tickets)
        {
            tickets.ToList().ForEach(ticket => {
                Console.WriteLine();
                Console.WriteLine("ID: " + ticket.Id);
                Console.WriteLine("Message: " + ticket.Message);
                Console.WriteLine("Date: " + ticket.Date);
                Console.WriteLine("Status: " + ticket.Status.ToString());
            });
        }

        private void DeleteUser()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Currently registered users:");
            Console.ResetColor();

            List<User> users = Client.ListUsers();
            users.ForEach(user => {Console.WriteLine(user.Id + ". " + user.Login);});

            int idToDelete = Utility.GetIntInListFromConsoleOrCancel("Enter valid user ID to delete, or press 'q' to cancel", "This is not valid user ID", users.Select(user => user.Id).ToHashSet());

            if (idToDelete != -1)
            {
                ShowMessageOrError(Client.DeleteUser(idToDelete));
            }

            Return();
        }

        private void ChangeUserType()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Currently registered users:");
            Console.ResetColor();

            List<User> users = Client.ListUsers();
            users.ForEach(user => { Console.WriteLine(user.Id + ". " + user.Login); });

            int idToChange = Utility.GetIntInListFromConsoleOrCancel("Enter valid user ID to change, or press 'q' to cancel", "This is not valid user ID", users.Select(user => user.Id).ToHashSet());

            if (idToChange != -1)
            {
                UserType userType = Utility.GetUserTypeFromConsole("Select valid user type");

                ShowMessageOrError(Client.ChangeUserType(idToChange, userType));
            }

            Return();
        }

        private void ChangeUserPassword()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Currently registered users:");
            Console.ResetColor();

            List<User> users = Client.ListUsers();
            users.ForEach(user => { Console.WriteLine(user.Id + ". " + user.Login); });

            int idToChange = Utility.GetIntInListFromConsoleOrCancel("Enter valid user ID to change, or press 'q' to cancel", "This is not valid user ID", users.Select(user => user.Id).ToHashSet());

            if (idToChange != -1)
            {
                string password = Utility.GetStringFromConsole("Enter users password", "Password can't be empty", 8, "Password must be at least 8 characters long");

                ShowMessageOrError(Client.ChangeUserPassword(idToChange, password));
            }

            Return();
        }

        private void ShowMessageOrError(Tuple<MessageType, string> message)
        {
            if (message.Item1 == MessageType.Error)
            {
                Utility.WriteError(message.Item2);
            }
            else
            {
                Utility.WriteMessage(message.Item2);
            }
        }
        private void CreateUser()
        {
            ShowMessageOrError(Client.CreateUser(User.CreateUser().ToXmlString()));

            Return();
        }

        private void ListUsers()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Currently registered users:");
            Console.ResetColor();

            List<User> users = Client.ListUsers();
            users.ForEach(user => {
                Console.WriteLine();
                Console.WriteLine(user.FirstName + " " + user.LastName);
                Console.WriteLine("ID: " + user.Id);
                Console.WriteLine("Login: " + user.Login);
                Console.WriteLine("Type: " + user.UserType.ToString());
            });
        }
    }
}