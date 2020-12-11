using Marcin_Domek_Server.Src.Users;
using Marcin_Domek_Server.Src.Users.Password;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;

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

        public int AddUser(string firstName, string lastName, string login, string password)
        {
            if (!(firstName is null) && !(lastName is null) && !(login is null) && !(password is null))
            {
                string query = "INSERT INTO user (`FirstName`, `LastName`, `Login`, `Password`) VALUES (@fname, @lname, @login, @password)";

                password = PasswordHasher.Hash(password);

                MySqlCommand command = new MySqlCommand(query, Connection);
                command.Parameters.AddWithValue("@fname", firstName);
                command.Parameters.AddWithValue("@lname", lastName);
                command.Parameters.AddWithValue("@login", login);
                command.Parameters.AddWithValue("@password", password);

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
                    return Enumerable.Range(0, dataReader.FieldCount).ToDictionary(dataReader.GetName, dataReader.GetString);
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

        public void Dispose()
        {
            Close();
            instance = null;
        }
    }
}