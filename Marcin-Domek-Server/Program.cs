using Marcin_Domek_Server.Src;
using System;

namespace Marcin_Domek_Server
{
    class Program
    {
        static void Main(string[] args)
        {
            DomekServer domekServer = new DomekServer();
            domekServer.Loop();
        }
    }
}
