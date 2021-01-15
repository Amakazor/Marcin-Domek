using Marcin_Domek_Server.Src.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Marcin_Domek_Server.Src.Users
{
    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public UserType UserType { get; set; }
        public string Login { get; set; }

        public User(int id, string firstName, string lastName, UserType userType, string login)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            UserType = userType;
            Login = login;
        }

        public User(User user)
        {
            Id = user.Id;
            FirstName = user.FirstName;
            LastName = user.LastName;
            UserType = user.UserType;
            Login = user.Login;
        }

        public User(string login)
        {
            Dictionary<string, string> userData = DatabaseHandler.Instance().GetUserData(login);

            Id = int.Parse(userData["UserID"]);
            FirstName = userData["FirstName"];
            LastName = userData["LastName"];
            UserType = (UserType)int.Parse(userData["Type"]);
            Login = userData["Login"];
        }

        public User(int id)
        {
            Dictionary<string, string> userData = DatabaseHandler.Instance().GetUserData(id);

            Id = int.Parse(userData["UserID"]);
            FirstName = userData["FirstName"];
            LastName = userData["LastName"];
            UserType = (UserType)int.Parse(userData["Type"]);
            Login = userData["Login"];
        }

        public User()
        {
            Id = 0;
            FirstName = null;
            LastName = null;
            UserType = UserType.User;
            Login = null;
        }

        internal virtual string ListTickets(XDocument requestXML)
        {
            HashSet<Tuple<int, string, string, string>> data;

            try
            {
                data = DatabaseHandler.Instance().GetUserTickets(Id);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return "<Error>Server encountered an Error during listing tickets. Try again later.</Error>";
            }

            if (data.Count > 0)
            {
                string returnList = "<Tickets>";

                data.ToList().ForEach(data => returnList += "<Ticket>" + "<Id>" + data.Item1 + "</Id>" + "<Message>"+data.Item2+"</Message>"+ "<Date>" + data.Item3 + "</Date>" + "<Status>" + data.Item4 + "</Status>" + "</Ticket>");

                return returnList + "</Tickets>";
            }
            else return "<Message>No tickets.</Message>";
        }

        internal string CreateTicket(XDocument requestXML)
        {
            if (requestXML.Root.Element("Ticket")?.Element("Message") != null)
            {
                if (DatabaseHandler.Instance().AddTicket(Id, requestXML.Root.Element("Ticket").Element("Message").Value) != -1)
                {
                    return "<Message>Ticket addded successfully</Message>";
                }
                else return "<Error>Ticket wasn't added, unknown error</Error>";

            }
            else return "<Error>Ticket wasn't added, missing message</Error>";
        }

        internal string CreateExpense(XDocument requestXML)
        {
            XElement expenseElement = requestXML.Root.Element("Expense") ?? (requestXML.Root.Name == "Expense" ? requestXML.Root  : null);
            if (expenseElement != null && expenseElement.Element("Name") != null && expenseElement.Element("Amount") != null && expenseElement.Element("Type") != null && expenseElement.Element("Place") != null && expenseElement.Element("Source") != null)
            {
                int incomeSum = DatabaseHandler.Instance().IncomeSum(Id);
                int expenseSum = DatabaseHandler.Instance().ExpenseSum(Id);

                if (incomeSum != -1 && expenseSum != -1)
                {
                    if (incomeSum >= expenseSum + int.Parse(expenseElement.Element("Amount").Value))
                    {
                        if (expenseElement.Element("Date") != null)
                        {
                            try
                            {
                                if (DatabaseHandler.Instance().AddExpenseWithDate(expenseElement.Element("Name").Value, int.Parse(expenseElement.Element("Amount").Value), expenseElement.Element("Type").Value, expenseElement.Element("Place").Value, expenseElement.Element("Source").Value, expenseElement.Element("Date").Value, Id) == 1)
                                {
                                    return "<Message>Expense addded successfully</Message>";
                                }
                                else return "<Error>Expense wasn't added, unknown error</Error>";
                            }
                            catch (Exception)
                            {
                                return "<Error>Expense wasn't added, unknown error</Error>";
                            }
                        }
                        else
                        {
                            try
                            {
                                if (DatabaseHandler.Instance().AddExpense(expenseElement.Element("Name").Value, int.Parse(expenseElement.Element("Amount").Value), expenseElement.Element("Type").Value, expenseElement.Element("Place").Value, expenseElement.Element("Source").Value, Id) == 1)
                                {
                                    return "<Message>Expense addded successfully</Message>";
                                }
                                else return "<Error>Expense wasn't added, unknown error</Error>";
                            }
                            catch (Exception)
                            {
                                return "<Error>Expense wasn't added, unknown error</Error>";
                            }
                            
                        }
                            
                    }
                    else return "<Error>Expense wasn't added, not enough budget</Error>";
                }
                else return "<Error>Expense wasn't added, unknown error</Error>";
                
            }
            else return "<Error>Expense wasn't added, missing data</Error>";
        }

        internal string DeleteExpense(XDocument requestXML)
        {
            XElement expenseElement = requestXML.Root.Element("expense");
            if (expenseElement != null && expenseElement.Element("id") != null)
            {
                if (DatabaseHandler.Instance().DeleteExpense(int.Parse(expenseElement.Element("id").Value), Id) == 1)
                {
                    return "<Message>Expense deleted successfully</Message>";
                }
                else return "<Error>Expense wasn't deleted, unknown error</Error>";
            }
            else return "<Error>Expense wasn't deleted, missing data</Error>";
        }

        internal string EditExpense(XDocument requestXML)
        {
            XElement expenseElement = requestXML.Root.Element("Expense");
            if (expenseElement != null && expenseElement.Element("Id") != null && expenseElement.Element("Name") != null && expenseElement.Element("Amount") != null && expenseElement.Element("Type") != null && expenseElement.Element("Place") != null && expenseElement.Element("Source") != null)
            {
                if (DatabaseHandler.Instance().EditExpense(expenseElement.Element("Name").Value, int.Parse(expenseElement.Element("Amount").Value), expenseElement.Element("Type").Value, expenseElement.Element("Place").Value, expenseElement.Element("Source").Value, Id, int.Parse(expenseElement.Element("Id").Value)) == 1)
                {
                    return "<Message>Expense edited successfully</Message>";
                }
                else return "<Error>Expense wasn't edited, unknown error</Error>";
            }
            else return "<Error>Expense wasn't edited, missing data</Error>";
        }

        internal string ListExpenses(XDocument requestXML)
        {
            HashSet<Tuple<int, string, int, string, string, string, string>> data;

            try
            {
                data = DatabaseHandler.Instance().GetUserExpenses(Id);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return "<Error>Server encountered an error during listing expenses. Try again later.</Error>";
            }

            if (data.Count > 0)
            {
                string returnList = "<Expenses>";

                data.ToList().ForEach(data => returnList += "<Expense>" + "<Id>" + data.Item1 + "</Id>" + "<Name>" + data.Item2 + "</Name>" + "<Amount>" + data.Item3 + "</Amount>" + "<Type>" + data.Item4 + "</Type>"+ "<Place>" + data.Item5 + "</Place>" + "<Source>" + data.Item6 + "</Source>" + "<Date>" + data.Item7 + "</Date>" + "</Expense>");

                return returnList + "</Expenses>";
            }
            else
            {
                return "<Message>No Expenses.</Message>";
            }
        }

        internal string CreateIncome(XDocument requestXML)
        {
            XElement incomeElement = requestXML.Root.Element("Income") ?? (requestXML.Root.Name == "Income" ? requestXML.Root : null);
            if (incomeElement != null && incomeElement.Element("Name") != null && incomeElement.Element("Amount") != null && incomeElement.Element("Source") != null && incomeElement.Element("Destination") != null)
            {
                if (incomeElement.Element("Date") != null)
                {
                    if (DatabaseHandler.Instance().AddIncomeWithDate(incomeElement.Element("Name").Value, int.Parse(incomeElement.Element("Amount").Value), incomeElement.Element("Source").Value, incomeElement.Element("Destination").Value, incomeElement.Element("Date").Value, Id) == 1)
                    {
                        return "<Message>Income addded successfully</Message>";
                    }
                    else return "<Error>Income wasn't added, unknown error</Error>";
                }
                else
                {
                    if (DatabaseHandler.Instance().AddIncome(incomeElement.Element("Name").Value, int.Parse(incomeElement.Element("Amount").Value), incomeElement.Element("Source").Value, incomeElement.Element("Destination").Value, Id) == 1)
                    {
                        return "<Message>Income addded successfully</Message>";
                    }
                    else return "<Error>Income wasn't added, unknown error</Error>";
                }
            }
            else return "<Error>Income wasn't added, missing data</Error>";
        }

        internal string SearchIncome(XDocument requestXML)
        {
            if (requestXML.Root.Element("search") != null && requestXML.Root.Element("searchby") != null)
            {
                string search = requestXML.Root.Element("search").Value;
                SearchIncomeBy searchIncomeBy = Enum.Parse<SearchIncomeBy>(requestXML.Root.Element("searchby").Value);

                if (search.Length > 0)
                {
                    HashSet<Tuple<int, string, int, string, string, string>> data;

                    try
                    {
                        data = DatabaseHandler.Instance().SearchIncomes(Id, searchIncomeBy, search);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        return "<Error>Server encountered an error during listing incomes. Try again later.</Error>";
                    }

                    if (data.Count > 0)
                    {
                        string returnList = "<Incomes>";

                        data.ToList().ForEach(data => returnList += "<Income>" + "<Id>" + data.Item1 + "</Id>" + "<Name>" + data.Item2 + "</Name>" + "<Amount>" + data.Item3 + "</Amount>" + "<Source>" + data.Item4 + "</Source>" + "<Destination>" + data.Item5 + "</Destination>" + "<Date>" + data.Item6 + "</Date>" + "</Income>");

                        return returnList + "</Incomes>";
                    }
                    else
                    {
                        return "<Message>No incomes.</Message>";
                    }
                }
                else return "<Error>Empty search string</Error>";
            }
            else return "<Error>Income wasn't searched, missing data</Error>";
        }

        internal string SearchExpenses(XDocument requestXML)
        {
            if (requestXML.Root.Element("search") != null && requestXML.Root.Element("searchby") != null)
            {
                string search = requestXML.Root.Element("search").Value;
                SearchExpenseBy searchExpenseBy = Enum.Parse<SearchExpenseBy>(requestXML.Root.Element("searchby").Value);

                HashSet<Tuple<int, string, int, string, string, string, string>> data;

                if (search.Length > 0)
                {
                    try
                    {
                        data = DatabaseHandler.Instance().SearchExpenses(Id, searchExpenseBy, search);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        return "<Error>Server encountered an error during listing expenses. Try again later.</Error>";
                    }

                    if (data.Count > 0)
                    {
                        string returnList = "<Expenses>";

                        data.ToList().ForEach(data => returnList += "<Expense>" + "<Id>" + data.Item1 + "</Id>" + "<Name>" + data.Item2 + "</Name>" + "<Amount>" + data.Item3 + "</Amount>" + "<Type>" + data.Item4 + "</Type>" + "<Place>" + data.Item5 + "</Place>" + "<Source>" + data.Item6 + "</Source>" + "<Date>" + data.Item7 + "</Date>" + "</Expense>");

                        return returnList + "</Expenses>";
                    }
                    else return "<Message>No Expenses.</Message>";
                }
                else return "<Error>Empty search string</Error>";
            }
            else return "<Error>Expense wasn't searched, missing data</Error>";
        }

        internal string DeleteIncome(XDocument requestXML)
        {
            XElement incomeElement = requestXML.Root.Element("income");
            if (incomeElement != null && incomeElement.Element("id") != null)
            {
                if (DatabaseHandler.Instance().DeleteIncome(int.Parse(incomeElement.Element("id").Value), Id) == 1)
                {
                    return "<Message>Income deleted successfully</Message>";
                }
                else return "<Error>Income wasn't deleted, unknown error</Error>";
            }
            else return "<Error>Income wasn't deleted, missing data</Error>";
        }

        internal string EditIncome(XDocument requestXML)
        {
            XElement incomeElement = requestXML.Root.Element("Income");
            if (incomeElement != null && incomeElement.Element("Id") != null && incomeElement.Element("Name") != null && incomeElement.Element("Amount") != null && incomeElement.Element("Source") != null && incomeElement.Element("Destination") != null)
            {
                if (DatabaseHandler.Instance().EditIncome(incomeElement.Element("Name").Value, int.Parse(incomeElement.Element("Amount").Value), incomeElement.Element("Source").Value, incomeElement.Element("Destination").Value, Id, int.Parse(incomeElement.Element("Id").Value)) == 1)
                {
                    return "<Message>Income edited successfully</Message>";
                }
                else return "<Error>Income wasn't edited, unknown error</Error>";
            }
            else return "<Error>Income wasn't edited, missing data</Error>";
        }

        internal string ListIncome(XDocument requestXML)
        {
            HashSet<Tuple<int, string, int, string, string, string>> data;

            try
            {
                data = DatabaseHandler.Instance().GetUserIncomes(Id);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return "<Error>Server encountered an error during listing incomes. Try again later.</Error>";
            }

            if (data.Count > 0)
            {
                string returnList = "<Incomes>";

                data.ToList().ForEach(data => returnList += "<Income>" + "<Id>" + data.Item1 + "</Id>" + "<Name>" + data.Item2 + "</Name>" + "<Amount>" + data.Item3 + "</Amount>" + "<Source>" + data.Item4 + "</Source>" + "<Destination>" + data.Item5 + "</Destination>" + "<Date>" + data.Item6 + "</Date>" + "</Income>");

                return returnList + "</Incomes>";
            }
            else
            {
                return "<Message>No incomes.</Message>";
            }
        }

        internal string ImportExpenses(XDocument requestXML)
        {
            XElement expensesElement = requestXML.Root.Element("Expenses");
            string response = "";

            if (expensesElement?.Descendants("Expense") != null && expensesElement.Descendants("Expense").Any())
            {
                expensesElement.Descendants("Expense").ToList().ForEach(expense => {
                    XDocument expenseDocument = new XDocument();
                    expenseDocument.Add(expense);
                    response += CreateExpense(expenseDocument);
                });

                return response;
            }
            else return "<Error>Expenses weren't imported, missing data</Error>";
        }

        internal string ImportIncome(XDocument requestXML)
        {
            XElement incomesElement = requestXML.Root.Element("Incomes");
            string response = "";

            if (incomesElement?.Descendants("Income") != null && incomesElement.Descendants("Income").Any())
            {
                incomesElement.Descendants("Income").ToList().ForEach(expense => {
                    XDocument incomeDocument = new XDocument();
                    incomeDocument.Add(expense);
                    response += CreateIncome(incomeDocument);
                });

                return response;
            }
            else return "<Error>Incomes weren't imported, missing data</Error>";
        }
    }
}
