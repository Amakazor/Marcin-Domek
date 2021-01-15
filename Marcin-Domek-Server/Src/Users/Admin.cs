using Marcin_Domek_Server.Src.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

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

        protected Admin() : base()
        {

        }

        internal string ListUsers(XDocument requestXML)
        {
            HashSet<int> ids;

            try
            {
                ids = DatabaseHandler.Instance().GetAllUserIds();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return "<Error>Server encountered an error during listing users. Try again later.</Error>";
            }

            if (ids.Count > 0)
            {
                string returnList = "<Users>";

                foreach (var id in ids.Where(id => DatabaseHandler.Instance().UserExists(id) == 1)) returnList += new User(id).ToXmlString();

                return returnList + "</Users>";
            }
            else
            {
                return "<Message>No users.</Message>";
            }
        }

        internal string AddUser(XDocument requestXML)
        {
            string response;

            if (!(requestXML.Root.Element("User") is null) && 
                !(requestXML.Root.Element("User").Element("FirstName") is null) && 
                !(requestXML.Root.Element("User").Element("LastName") is null) && 
                !(requestXML.Root.Element("User").Element("UserType") is null) && 
                !(requestXML.Root.Element("User").Element("Login") is null) && 
                !(requestXML.Root.Element("User").Element("Password") is null))
            {
                DatabaseHandler.Instance().AddUser(
                    requestXML.Root.Element("User").Element("FirstName").Value,
                    requestXML.Root.Element("User").Element("LastName").Value,
                    (UserType)Enum.Parse(UserType.GetType(), requestXML.Root.Element("User").Element("UserType").Value),
                    requestXML.Root.Element("User").Element("Login").Value,
                    requestXML.Root.Element("User").Element("Password").Value);

                response = "<Message>User added successfully</Message>";
            }
            else
            {
                response = "<Error>User wasn't added. Missing data</Error>";
            }

            return response;
        }

        internal string DeleteUser(XDocument requestXML)
        {
            string response;

            if (!(requestXML.Root.Element("User") is null) &&
                !(requestXML.Root.Element("User").Element("Id") is null))
            {
                DatabaseHandler.Instance().DeleteUser(int.Parse(requestXML.Root.Element("User").Element("Id").Value));
                response = "<Message>User deleted successfully</Message>";
            }
            else
            {
                response = "<Error>User wasn't deleted. Missing data</Error>";
            }

            return ListUsers(requestXML) + response;
        }

        internal string ChangeUserPassword(XDocument requestXML)
        {
            string response;

            if (!(requestXML.Root.Element("User") is null) &&
                !(requestXML.Root.Element("User").Element("Id") is null) &&
                !(requestXML.Root.Element("User").Element("Password") is null))
            {
                DatabaseHandler.Instance().UpdateUserPassword(int.Parse(requestXML.Root.Element("User").Element("Id").Value), requestXML.Root.Element("User").Element("Password").Value);
                response = "<Message>Users password changed successfully</Message>";
            }
            else
            {
                response = "<Error>Users password wasn't changed. Missing data</Error>";
            }

            return ListUsers(requestXML) + response;
        }

        internal string ChangeUserType(XDocument requestXML)
        {
            string response;

            if (!(requestXML.Root.Element("User") is null) &&
                !(requestXML.Root.Element("User").Element("Id") is null) &&
                !(requestXML.Root.Element("User").Element("Type") is null))
            {
                DatabaseHandler.Instance().ChangeUserType(int.Parse(requestXML.Root.Element("User").Element("Id").Value), (UserType)Enum.Parse(UserType.GetType(), requestXML.Root.Element("User").Element("Type").Value));
                response = "<Message>Users type changed successfully</Message>";
            }
            else
            {
                response = "<Error>Users type wasn't changed. Missing data</Error>";
            }

            return ListUsers(requestXML) + response;
        }
    }
}