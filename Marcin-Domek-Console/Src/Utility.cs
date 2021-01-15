using Marcin_Domek_Server.Src.Users;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Marcin_Domek_Console.Src
{
    internal static class Utility
    {
        public static string GetStringFromConsole(string message, string messageIfFailed, uint minLenght = 0, string messageIfLenghtTooShort = null)
        {
            string newString = null;

            do
            {
                if (newString != null && newString.Length == 0)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(messageIfFailed);
                    Console.ResetColor();
                }
                
                if (minLenght > 0 && newString != null && newString.Length < minLenght)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(messageIfLenghtTooShort);
                    Console.ResetColor();
                }

                Console.WriteLine(message);
                newString = Console.ReadLine();

            } while (newString == null || newString.Length == 0 || newString.Length < minLenght);

            return newString;
        }
        
        public static string GetStringFromConsoleOrCancel(string message, string messageIfFailed, uint minLenght = 0, string messageIfLenghtTooShort = null)
        {
            string newString = null;

            do
            {
                if (newString != null && newString.Length == 0)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(messageIfFailed);
                    Console.ResetColor();
                }
                
                if (minLenght > 0 && newString != null && newString.Length < minLenght)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(messageIfLenghtTooShort);
                    Console.ResetColor();
                }

                Console.WriteLine(message);
                newString = Console.ReadLine();

                if (newString == "q") return null;

            } while (newString == null || newString.Length == 0 || newString.Length < minLenght);

            return newString;
        }

        public static UserType GetUserTypeFromConsole(string message)
        {
            UserType newUserType;
            bool failed = false;

            do
            {
                if (failed)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("That's not valid user type");
                    Console.ResetColor();
                }
                Console.WriteLine(message);
                ((int[])Enum.GetValues(typeof(UserType))).ToList().FindAll(i => i != 0).ForEach(i => Console.WriteLine(i + ". " + (UserType)i));

                failed = !int.TryParse(Console.ReadLine(), out int userTypeNumeral);
                newUserType = !failed ? (UserType)userTypeNumeral : UserType.None;
            } while (newUserType == UserType.None || failed);
            return newUserType;
        }

        public static int GetIntInListFromConsoleOrCancel(string message, string messageIfFailed, HashSet<int> validInts)
        {
            bool failed = false;
            int toReturn;

            do
            {
                if (failed)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(messageIfFailed);
                    Console.ResetColor();
                }
                Console.WriteLine(message);

                string input = Console.ReadLine();
                if (input == "q") return -1;

                failed = !int.TryParse(input, out toReturn);

                failed = failed && !validInts.Contains(toReturn);

            } while (failed);

            return toReturn;
        }
        
        public static int GetIntFromConsoleOrCancel(string message, string messageIfFailed)
        {
            bool failed = false;
            int toReturn;

            do
            {
                if (failed)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(messageIfFailed);
                    Console.ResetColor();
                }
                Console.WriteLine(message);

                string input = Console.ReadLine();
                if (input == "q") return -1;

                failed = !int.TryParse(input, out toReturn);

            } while (failed);

            return toReturn;
        }

        public static int GetIntFromConsole(string message, string messageIfFailed)
        {
            bool failed = false;
            int toReturn;

            do
            {
                if (failed)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(messageIfFailed);
                    Console.ResetColor();
                }
                Console.WriteLine(message);

                string input = Console.ReadLine();

                failed = !int.TryParse(input, out toReturn);
            } while (failed);

            return toReturn;
        }

        public static void WriteError(string error)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(error);
            Console.ResetColor();
        }
        
        public static void WriteMessage(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(message);
            Console.ResetColor();
        }
    }
}