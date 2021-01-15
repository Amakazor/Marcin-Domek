namespace Marcin_Domek_Console.Src
{
    public class Income
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Amount { get; set; }
        public string Source { get; set; }
        public string Destination { get; set; }
        public string Date { get; set; }

        public Income(int id, string name, int amount, string source, string destination)
        {
            Id = id;
            Name = name;
            Amount = amount;
            Source = source;
            Destination = destination;
        }

        public Income()
        {
        }

        public static Income CreateIncome()
        {
            string name = Utility.GetStringFromConsole("Enter income name", "Income name is empty", 4, "Income name is too short");
            int amount = Utility.GetIntFromConsole("Enter income amount", "Income amount is invalid");
            string source = Utility.GetStringFromConsole("Enter income source", "Income source is empty", 4, "Income source is too short");
            string destination = Utility.GetStringFromConsole("Enter income destination", "Income destination is empty", 4, "Income destination is too short");

            return new Income(0, name, amount, source, destination);
        }

        internal void Edit()
        {
            string name = Utility.GetStringFromConsoleOrCancel("Enter income name or q to cancel", "Income name is empty", 4, "Income name is too short");
            int amount = Utility.GetIntFromConsoleOrCancel("Enter income amount or q to cancel", "Income amount is invalid");
            string source = Utility.GetStringFromConsoleOrCancel("Enter income source or q to cancel", "Income source is empty", 4, "Income source is too short");
            string destination = Utility.GetStringFromConsoleOrCancel("Enter income destination or q to cancel", "Income destination is empty", 4, "Income destination is too short");

            Name = name ?? Name;
            Source = source ?? Source;
            Destination = destination ?? Destination;
            Amount = amount != -1 ? amount : Amount;
        }
    }
}