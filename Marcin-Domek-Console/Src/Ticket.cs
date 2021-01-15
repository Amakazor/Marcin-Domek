namespace Marcin_Domek_Console.Src
{
    public class Ticket
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public string Date { get; set; }
        public TicketStatus Status { get; set; }

        public Ticket(int id, string message, string date, TicketStatus status)
        {
            Id = id;
            Message = message;
            Date = date;
            Status = status;
        }

        public Ticket()
        {

        }

        public static Ticket CreateTicket()
        {
            string message = Utility.GetStringFromConsole("Enter your ticket message", "Message is empty", 10, "Message is too short");
            return new Ticket(0, message, null, TicketStatus.NotTaken);
        }
    }
}
