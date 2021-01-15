namespace Marcin_Domek_Console.Src
{
    public class Expense
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Amount { get; set; }
        public string Type { get; set; }
        public string Place { get; set; }
        public string Source { get; set; }
        public string Date { get; set; }

        public Expense(int id, string name, int amount, string type, string place, string source)
        {
            Id = id;
            Name = name;
            Amount = amount;
            Type = type;
            Place = place;
            Source = source;
        }

        public Expense()
        {
        }

        public static Expense CreateExpense()
        {
            string name = Utility.GetStringFromConsole("Enter expense name", "Expense name is empty", 4, "Expense name is too short");
            int amount = Utility.GetIntFromConsole("Enter expense amount", "Expense amount is invalid");
            string type = Utility.GetStringFromConsole("Enter expense type", "Expense type is empty", 4, "Expense type is too short");
            string place = Utility.GetStringFromConsole("Enter expense place", "Expense place is place", 4, "Expense place is too short");
            string source = Utility.GetStringFromConsole("Enter expense source", "Expense source is empty", 4, "Expense source is too short");

            return new Expense(0, name, amount, type, place, source);
        }

        internal void Edit()
        {
            string name = Utility.GetStringFromConsoleOrCancel("Enter expense name or q to cancel", "Expense name is empty", 4, "Expense name is too short");
            int amount = Utility.GetIntFromConsoleOrCancel("Enter expense amount or q to cancel", "Expense amount is invalid");
            string type = Utility.GetStringFromConsoleOrCancel("Enter expense type or q to cancel", "Expense type is empty", 4, "Expense type is too short");
            string place = Utility.GetStringFromConsoleOrCancel("Enter expense place or q to cancel", "Expense place is place", 4, "Expense place is too short");
            string source = Utility.GetStringFromConsoleOrCancel("Enter expense source or q to cancel", "Expense source is empty", 4, "Expense source is too short");

            Name = name ?? Name;
            Type = type ?? Type;
            Place = place ?? Place;
            Source = source ?? Source;
            Amount = amount != -1 ? amount : Amount;
        }
    }
}