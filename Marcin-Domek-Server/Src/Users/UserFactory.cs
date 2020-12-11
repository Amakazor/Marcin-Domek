using System;
using System.Collections.Generic;
using System.Text;

namespace Marcin_Domek_Server.Src.Users
{
    internal static class UserFactory
    {
        public static User CreateUser(UserType userType, string login)
        {
            return userType switch
            {
                UserType.Admin => new Admin(login),
                UserType.Helpdesk => new Helpdesk(login),
                _ => new User(login),
            };
        }

        public static User CreateUser(string login)
        {
            return CreateUser(DatabaseHandler.Instance().GetUserType(login), login);
        }
    }
}
