using Marcin_Domek_Server.Src;
using System;
using System.Collections.Generic;

namespace Marcin_Domek_Server
{
    class Program
    {
        static void Main(string[] args)
        {
            DatabaseHandler databaseConnection = DatabaseHandler.Instance();
            databaseConnection.Open();

            DomekServer domekServer = new DomekServer();
            domekServer.Loop();

            databaseConnection.Dispose();
        }
    }
}
