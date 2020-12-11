using System;
using System.Collections.Generic;
using System.Text;

namespace Marcin_Domek_Server.Src.Users
{
    class Helpdesk : User
    {
        public Helpdesk(string login) : base(login)
        {
        }

        public Helpdesk(int id, string firstName, string lastName, UserType userType, string login) : base(id, firstName, lastName, userType, login)
        {
        }
    }
}
