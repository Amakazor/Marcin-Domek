using Marcin_Domek_Server.Src.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Marcin_Domek_Server.Src.Users
{
    internal class Helpdesk : User
    {
        public Helpdesk(string login) : base(login)
        {

        }

        public Helpdesk(int id, string firstName, string lastName, UserType userType, string login) : base(id, firstName, lastName, userType, login)
        {

        }

        protected Helpdesk() : base()
        {

        }

        internal string ApplyTicketToSelf(XDocument requestXML)
        {
            if (requestXML.Root.Element("ticket") != null && int.Parse(requestXML.Root.Element("ticket").Value) > 0)
            {
                if (DatabaseHandler.Instance().TicketExists(int.Parse(requestXML.Root.Element("ticket").Value)) != -1 && DatabaseHandler.Instance().ChangeTicketHelpdesk(int.Parse(requestXML.Root.Element("ticket").Value), Id) != -1 && DatabaseHandler.Instance().ChangeTicketStatus(int.Parse(requestXML.Root.Element("ticket").Value), TicketStatus.Taken) != -1)
                {
                    return "<Message>Ticket applied successfully</Message>";
                }
                else
                {
                    return "<Error>Unknown error during ticket ownership change. Try again later.</Error>";
                }
            }
            else
            {
                return "<Error>Missing data.</Error>";
            }
        }

        internal string ReleaseTicket(XDocument requestXML)
        {
            if (requestXML.Root.Element("ticket") != null && int.Parse(requestXML.Root.Element("ticket").Value) > 0)
            {
                if (DatabaseHandler.Instance().TicketExists(int.Parse(requestXML.Root.Element("ticket").Value)) != -1 && DatabaseHandler.Instance().NullTicketHelpdesk(int.Parse(requestXML.Root.Element("ticket").Value)) != -1 && DatabaseHandler.Instance().ChangeTicketStatus(int.Parse(requestXML.Root.Element("ticket").Value), TicketStatus.NotTaken) != -1)
                {
                    return "<Message>Ticket rejected successfully</Message>";
                }
                else
                {
                    return "<Error>Unknown error during ticket rejection. Try again later.</Error>";
                }
            }
            else
            {
                return "<Error>Missing data.</Error>";
            }
        }

        internal string RejectTicket(XDocument requestXML)
        {
            if (requestXML.Root.Element("ticket") != null && int.Parse(requestXML.Root.Element("ticket").Value) > 0)
            {
                if (DatabaseHandler.Instance().TicketExists(int.Parse(requestXML.Root.Element("ticket").Value)) != -1 && DatabaseHandler.Instance().ChangeTicketHelpdesk(int.Parse(requestXML.Root.Element("ticket").Value), Id) != -1 && DatabaseHandler.Instance().ChangeTicketStatus(int.Parse(requestXML.Root.Element("ticket").Value), TicketStatus.Rejected) != -1)
                {
                    return "<Message>Ticket rejected successfully</Message>";
                }
                else
                {
                    return "<Error>Unknown error during ticket rejection. Try again later.</Error>";
                }
            }
            else
            {
                return "<Error>Missing data.</Error>";
            }
        }

        internal string CompleteTicket(XDocument requestXML)
        {
            if (requestXML.Root.Element("ticket") != null && int.Parse(requestXML.Root.Element("ticket").Value) > 0)
            {
                if (DatabaseHandler.Instance().TicketExists(int.Parse(requestXML.Root.Element("ticket").Value)) != -1 && DatabaseHandler.Instance().ChangeTicketStatus(int.Parse(requestXML.Root.Element("ticket").Value), TicketStatus.Completed) != -1)
                {
                    return "<Message>Ticket completed successfully</Message>";
                }
                else
                {
                    return "<Error>Unknown error during ticket completion. Try again later.</Error>";
                }
            }
            else
            {
                return "<Error>Missing data.</Error>";
            }
        }

        internal override string ListTickets(XDocument requestXML)
        {
            HashSet<Tuple<int, string, string, string>> data;

            try
            {
                data = DatabaseHandler.Instance().GetNotTakenAndTakenByHelpdeskId(Id);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return "<Error>Server encountered an Error during listing tickets. Try again later.</Error>";
            }

            if (data.Count > 0)
            {
                string returnList = "<Tickets>";

                data.ToList().ForEach(data => returnList += "<Ticket>" + "<Id>" + data.Item1 + "</Id>" + "<Message>" + data.Item2 + "</Message>" + "<Date>" + data.Item3 + "</Date>" + "<Status>" + data.Item4 + "</Status>" + "</Ticket>");

                return returnList + "</Tickets>";
            }
            else
            {
                return "<Message>No tickets.</Message>";
            }
        }

        internal string ListMyTickets(XDocument requestXML)
        {
            HashSet<Tuple<int, string, string, string>> data;

            try
            {
                data = DatabaseHandler.Instance().GetHelpdeskTickets(Id);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return "<Error>Server encountered an Error during listing tickets. Try again later.</Error>";
            }

            if (data.Count > 0)
            {
                string returnList = "<Tickets>";

                data.ToList().ForEach(data => returnList += "<Ticket>" + "<Id>" + data.Item1 + "</Id>" + "<Message>" + data.Item2 + "</Message>" + "<Date>" + data.Item3 + "</Date>" + "<Status>" + data.Item4 + "</Status>" + "</Ticket>");

                return returnList + "</Tickets>";
            }
            else
            {
                return "<Message>No tickets.</Message>";
            }
        }

        internal string ListUnclaimedTickets(XDocument requestXML)
        {
            HashSet<Tuple<int, string, string, string>> data;

            try
            {
                data = DatabaseHandler.Instance().GetNotTakenTickets();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return "<Error>Server encountered an Error during listing tickets. Try again later.</Error>";
            }

            if (data.Count > 0)
            {
                string returnList = "<Tickets>";

                data.ToList().ForEach(data => returnList += "<Ticket>" + "<Id>" + data.Item1 + "</Id>" + "<Message>" + data.Item2 + "</Message>" + "<Date>" + data.Item3 + "</Date>" + "<Status>" + data.Item4 + "</Status>" + "</Ticket>");

                return returnList + "</Tickets>";
            }
            else
            {
                return "<Message>No tickets.</Message>";
            }
        }
    }
}
