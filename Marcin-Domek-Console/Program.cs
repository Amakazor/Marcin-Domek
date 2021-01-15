using Marcin_Domek.Src;
using Marcin_Domek_Console.Src;

namespace Marcin_Domek_Console
{
    internal static class Program
    {
        static void Main(string[] args)
        {
            Client client = new Client();

            Menu menu = new Menu(client);
            menu.Loop();
        }
    }
}
