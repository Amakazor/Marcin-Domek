using System;
using System.Collections.Generic;
using System.Text;

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

        public User(string login)
        {
            Dictionary<string, string> userData = DatabaseHandler.Instance().GetUserData(login);

            Id = int.Parse(userData["UserID"]);
            FirstName = userData["FirstName"];
            LastName = userData["LastName"];
            UserType = (UserType)int.Parse(userData["Type"]);
            Login = userData["Login"];
        }

        private User()
        {
            Id = 0;
            FirstName = null;
            LastName = null;
            UserType = UserType.User;
            Login = null;
        }
    }
}
