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
        private Dictionary<string, TimeOut> timeOuts = new Dictionary<string, TimeOut>();

        private static string FILEPATH = Path.Combine(HostingEnvironment.ApplicationPhysicalPath, "App_Data", "users.xml");
        
        private class TimeOut
        {
            private string token;
            private long timeout;
            

            public TimeOut(string token)
            {
                this.token = token;
                this.timeout = Environment.TickCount + timeout;
            }

            public string Token
            {
                get { return token; }
            }

            public long Timeout
            {
                get { return timeout; }
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

        private void cleanUpTimeOuts()
        {
            foreach (TimeOut timeOutObject in timeOuts.Values)
            {
                if (timeOutObject.isTimeoutExpired())
                {
                    timeOuts.Remove(timeOutObject.Token);
                }
            }
        }

        private bool isUserValid(string username, string password)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(FILEPATH);

            XmlNode passNode = doc.SelectSingleNode("//user[username = '" + username + "']//password");

            return passNode != null && passNode.InnerText.Equals(password);
        }

        public void SignUp(User user)
        {
            string username = user.Username;
            string password = user.Password;
            bool admin = user.Admin;

            XmlDocument doc = new XmlDocument();

            XmlNode userExistsNode = doc.SelectSingleNode("//user[username = '" + username + "'");

            if(userExistsNode != null)
                throw new ArgumentException("Username already exists");

            User userIntern = new User(username, password, admin);

            XmlNode root = doc.SelectSingleNode("//users");

            XmlElement userNode = doc.CreateElement("user");
            userNode.SetAttribute("isAdmin", userIntern.Admin.ToString());
            userNode.SetAttribute("id", userIntern.Id.ToString());

            XmlElement usernameNode = doc.CreateElement("username");
            usernameNode.InnerText = userIntern.Username;

            XmlElement passwordNode = doc.CreateElement("password");
            passwordNode.InnerText = userIntern.Password;

            //FALTA CENAS
        }

        public void LogIn(string username, string password)
        {
            cleanUpTokens();

            if (!String.IsNullOrEmpty(username) && !String.IsNullOrEmpty(password) && isUserValid(username, password))
            {
                XmlDocument doc = new XmlDocument();

                String tokenNode = doc.SelectSingleNode("//user[username = '" + username + "'//token").InnerText;

                TimeOut timeOut = new TimeOut(tokenNode);
                timeOuts.Add(timeOut.Token, timeOut);
            }
            throw new ArgumentException("ERROR: invalid username/password combination.");

        }

        public void LogOut(string token)
        {
            timeOuts.Remove(token);
            cleanUpTimeOuts();
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
