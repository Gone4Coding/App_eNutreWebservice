using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Web.Hosting;
using System.Xml;

namespace WebserviceAppNutre
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class ServiceAppNutre : IServiceAppNutre
    {
        private Dictionary<string, User> users;
        private Dictionary<string, Token> tokens;

        private static string FILEPATH;

        public ServiceAppNutre()
        {
            users = new Dictionary<string, User>();
            tokens = new Dictionary<string, Token>();
            fillUsers();

            FILEPATH = Path.Combine(HostingEnvironment.ApplicationPhysicalPath, "App_Data", "users.xml");
        }

        private class Token
        {
            private string value;
            private long timeout;
            private User user;

            public Token(User user)
                : this(user, 240000) // token válido por 4 minutos
            {
            }

            public Token(User user, long timeout)
            {
                this.value = Guid.NewGuid().ToString();
                this.timeout = Environment.TickCount + timeout;
                this.user = user;
            }

            public string Value
            {
                get { return value; }
            }

            public long Timeout
            {
                get { return timeout; }
            }

            public User User
            {
                get { return user; }
            }

            public string Username
            {
                get { return user.Username; }
            }

            public void UpdateTimeout()
            {
                UpdateTimeout(240000); // token renovado por 4 minutos
            }

            public void UpdateTimeout(long timeout)
            {
                this.timeout = Environment.TickCount + timeout;
            }

            public Boolean isTimeoutExpired()
            {
                return Environment.TickCount > timeout;
            }
        }

        private void cleanUpTokens()
        {
            foreach (Token tokenObject in tokens.Values)
            {
                if (tokenObject.isTimeoutExpired())
                {
                    tokens.Remove(tokenObject.Username);
                }
            }
        }

        /*private bool isUserValid(string username, string password)
        {
            
            XmlDocument doc = new XmlDocument();
            doc.Load(FILEPATH);

            XmlNode passNode = doc.SelectSingleNode("//user[username = '" + username + "']//password");

            return passNode != null && passNode.InnerText.Equals(password);
        }*/

        private void fillUsers()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(FILEPATH);
            XmlNodeList usersList = doc.SelectNodes("//user");

            foreach (XmlNode user in usersList)
            {
                string username = user.SelectSingleNode("//username").InnerText;
                string password = user.SelectSingleNode("//pasword").InnerText;
                bool isAdmin = Convert.ToBoolean(user.SelectSingleNode("//@isAdmin").InnerText);

                users.Add(username, new User(username, password, isAdmin));
            }
        }

        public void SignUp(User user, string token)
        {
            throw new NotImplementedException();
        }

        public void LogIn(string username, string password)
        {
            cleanUpTokens();

            if (!String.IsNullOrEmpty(username) && !String.IsNullOrEmpty(password) && password.Equals(users[username].Password))
            {
                Token tokenObject = new Token(users[username]);
                tokens.Add(tokenObject.Value, tokenObject);
            }
            throw new ArgumentException("ERROR: invalid username/password combination.");

        }

        public void LogOut(string token)
        {
            tokens.Remove(token);
            cleanUpTokens();
        }

        public void addActivity(Activity activity, string token)
        {
            throw new NotImplementedException();
        }

        public void addActivity(XmlDocument activitiesXml, string token)
        {
            throw new NotImplementedException();
        }

        public void addRestaurant(Restaurant restaurant, string token) 
        {
            throw new NotImplementedException();
        }

        public void addRestaurant(XmlDocument restaurantsXml, string token)
        {
            throw new NotImplementedException();
        }

        public void addVegetable(Vegetable vegetable, string token)
        {
            throw new NotImplementedException();
        }

        public void addVegetable(XmlDocument vegetablesXml, string token)
        {
            throw new NotImplementedException();
        }

        public List<Activity> getActivitiesList()
        {
            throw new NotImplementedException();
        }

        public List<Restaurant> GetRestaurantsList()
        {
            throw new NotImplementedException();
        }

        public List<Vegetable> getVegetablesList()
        {
            throw new NotImplementedException();
        }
    }
}
