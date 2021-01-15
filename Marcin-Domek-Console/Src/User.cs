using Marcin_Domek_Console.Src;

namespace Marcin_Domek_Server.Src.Users
{
    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public UserType UserType { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }

        public User(int id, string firstName, string lastName, UserType userType, string login, string password = null)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            UserType = userType;
            Login = login;
            Password = password;
        }

        public User(User user)
        {
            Id = user.Id;
            FirstName = user.FirstName;
            LastName = user.LastName;
            UserType = user.UserType;
            Login = user.Login;
        }

        public User()
        {
            Id = 0;
            FirstName = null;
            LastName = null;
            UserType = UserType.User;
            Login = null;
        }

        public static User CreateUser()
        {
            string firstName = Utility.GetStringFromConsole("Enter users first name", "Name can't be empty");
            string lastName = Utility.GetStringFromConsole("Enter users last name", "Name can't be empty");
            string login = Utility.GetStringFromConsole("Enter users login name", "Login can't be empty", 8, "Login must be at least 8 characters long");
            string password = Utility.GetStringFromConsole("Enter users password name", "Password can't be empty", 8, "Password must be at least 8 characters long");
            UserType userType = Utility.GetUserTypeFromConsole("Select valid user type");

            return new User(0, firstName, lastName, userType, login, password);
        }
    }
}
