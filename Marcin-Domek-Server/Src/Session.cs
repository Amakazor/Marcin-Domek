using Marcin_Domek_Server.Src.Users;
using System;

namespace Marcin_Domek_Server.Src
{
    internal struct Session
    {
        public Guid Sessionid { get; set; }
        public User User { get; set; }
        public DateTime LastUpdate { get; set; }

        public Session(Guid sessionid, User user, DateTime lastUpdate)
        {
            Sessionid = sessionid;
            User = user;
            LastUpdate = lastUpdate;
        }
    }
}