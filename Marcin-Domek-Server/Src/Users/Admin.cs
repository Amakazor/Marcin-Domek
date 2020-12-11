﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Marcin_Domek_Server.Src.Users
{
    public class Admin : User
    {
        public Admin(string login) : base(login)
        {

        }

        public Admin(int id, string firstName, string lastName, UserType userType, string login) : base(id, firstName, lastName, userType, login)
        {

        }
    }
}
