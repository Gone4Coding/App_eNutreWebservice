using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Web.Hosting;
using System.Xml;

namespace WebserviceAppNutre
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class ServiceAppNutre : IServiceAppNutre
    {
        private static string USERS_FILEPATH = Path.Combine(HostingEnvironment.ApplicationPhysicalPath, "App_Data", "users.xml");
        private static string ACTIVITY_FILEPATH = Path.Combine(HostingEnvironment.ApplicationPhysicalPath, "App_Data", "exercises.xml");
        private static string PLATE_FILEPATH = Path.Combine(HostingEnvironment.ApplicationPhysicalPath, "App_Data", "restaurants.xml");
        private static string VEGETABLE_FILEPATH = Path.Combine(HostingEnvironment.ApplicationPhysicalPath, "App_Data", "vegetables.xml");
        private static string TOKEN_FILEPATH = Path.Combine(HostingEnvironment.ApplicationPhysicalPath, "App_Data", "tokens.xml");

        private class Token
        {
            private string value;
            private string username;
            private bool isAdmin;

            public Token(string username, bool isAdmin)
            {
                this.value = Guid.NewGuid().ToString();
                this.username = username;
                this.isAdmin = isAdmin;
              //  saveToken();
            }

            public string Value
            {
                get { return value; }
            }

            public string Username
            {
                get { return username; }
            }

            private void saveToken()
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(TOKEN_FILEPATH);

                XmlNode tokensNode = doc.SelectSingleNode("//tokens");

                XmlElement tokenNode = doc.CreateElement("token");
                tokenNode.SetAttribute("isAdmin", isAdmin.ToString());

                XmlElement usernameNode = doc.CreateElement("username");
                usernameNode.InnerText = username;

                XmlElement valueNode = doc.CreateElement("value");
                valueNode.InnerText = value;

                doc.Save(TOKEN_FILEPATH);
            }
        }

        private void cleanUpTokens(string username)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(TOKEN_FILEPATH);

            XmlNode root = doc.SelectSingleNode("/tokens");
            XmlNodeList tokens = doc.SelectNodes("//token[username = '"+ username +"']");

            foreach (XmlNode token in tokens)
            {
                root.RemoveChild(token);
                doc.Save(TOKEN_FILEPATH);
            }
       
        }

        public void SignUp(User user, string token)
        {
            string username = user.Username;
            string password = user.Password;
            bool admin = user.Admin;

            XmlDocument doc = new XmlDocument();
            doc.Load(USERS_FILEPATH);

            XmlNode userExistsNode = doc.SelectSingleNode("//user[username = '" + username + "'");

            if (userExistsNode != null)
                throw new ArgumentException("Username already exists");

            XmlNode root = doc.SelectSingleNode("//users");

            XmlElement userNode = doc.CreateElement("user");
            userNode.SetAttribute("isAdmin", admin.ToString());
            userNode.SetAttribute("id", getValidUserId().ToString());

            XmlElement usernameNode = doc.CreateElement("username");
            usernameNode.InnerText = username;

            XmlElement passwordNode = doc.CreateElement("password");
            passwordNode.InnerText = getPasswordCrypt(password);

            userNode.AppendChild(usernameNode);
            userNode.AppendChild(passwordNode);
            root.AppendChild(userNode);

            doc.Save(USERS_FILEPATH);
        }

        public string LogIn(string username, string password)
        {
            cleanUpTokens(username);

            if (!String.IsNullOrEmpty(username) && !String.IsNullOrEmpty(password) && isUserValid(username, password))
            {
                if (isAdmin(username))
                {
                    Token token = new Token(username, true);
                    return token.Value;
                }
                if (tokenExistsForToken(username)==false)
                {
                    string t = Guid.NewGuid().ToString();

                    XmlDocument doc = new XmlDocument();
                    doc.Load(TOKEN_FILEPATH);

                    XmlNode root = doc.SelectSingleNode("/tokens");
                    //username e token
                   
                    XmlElement tokenElem = doc.CreateElement("token");

                    XmlElement usernameTkXML = doc.CreateElement("username");
                    usernameTkXML.InnerText = username;

                    XmlElement value = doc.CreateElement("value");
                    value.InnerText = t;

                    tokenElem.AppendChild(usernameTkXML);
                    tokenElem.AppendChild(value);
                   
                    root.AppendChild(tokenElem);

                    doc.Save(TOKEN_FILEPATH);

                    return t;
                }
                else
                {
                    XmlDocument doc = new XmlDocument();
                    doc.Load(TOKEN_FILEPATH);

                    XmlNode node = doc.SelectSingleNode("//token[username = '" + username + "']/value");
                }
                
            }
            else
            {
                throw new ArgumentException("ERROR: invalid username/password combination.");
            }

            return "NULL";
        }

        public void LogOut(string username)
        {
            cleanUpTokens(username);

            XmlDocument doc = new XmlDocument();
            doc.Load(TOKEN_FILEPATH);

            XmlNode root = doc.SelectSingleNode("/tokens");
            XmlNode tokenNode = doc.SelectSingleNode("//token[username = '" + username + "']");

            if (tokenNode != null)
            {
                root.RemoveChild(tokenNode);
                doc.Save(TOKEN_FILEPATH);
            }
        }

        public void addActivity(Activity activity, string username)
        {
            cleanUpTokens(username);

            if (isAdmin(username))
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(ACTIVITY_FILEPATH);

                XmlNode root = doc.CreateElement("/exercises");

                XmlElement exerciseNode = doc.CreateElement("exercise");
                exerciseNode.SetAttribute("id", getValidActivityId().ToString());

                XmlElement activityNode = doc.CreateElement("activity");
                activityNode.InnerText = activity.Nome;

                XmlElement metNode = doc.CreateElement("met");

                XmlElement metNameNode = doc.CreateElement("name");
                metNameNode.InnerText = "Metabolic Equivalent";
                metNode.AppendChild(metNameNode);

                XmlElement metValuetNode = doc.CreateElement("value");
                metValuetNode.InnerText = activity.Met.ToString();
                metNode.AppendChild(metValuetNode);

                XmlElement caloriesNode = doc.CreateElement("calories");

                XmlElement caloriesValueNode = doc.CreateElement("value");
                caloriesValueNode.InnerText = activity.Calorias.ToString();
                caloriesNode.AppendChild(caloriesValueNode);

                XmlElement caloriesUnitNode = doc.CreateElement("unity");
                caloriesUnitNode.InnerText = "kcal";
                caloriesNode.AppendChild(caloriesUnitNode);

                exerciseNode.AppendChild(activityNode);
                exerciseNode.AppendChild(metNode);
                exerciseNode.AppendChild(caloriesNode);

                root.AppendChild(exerciseNode);

                doc.Save(ACTIVITY_FILEPATH);
            }
            else
            {
                throw new ArgumentException("ERROR: User not valid");
            }
        }

        public void addActivityXML(XmlDocument activitiesXml, string token)
        {
            throw new NotImplementedException();
        }

        public void addRestaurant(Restaurant restaurant, string token)
        {
            throw new NotImplementedException();
        }

        public void addRestaurantXML(XmlDocument restaurantsXml, string token)
        {
            throw new NotImplementedException();
        }

        public void addVegetable(Vegetable vegetable, string token)
        {
            throw new NotImplementedException();
        }

        public void addVegetableXML(XmlDocument vegetablesXml, string token)
        {

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
            XmlDocument doc = new XmlDocument();
            doc.Load(VEGETABLE_FILEPATH);
            XmlNodeList nodes;
            List<Vegetable> lista = new List<Vegetable>();
            nodes = doc.SelectNodes("//food");
            List<string> extraInfo = new List<string>();
            foreach (XmlNode s in nodes)
            {
                int id = int.Parse(s.SelectSingleNode("@id").InnerText);
                string name = s.SelectSingleNode("vegetable").InnerText;
                XmlNodeList nodesExtra = s.SelectNodes("/extraInfo");
                foreach (XmlNode extra in nodesExtra)
                {
                    string extraInformacao = extra.SelectSingleNode("extraInfo").InnerText;
                    extraInfo.Add(extraInformacao);
                }
                XmlNode quantity = s.SelectSingleNode("quantity");
                double quantityValue;
                if (quantity.SelectSingleNode("value").InnerText.Equals("1/2"))
                {
                    quantityValue = 0.5;
                }
                else
                {
                    quantityValue = double.Parse(quantity.SelectSingleNode("value").InnerText);
                }

                string unityQuantity = quantity.SelectSingleNode("unity").InnerText;
                XmlNode calories = s.SelectSingleNode("calories");
                int caloriesValue = int.Parse(calories.SelectSingleNode("value").InnerText);
                string unityCal = calories.SelectSingleNode("unity").InnerText;


                lista.Add(new Vegetable(id, name, extraInfo, quantityValue, unityQuantity, caloriesValue, unityCal));
            }
            return lista;
        }

        private int getValidUserId()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(USERS_FILEPATH);

            XmlNodeList idList = doc.SelectNodes("//@id");
            return idList.Count + 1;
        }

        private int getValidActivityId()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(ACTIVITY_FILEPATH);

            XmlNodeList actsIds = doc.SelectNodes("//@id");
            return actsIds.Count + 1;
        }

        private int getValidPlateId()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(PLATE_FILEPATH);

            XmlNodeList platesIds = doc.SelectNodes("//@id");
            return platesIds.Count + 1;
        }

        private int getValidVegetableId()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(VEGETABLE_FILEPATH);

            XmlNodeList VegetableIds = doc.SelectNodes("//@id");
            return VegetableIds.Count + 1;
        }

        private string getPasswordCrypt(string password)
        {
            StringBuilder builder = new StringBuilder();

            using (SHA256 hash = SHA256.Create())
            {
                Encoding enc = Encoding.UTF8;
                Byte[] result = hash.ComputeHash(enc.GetBytes(password));

                foreach (byte b in result)
                {
                    builder.Append(b.ToString("x2"));
                }
            }

            return builder.ToString();
        }

        private bool isUserValid(string username, string password)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(USERS_FILEPATH);

                String passNode = doc.SelectSingleNode("//user[username = '" + username + "']//password").InnerText;

                if (!passNode.Equals(password))
                    throw new Exception();
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        private bool isAdmin(string username)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(USERS_FILEPATH);

            XmlNode node = doc.SelectSingleNode("//user[username = '" + username + "']//@isAdmin");
            return node.InnerText.Equals("true");
        }

        private bool tokenExistsForToken(string username)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(TOKEN_FILEPATH);

            XmlNode node = doc.SelectSingleNode("//token[username = '" + username + "']");
            if (node != null)
            {
                return true;

            }
           
          return false;
            
        }
    }
}

