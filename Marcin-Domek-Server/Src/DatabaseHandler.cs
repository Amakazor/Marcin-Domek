using Marcin_Domek_Server.Src.Requests;
using Marcin_Domek_Server.Src.Users;
using Marcin_Domek_Server.Src.Users.Password;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Marcin_Domek_Server.Src
{
    internal class DatabaseHandler : IDisposable
    {
        private MySqlConnection Connection;
        private string Server;
        private string Database;
        private string Uid;
        private string Password;

        private static DatabaseHandler instance;

        private DatabaseHandler()
        {
            Server = "localhost";
            Database = "domek";
            Uid = "csharp";
            Password = "gCkdGin3YlayW0CK";
            string connectionString = "SERVER=" + Server + ";" + "DATABASE=" + Database + ";" + "UID=" + Uid + ";" + "PASSWORD=" + Password + ";";

            Connection = new MySqlConnection(connectionString);
        }

        public static DatabaseHandler Instance()
        {
            if (instance == null)
            {
                instance = new DatabaseHandler();
            }

            return instance;
        }

        public bool Open()
        {
            try
            {
                Connection.Open();
                return true;
            }
            catch (MySqlException exception)
            {
                switch (exception.Number)
                {
                    case 0:
                        Console.WriteLine("Cannot connect to the server");
                        break;
                    case 1045:
                        Console.WriteLine("Invalid username/password");
                        break;
                }
                return false;
            }
        }

        private bool Close()
        {
            try
            {
                Connection.Close();
                return true;
            }
            catch (MySqlException exception)
            {
                Console.WriteLine(exception.Message);
                return false;
            }
        }

        public int AddUser(string firstName, string lastName, UserType userType, string login, string password)
        {
            if (!(firstName is null) && !(lastName is null) && !(login is null) && !(password is null))
            {
                string query = "INSERT INTO user (`FirstName`, `LastName`, `Type`, `Login`, `Password`) VALUES (@fname, @lname, @type, @login, @password)";

                password = PasswordHasher.Hash(password);

                MySqlCommand command = new MySqlCommand(query, Connection);
                command.Parameters.AddWithValue("@fname", firstName);
                command.Parameters.AddWithValue("@lname", lastName);
                command.Parameters.AddWithValue("@type", (int)userType);
                command.Parameters.AddWithValue("@login", login);
                command.Parameters.AddWithValue("@password", password);

                return command.ExecuteNonQuery();
            }
            return -1;
        }
        
        public int AddExpense(string name, int amount, string type, string place, string source, int userID)
        {
            if (!(name is null) && !(type is null) && !(place is null) && !(source is null) && amount > 0 && userID != 0 && UserExists(userID) == 1)
            {
                string query = "INSERT INTO expense (`Name`, `Amount`, `Type`, `Place`, `Source`, `USerID`) VALUES (@name, @amount, @type, @place, @source, @userID)";

                MySqlCommand command = new MySqlCommand(query, Connection);
                command.Parameters.AddWithValue("@name", name);
                command.Parameters.AddWithValue("@amount", amount);
                command.Parameters.AddWithValue("@type", type);
                command.Parameters.AddWithValue("@place", place);
                command.Parameters.AddWithValue("@source", source);
                command.Parameters.AddWithValue("@userID", userID);

                return command.ExecuteNonQuery();
            }
            return -1;
        }
        
        public int AddIncome(string name, int amount, string source, string destination, int userID)
        {
            if (!(name is null) && !(source is null) && !(destination is null) && amount > 0 && userID != 0 && UserExists(userID) == 1)
            {
                string query = "INSERT INTO income (`Name`, `Amount`, `source`, `destination`, `USerID`) VALUES (@name, @amount, @source, @destination, @userID)";

                MySqlCommand command = new MySqlCommand(query, Connection);
                command.Parameters.AddWithValue("@name", name);
                command.Parameters.AddWithValue("@amount", amount);
                command.Parameters.AddWithValue("@source", source);
                command.Parameters.AddWithValue("@destination", destination);
                command.Parameters.AddWithValue("@userID", userID);

                return command.ExecuteNonQuery();
            }
            return -1;
        }

        internal int AddExpenseWithDate(string name, int amount, string type, string place, string source, string date, int userID)
        {
            if (!(name is null) && !(type is null) && !(place is null) && !(source is null) && amount > 0 && date != null && userID != 0 && UserExists(userID) == 1)
            {
                string query = "INSERT INTO expense (`Name`, `Amount`, `Type`, `Place`, `Source`, `date`, `USerID`) VALUES (@name, @amount, @type, @place, @source, STR_TO_DATE(@date, '%d.%m.%Y %H:%i:%s'), @userID)";

                MySqlCommand command = new MySqlCommand(query, Connection);
                command.Parameters.AddWithValue("@name", name);
                command.Parameters.AddWithValue("@amount", amount);
                command.Parameters.AddWithValue("@type", type);
                command.Parameters.AddWithValue("@place", place);
                command.Parameters.AddWithValue("@source", source);
                command.Parameters.AddWithValue("@date", date);
                command.Parameters.AddWithValue("@userID", userID);

                return command.ExecuteNonQuery();
            }
            return -1;
        }

        internal int AddIncomeWithDate(string name, int amount, string source, string destination, string date, int userID)
        {
            if (!(name is null) && !(source is null) && !(destination is null) && amount > 0 && userID != 0 && date != null && UserExists(userID) == 1)
            {
                string query = "INSERT INTO income (`Name`, `Amount`, `source`, `destination`, `date`, `USerID`) VALUES (@name, @amount, @source, @destination, STR_TO_DATE(@date, '%d.%m.%Y %H:%i:%s'), @userID)";

                MySqlCommand command = new MySqlCommand(query, Connection);
                command.Parameters.AddWithValue("@name", name);
                command.Parameters.AddWithValue("@amount", amount);
                command.Parameters.AddWithValue("@source", source);
                command.Parameters.AddWithValue("@destination", destination);
                command.Parameters.AddWithValue("@date", date);
                command.Parameters.AddWithValue("@userID", userID);

                return command.ExecuteNonQuery();
            }
            return -1;
        }

        public int EditExpense(string name, int amount, string type, string place, string source, int userID, int id)
        {
            if (ExpenseExists(id) == 1 && UserExists(userID) == 1 && !(name is null) && amount > 0 && !(type is null) && !(place is null) && !(source is null))
            {
                string query = "UPDATE expense SET `Name` = @name, `Amount` = @amount, `Type` = @type, `Place` = @place, `Source` = @source WHERE `UserID` = @userID";

                MySqlCommand command = new MySqlCommand(query, Connection);
                command.Parameters.AddWithValue("@name", name);
                command.Parameters.AddWithValue("@amount", amount);
                command.Parameters.AddWithValue("@type", type);
                command.Parameters.AddWithValue("@place", place);
                command.Parameters.AddWithValue("@source", source);
                command.Parameters.AddWithValue("@userID", userID);

                return command.ExecuteNonQuery();
            }
            else return -1;
        }
        
        public int EditIncome(string name, int amount, string source, string destination, int userID, int id)
        {
            if (IncomeExists(id) == 1 && UserExists(userID) == 1 && !(name is null) && amount > 0 && !(source is null) && !(destination is null))
            {
                string query = "UPDATE income SET `Name` = @name, `Amount` = @amount, `source` = @source, `destination` = @destination WHERE `UserID` = @userID";

                MySqlCommand command = new MySqlCommand(query, Connection);
                command.Parameters.AddWithValue("@name", name);
                command.Parameters.AddWithValue("@amount", amount);
                command.Parameters.AddWithValue("@source", source);
                command.Parameters.AddWithValue("@destination", destination);
                command.Parameters.AddWithValue("@userID", userID);

                return command.ExecuteNonQuery();
            }
            return -1;
        }
        
        public int AddTicket(int ticketerID, string message)
        {
            if (ticketerID != 0 && UserExists(ticketerID) != -1 && message != null && message.Length > 0)
            {
                string query = "INSERT INTO ticket (`TicketerID`, `Message`) VALUES (@ticketerID, @message)";

                MySqlCommand command = new MySqlCommand(query, Connection);
                command.Parameters.AddWithValue("@ticketerID", ticketerID);
                command.Parameters.AddWithValue("@message", message);

                return command.ExecuteNonQuery();
            }
            return -1;
        }

        public int UserExists(int id)
        {
            if (id > 0)
            {
                string query = "SELECT `UserID` FROM user WHERE `UserID` = @id";
                MySqlCommand command = new MySqlCommand(query, Connection);
                command.Parameters.AddWithValue("@id", id);

                return (command.ExecuteScalar() is null) ? 0 : 1;
            }
            else
            {
                return -1;
            }
        }
        
        public int TicketExists(int id)
        {
            if (id > 0)
            {
                string query = "SELECT `TicketID` FROM ticket WHERE `TicketID` = @id";
                MySqlCommand command = new MySqlCommand(query, Connection);
                command.Parameters.AddWithValue("@id", id);

                return (command.ExecuteScalar() is null) ? 0 : 1;
            }
            else
            {
                return -1;
            }
        }

        public int IncomeExists(int id)
        {
            if (id > 0)
            {
                string query = "SELECT `IncomeID` FROM income WHERE `IncomeID` = @id";
                MySqlCommand command = new MySqlCommand(query, Connection);
                command.Parameters.AddWithValue("@id", id);

                return (command.ExecuteScalar() is null) ? 0 : 1;
            }
            else
            {
                return -1;
            }
        }
        
        public int ExpenseExists(int id)
        {
            if (id > 0)
            {
                string query = "SELECT `ExpenseID` FROM expense WHERE `ExpenseID` = @id";
                MySqlCommand command = new MySqlCommand(query, Connection);
                command.Parameters.AddWithValue("@id", id);

                return (command.ExecuteScalar() is null) ? 0 : 1;
            }
            else
            {
                return -1;
            }
        }

        public int UserExists(string login)
        {
            if (!(login is null))
            {
                string query = "SELECT `UserID` FROM user WHERE `Login` = @login";
                MySqlCommand command = new MySqlCommand(query, Connection);
                command.Parameters.AddWithValue("@login", login);

                return (command.ExecuteScalar() is null) ? 0 : 1;
            }
            else
            {
                return -1;
            }
        }

        public int DeleteUser(int id)
        {
            if (UserExists(id) == 1)
            {
                string query = "DELETE FROM user WHERE `UserID` = @id";
                MySqlCommand command = new MySqlCommand(query, Connection);
                command.Parameters.AddWithValue("@id", id);

                return command.ExecuteNonQuery();
            }
            else
            {
                return -1;
            }
        }
        
        public int DeleteExpense(int id, int userID)
        {
            if (UserExists(userID) == 1 && ExpenseExists(id) == 1)
            {
                string query = "DELETE FROM expense WHERE `UserID` = @userID AND `ExpenseID` = @id";
                MySqlCommand command = new MySqlCommand(query, Connection);
                command.Parameters.AddWithValue("@userID", userID);
                command.Parameters.AddWithValue("@id", id);

                return command.ExecuteNonQuery();
            }
            else
            {
                return -1;
            }
        }
        
        public int ExpenseSum(int userID)
        {
            if (UserExists(userID) == 1)
            {
                string query = "SELECT SUM(`Amount`) AS `Amount` FROM expense WHERE `UserID` = @userID";
                MySqlCommand command = new MySqlCommand(query, Connection);
                command.Parameters.AddWithValue("@userID", userID);

                try
                {
                    return int.Parse(command.ExecuteScalar().ToString());
                }
                catch (Exception)
                {

                    return 0;
                }
            }
            else
            {
                return -1;
            }
        }
        
        public int IncomeSum(int userID)
        {
            if (UserExists(userID) == 1)
            {
                string query = "SELECT SUM(`Amount`) AS `Amount` FROM income WHERE `UserID` = @userID";
                MySqlCommand command = new MySqlCommand(query, Connection);
                command.Parameters.AddWithValue("@userID", userID);

                try
                {
                    return int.Parse(command.ExecuteScalar().ToString());
                }
                catch (Exception)
                {

                    return 0;
                }
            }
            else
            {
                return -1;
            }
        }
        
        public int DeleteIncome(int id, int userID)
        {
            if (UserExists(userID) == 1 && IncomeExists(id) == 1)
            {
                string query = "DELETE FROM income WHERE `UserID` = @userID AND `IncomeID` = @id";
                MySqlCommand command = new MySqlCommand(query, Connection);
                command.Parameters.AddWithValue("@userID", userID);
                command.Parameters.AddWithValue("@id", id);

                return command.ExecuteNonQuery();
            }
            else
            {
                return -1;
            }
        }

        public int UpdateUserPassword(int id, string password)
        {
            if (UserExists(id) == 1 && !(password is null))
            {
                password = PasswordHasher.Hash(password);

                string query = "UPDATE user SET `Password` = @password WHERE `UserID` = @id";
                MySqlCommand command = new MySqlCommand(query, Connection);
                command.Parameters.AddWithValue("@id", id);
                command.Parameters.AddWithValue("@password", password);

                return command.ExecuteNonQuery();
            }
            else
            {
                return -1;
            }
        }
        
        public int ChangeUserType(int id, UserType type)
        {
            if (UserExists(id) == 1)
            {
                string query = "UPDATE user SET `Type` = @type WHERE `UserID` = @id";
                MySqlCommand command = new MySqlCommand(query, Connection);
                command.Parameters.AddWithValue("@id", id);
                command.Parameters.AddWithValue("@type", (int)type);

                return command.ExecuteNonQuery();
            }
            else
            {
                return -1;
            }
        }

        public int ChangeTicketStatus(int id, TicketStatus ticketStatus)
        {
            if (TicketExists(id) == 1)
            {
                string query = "UPDATE ticket SET `Status` = @status WHERE `TicketID` = @id";
                MySqlCommand command = new MySqlCommand(query, Connection);
                command.Parameters.AddWithValue("@id", id);
                command.Parameters.AddWithValue("@status", ticketStatus.ToString());

                return command.ExecuteNonQuery();
            }
            else
            {
                return -1;
            }
        }
        
        public int ChangeTicketHelpdesk(int id, int helpdeskId)
        {
            if (TicketExists(id) == 1)
            {
                string query = "UPDATE ticket SET `HelpdeskID` = @helpdeskId WHERE `TicketID` = @id";
                MySqlCommand command = new MySqlCommand(query, Connection);
                command.Parameters.AddWithValue("@id", id);
                command.Parameters.AddWithValue("@helpdeskId", helpdeskId);

                return command.ExecuteNonQuery();
            }
            else
            {
                return -1;
            }
        }

        public int NullTicketHelpdesk(int id)
        {
            if (TicketExists(id) == 1)
            {
                string query = "UPDATE ticket SET `HelpdeskID` = null WHERE `TicketID` = @id";
                MySqlCommand command = new MySqlCommand(query, Connection);
                command.Parameters.AddWithValue("@id", id);

                return command.ExecuteNonQuery();
            }
            else
            {
                return -1;
            }
        }

        public int CheckUserPassword(string login, string password)
        {
            if (UserExists(login) == 1 && !(password is null))
            {
                string query = "SELECT `password` FROM user WHERE `Login` = @login";
                MySqlCommand command = new MySqlCommand(query, Connection);
                command.Parameters.AddWithValue("@login", login);

                string hash = command.ExecuteScalar().ToString();

                if (hash is null || !PasswordHasher.IsHashSupported(hash) || !PasswordHasher.Verify(password, hash)) return 0;
                return 1;
            }
            else
            {
                return -1;
            }
        }

        public int GetUserIdFromLogin(string login)
        {
            if (UserExists(login) == 1)
            {
                string query = "SELECT `UserID` FROM user WHERE `Login` = @login";
                MySqlCommand command = new MySqlCommand(query, Connection);
                command.Parameters.AddWithValue("@login", login);

                return int.Parse(command.ExecuteScalar().ToString());

            }
            else
            {
                return -1;
            }
        }

        public UserType GetUserType(int id)
        {
            if (UserExists(id) == 1)
            {
                string query = "SELECT `Type` FROM user WHERE `UserID` = @id";
                MySqlCommand command = new MySqlCommand(query, Connection);
                command.Parameters.AddWithValue("@id", id);

                return (UserType)int.Parse(command.ExecuteScalar().ToString());
            }
            else
            {
                throw new Exception();
            }
        }

        public UserType GetUserType(string login)
        {
            if (UserExists(login) == 1)
            {
                return GetUserType(GetUserIdFromLogin(login));
            }
            else
            {
                throw new Exception();
            }
        }

        public Dictionary<string, string> GetUserData(int id)
        {
            if (id > 0 && UserExists(id) > 0)
            {
                string query = "SELECT `UserID`, `FirstName`, `LastName`, `Type`, `Login` FROM user WHERE `UserID` = @id";
                MySqlCommand command = new MySqlCommand(query, Connection);
                command.Parameters.AddWithValue("@id", id);

                MySqlDataReader dataReader = command.ExecuteReader();

                if (dataReader.HasRows)
                {
                    dataReader.Read();
                    Dictionary<string, string> userData = Enumerable.Range(0, dataReader.FieldCount).ToDictionary(dataReader.GetName, dataReader.GetString);
                    dataReader.Close();
                    return userData;
                }
                else
                {
                    throw new Exception();
                }
            }
            else
            {
                throw new Exception();
            }
        }

        public Dictionary<string, string> GetUserData(string login)
        {
            int id = GetUserIdFromLogin(login);

            if (id > 0)
            {
                return GetUserData(id);
            }
            else
            {
                throw new Exception();
            }
        }

        public HashSet<int> GetAllUserIds()
        {
            string query = "SELECT `UserID` FROM user";
            MySqlCommand command = new MySqlCommand(query, Connection);

            MySqlDataReader dataReader = command.ExecuteReader();

            if (dataReader.HasRows)
            {
                HashSet<int> ids = new HashSet<int>();

                while (dataReader.Read())
                {
                    ids.Add(dataReader.GetInt32(0));
                }

                dataReader.Close();
                return ids;
            }
            else
            {
                throw new Exception();
            }
        }

        public HashSet<Tuple<int, string, string, string>> GetUserTickets(int id)
        {
            string query = "SELECT `TicketID`, `Message`, `Date`, `Status` FROM ticket WHERE `TicketerID` = @id";
            MySqlCommand command = new MySqlCommand(query, Connection);
            command.Parameters.AddWithValue("@id", id);

            MySqlDataReader dataReader = command.ExecuteReader();

            if (dataReader.HasRows)
            {
                HashSet<Tuple<int, string, string, string>> data = new HashSet<Tuple<int, string, string, string>>();

                while (dataReader.Read())
                {
                    data.Add(new Tuple<int, string, string, string>(dataReader.GetInt32(0), dataReader.GetString(1), dataReader.GetString(2), dataReader.GetString(3)));
                }

                dataReader.Close();
                return data;
            }
            else
            {
                dataReader.Close();
                return new HashSet<Tuple<int, string, string, string>>();
            }
        }
        
        public HashSet<Tuple<int, string, string, string>> GetHelpdeskTickets(int id)
        {
            string query = "SELECT `TicketID`, `Message`, `Date`, `Status` FROM ticket WHERE `HelpdeskID` = @id";
            MySqlCommand command = new MySqlCommand(query, Connection);
            command.Parameters.AddWithValue("@id", id);

            MySqlDataReader dataReader = command.ExecuteReader();

            if (dataReader.HasRows)
            {
                HashSet<Tuple<int, string, string, string>> data = new HashSet<Tuple<int, string, string, string>>();

                while (dataReader.Read())
                {
                    data.Add(new Tuple<int, string, string, string>(dataReader.GetInt32(0), dataReader.GetString(1), dataReader.GetString(2), dataReader.GetString(3)));
                }

                dataReader.Close();
                return data;
            }
            else
            {
                dataReader.Close();
                return new HashSet<Tuple<int, string, string, string>>();
            }
        }

        public HashSet<Tuple<int, string, string, string>> GetNotTakenTickets()
        {
            string query = "SELECT `TicketID`, `Message`, `Date`, `Status` FROM ticket WHERE `HelpdeskID` is null";
            MySqlCommand command = new MySqlCommand(query, Connection);

            MySqlDataReader dataReader = command.ExecuteReader();

            if (dataReader.HasRows)
            {
                HashSet<Tuple<int, string, string, string>> data = new HashSet<Tuple<int, string, string, string>>();

                while (dataReader.Read())
                {
                    data.Add(new Tuple<int, string, string, string>(dataReader.GetInt32(0), dataReader.GetString(1), dataReader.GetString(2), dataReader.GetString(3)));
                }

                dataReader.Close();
                return data;
            }
            else
            {
                dataReader.Close();
                return new HashSet<Tuple<int, string, string, string>>();
            }
        }

        public HashSet<Tuple<int, string, string, string>> GetNotTakenAndTakenByHelpdeskId(int id)
        {
            List<Tuple<int, string, string, string>> toReturn = GetNotTakenTickets().ToList();
            toReturn.AddRange(GetHelpdeskTickets(id));

            return toReturn.ToHashSet();
        }
        public HashSet<Tuple<int, string, int, string, string, string, string>> GetUserExpenses(int id)
        {
            string query = "SELECT `ExpenseID`, `Name`, `Amount`, `Type`, `Place`, `Source`, `Date` FROM expense WHERE `UserID` = @id";
            MySqlCommand command = new MySqlCommand(query, Connection);
            command.Parameters.AddWithValue("@id", id);

            MySqlDataReader dataReader = command.ExecuteReader();

            if (dataReader.HasRows)
            {
                HashSet<Tuple<int, string, int, string, string, string, string>> data = new HashSet<Tuple<int, string, int, string, string, string, string>>();

                while (dataReader.Read())
                {
                    data.Add(new Tuple<int, string, int, string, string, string, string>(dataReader.GetInt32(0), dataReader.GetString(1), dataReader.GetInt32(2), dataReader.GetString(3), dataReader.GetString(4), dataReader.GetString(5), dataReader.GetString(6)));
                }

                dataReader.Close();
                return data;
            }
            else
            {
                dataReader.Close();
                return new HashSet<Tuple<int, string, int, string, string, string, string>>();
            }
        }
        
        public HashSet<Tuple<int, string, int, string, string, string, string>> SearchExpenses(int id, SearchExpenseBy by, string search)
        {
            string query = "SELECT `ExpenseID`, `Name`, `Amount`, `Type`, `Place`, `Source`, `Date` FROM expense WHERE `UserID` = @id AND ";

            query += by switch
            {
                SearchExpenseBy.Name => "`Name`",
                SearchExpenseBy.Place => "`Place`",
                SearchExpenseBy.Source => "`Source`",
                SearchExpenseBy.Type => "`Type`",
            };

            query += " LIKE @search";

            MySqlCommand command = new MySqlCommand(query, Connection);
            command.Parameters.AddWithValue("@id", id);
            command.Parameters.AddWithValue("@search", "%" + search + "%");

            MySqlDataReader dataReader = command.ExecuteReader();

            if (dataReader.HasRows)
            {
                HashSet<Tuple<int, string, int, string, string, string, string>> data = new HashSet<Tuple<int, string, int, string, string, string, string>>();

                while (dataReader.Read())
                {
                    data.Add(new Tuple<int, string, int, string, string, string, string>(dataReader.GetInt32(0), dataReader.GetString(1), dataReader.GetInt32(2), dataReader.GetString(3), dataReader.GetString(4), dataReader.GetString(5), dataReader.GetString(6)));
                }

                dataReader.Close();
                return data;
            }
            else
            {
                dataReader.Close();
                return new HashSet<Tuple<int, string, int, string, string, string, string>>();
            }
        }

        public HashSet<Tuple<int, string, int, string, string, string>> SearchIncomes(int id, SearchIncomeBy by, string search)
        {
            string query = "SELECT `IncomeID`, `Name`, `Amount`, `Source`, `Destination`, `Date` FROM income WHERE `UserID` = @id AND ";

            query += by switch
            {
                SearchIncomeBy.Name => "`Name`",
                SearchIncomeBy.Source => "`Source`",
                SearchIncomeBy.Destination => "`Destination`",
            };

            query += " LIKE @search";

            MySqlCommand command = new MySqlCommand(query, Connection);
            command.Parameters.AddWithValue("@id", id);
            command.Parameters.AddWithValue("@search", "%" + search + "%");

            MySqlDataReader dataReader = command.ExecuteReader();

            if (dataReader.HasRows)
            {
                HashSet<Tuple<int, string, int, string, string, string>> data = new HashSet<Tuple<int, string, int, string, string, string>>();

                while (dataReader.Read())
                {
                    data.Add(new Tuple<int, string, int, string, string, string>(dataReader.GetInt32(0), dataReader.GetString(1), dataReader.GetInt32(2), dataReader.GetString(3), dataReader.GetString(4), dataReader.GetString(5)));
                }

                dataReader.Close();
                return data;
            }
            else
            {
                dataReader.Close();
                return new HashSet<Tuple<int, string, int, string, string, string>>();
            }
        }

        public HashSet<Tuple<int, string, int, string, string, string>> GetUserIncomes(int id)
        {
            string query = "SELECT `IncomeID`, `Name`, `Amount`, `Source`, `Destination`, `Date` FROM income WHERE `UserID` = @id";
            MySqlCommand command = new MySqlCommand(query, Connection);
            command.Parameters.AddWithValue("@id", id);

            MySqlDataReader dataReader = command.ExecuteReader();

            if (dataReader.HasRows)
            {
                HashSet<Tuple<int, string, int, string, string, string>> data = new HashSet<Tuple<int, string, int, string, string, string>>();

                while (dataReader.Read())
                {
                    data.Add(new Tuple<int, string, int, string, string, string>(dataReader.GetInt32(0), dataReader.GetString(1), dataReader.GetInt32(2), dataReader.GetString(3), dataReader.GetString(4), dataReader.GetString(5)));
                }

                dataReader.Close();
                return data;
            }
            else
            {
                dataReader.Close();
                return new HashSet<Tuple<int, string, int, string, string, string>>();
            }
        }

        public void Dispose()
        {
            Close();
            instance = null;
        }
    }
}