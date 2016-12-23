using System;
using System.Collections.Generic;
using System.Globalization;
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
        private static readonly string USERS_FILEPATH_XML = Path.Combine(HostingEnvironment.ApplicationPhysicalPath, "App_Data", "users.xml");
        private static readonly string USERS_FILEPATH_SCHEMA = Path.Combine(HostingEnvironment.ApplicationPhysicalPath, "App_Data", "usersSchema.xsd");

        private static readonly string ACTIVITY_FILEPATH_XML = Path.Combine(HostingEnvironment.ApplicationPhysicalPath, "App_Data", "exercises.xml");
        private static readonly string ACTIVITY_FILEPATH_SCHEMA = Path.Combine(HostingEnvironment.ApplicationPhysicalPath, "App_Data", "activitiesSchema.xml");

        private static readonly string PLATE_FILEPATH_XML = Path.Combine(HostingEnvironment.ApplicationPhysicalPath, "App_Data", "restaurants.xml");
        private static readonly string PLATE_FILEPATH_SCHEMA = Path.Combine(HostingEnvironment.ApplicationPhysicalPath, "App_Data", "restaurantsSchema.xml");

        private static readonly string VEGETABLE_FILEPATH_XML = Path.Combine(HostingEnvironment.ApplicationPhysicalPath, "App_Data", "vegetables.xml");
        private static readonly string VEGETABLE_FILEPATH_SCHEMA = Path.Combine(HostingEnvironment.ApplicationPhysicalPath, "App_Data", "vegetablesSchema.xml");

        private static readonly string TOKEN_FILEPATH_XML = Path.Combine(HostingEnvironment.ApplicationPhysicalPath, "App_Data", "tokens.xml");

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
                doc.Load(TOKEN_FILEPATH_XML);

                XmlNode tokensNode = doc.SelectSingleNode("//tokens");

                XmlElement tokenNode = doc.CreateElement("token");
                tokenNode.SetAttribute("isAdmin", isAdmin.ToString());

                XmlElement usernameNode = doc.CreateElement("username");
                usernameNode.InnerText = username;

                XmlElement valueNode = doc.CreateElement("value");
                valueNode.InnerText = value;

                doc.Save(TOKEN_FILEPATH_XML);
            }
        }

        private void cleanUpTokens(string username)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(TOKEN_FILEPATH_XML);

            XmlNode root = doc.SelectSingleNode("/tokens");
            XmlNodeList tokens = doc.SelectNodes("//token[username = '"+ username +"']");

            foreach (XmlNode token in tokens)
            {
                root.RemoveChild(token);
                doc.Save(TOKEN_FILEPATH_XML);
            }
       
        }

        public void SignUp(User user, string token)
        {
            string username = user.Username;
            string password = user.Password;
            bool admin = user.Admin;

            XmlDocument doc = new XmlDocument();
            doc.Load(USERS_FILEPATH_XML);

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

            doc.Save(USERS_FILEPATH_XML);
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
                    doc.Load(TOKEN_FILEPATH_XML);

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

                    doc.Save(TOKEN_FILEPATH_XML);

                    return t;
                }
                else
                {
                    XmlDocument doc = new XmlDocument();
                    doc.Load(TOKEN_FILEPATH_XML);

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
            doc.Load(TOKEN_FILEPATH_XML);

            XmlNode root = doc.SelectSingleNode("/tokens");
            XmlNode tokenNode = doc.SelectSingleNode("//token[username = '" + username + "']");

            if (tokenNode != null)
            {
                root.RemoveChild(tokenNode);
                doc.Save(TOKEN_FILEPATH_XML);
            }
        }

        public void addActivity(Activity activity, string username)
        {
            cleanUpTokens(username);

            if (isAdmin(username))
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(ACTIVITY_FILEPATH_XML);

                XmlNode root = doc.SelectSingleNode("/exercises");

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

                XmlElement caloriesNode = doc.CreateElement("caloriesValue");

                XmlElement caloriesValueNode = doc.CreateElement("value");
                caloriesValueNode.InnerText = activity.CaloriasValue.ToString();
                caloriesNode.AppendChild(caloriesValueNode);

                XmlElement caloriesUnitNode = doc.CreateElement("unity");
                caloriesUnitNode.InnerText = activity.CaloriasUnit;
                caloriesNode.AppendChild(caloriesUnitNode);

                exerciseNode.AppendChild(activityNode);
                exerciseNode.AppendChild(metNode);
                exerciseNode.AppendChild(caloriesNode);

                root.AppendChild(exerciseNode);

                doc.Save(ACTIVITY_FILEPATH_XML);
            }
            else
            {
                throw new ArgumentException("ERROR: User not valid");
            }
        }

        public void addActivityXML(XmlDocument activitiesXml, string token)
        {
            
            XmlDocument doc = new XmlDocument();

            XmlReaderSettings settings = new XmlReaderSettings();
            settings.Schemas.Add(null, ACTIVITY_FILEPATH_SCHEMA);
            settings.ValidationType = ValidationType.Schema;

            XmlReader reader = XmlReader.Create(activitiesXml.InnerXml, settings);
            doc.Load(reader);

            doc.Load(ACTIVITY_FILEPATH_XML);

            XmlNodeList activityList = activitiesXml.SelectNodes("//exercise");

            XmlNode root = doc.SelectSingleNode("/exercises");

            foreach (XmlNode actNode in activityList)
            {
                XmlElement exerciseNode = doc.CreateElement("exercise");
                exerciseNode.SetAttribute("id", getValidActivityId().ToString());

                XmlNode activityNode = actNode.SelectSingleNode("/activity");
                exerciseNode.AppendChild(activityNode);

                XmlNode metNode = actNode.SelectSingleNode("/met");
                exerciseNode.AppendChild(metNode);

                XmlNode caloriesNode = actNode.SelectSingleNode("/caloriesValue");
                exerciseNode.AppendChild(caloriesNode);

                root.AppendChild(exerciseNode);
            }

            doc.Save(ACTIVITY_FILEPATH_XML);
        }

        public void addRestaurant(Plate plate, string token)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(PLATE_FILEPATH_XML);

            XmlElement plateNode = doc.CreateElement("plate");
            plateNode.SetAttribute("id", getValidPlateId().ToString());

            XmlElement itemNode = doc.CreateElement("item");
            itemNode.InnerText = plate.Name;

            XmlElement quantityNode = doc.CreateElement("quantityValue");

            XmlElement quantityValueNode = doc.CreateElement("value");
            quantityValueNode.InnerText = plate.QuantityValue;
            quantityNode.AppendChild(quantityValueNode);

            XmlElement quantityDosageNode = doc.CreateElement("quantityDosage");
            quantityDosageNode.InnerText = plate.QuantityDosage;
            quantityNode.AppendChild(quantityDosageNode);

            if (plate.QuantityExtraDosage != null)
            {
                XmlElement quantityExtraDosageNode = doc.CreateElement("quantityExtraDosage");
                quantityExtraDosageNode.InnerText = plate.QuantityExtraDosage;
                quantityNode.AppendChild(quantityExtraDosageNode);
            }

            XmlElement caloriesNode = doc.CreateElement("caloriesValue");

            XmlElement caloriesValueNode = doc.CreateElement("value");
            caloriesValueNode.InnerText = plate.CaloriesValue.ToString();
            caloriesNode.AppendChild(caloriesValueNode);

            XmlElement caloriesUnitNode = doc.CreateElement("unity");
            caloriesUnitNode.InnerText = plate.CaloriasUnit;
            caloriesNode.AppendChild(caloriesUnitNode);

            plateNode.AppendChild(itemNode);
            plateNode.AppendChild(quantityNode);
            plateNode.AppendChild(caloriesNode);

            XmlNode root = doc.SelectSingleNode("/foods");

            XmlNode restaurantNode = doc.SelectSingleNode("/restaurant[name = '" + plate.RestaurantName + "']");

            if (restaurantNode == null)
            {
                restaurantNode = doc.CreateElement(plate.RestaurantName);
                restaurantNode.AppendChild(plateNode);
                root.AppendChild(restaurantNode);
            }
            else
            {
                restaurantNode.AppendChild(plateNode);
            }

            doc.Save(PLATE_FILEPATH_XML);
                

        }

        public void addRestaurantXML(XmlDocument platesXml, string token)
        {
            XmlDocument doc = new XmlDocument();

            XmlReaderSettings settings = new XmlReaderSettings();
            settings.Schemas.Add(null, PLATE_FILEPATH_SCHEMA);
            settings.ValidationType = ValidationType.Schema;

            XmlReader reader = XmlReader.Create(platesXml.InnerXml, settings);
            doc.Load(reader);

            doc.Load(PLATE_FILEPATH_XML);
            
        }

        public void addVegetable(Vegetable vegetable, string token)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(ACTIVITY_FILEPATH_XML);

            XmlNode root = doc.SelectSingleNode("/foods");

            XmlElement veggetableNode = doc.CreateElement("food");
            veggetableNode.SetAttribute("id", getValidVegetableId().ToString());

            XmlElement vegetableNameNode = doc.CreateElement("vegetable");
            vegetableNameNode.InnerText = vegetable.Name;

            veggetableNode.AppendChild(vegetableNameNode);

            List<string> extraInfoList = vegetable.ExtraInfo;

            if (extraInfoList != null)
            {
                foreach (string extraInfo in extraInfoList)
                {
                    XmlElement extraInfoNode = doc.CreateElement("extraInfo");
                    extraInfoNode.InnerText = extraInfo;
                    veggetableNode.AppendChild(extraInfoNode);
                }
            }

            XmlElement quantityNode = doc.CreateElement("quantityValue");

            XmlElement quantityValueNode = doc.CreateElement("value");
            quantityValueNode.InnerText = vegetable.QuantityValue.ToString();
            quantityNode.AppendChild(quantityValueNode);

            XmlElement quantityUnitNode = doc.CreateElement("unity");
            quantityUnitNode.InnerText = vegetable.UnityQuantity;
            quantityNode.AppendChild(quantityUnitNode);

            veggetableNode.AppendChild(quantityNode);

            XmlElement caloriesNode = doc.CreateElement("caloriesValue");

            XmlElement caloriesValueNode = doc.CreateElement("value");
            caloriesValueNode.InnerText = vegetable.CaloriesValue.ToString();
            caloriesNode.AppendChild(caloriesValueNode);

            XmlElement caloriesUnitNode = doc.CreateElement("unity");
            caloriesUnitNode.InnerText = vegetable.UnityCal;
            caloriesNode.AppendChild(caloriesUnitNode);

            veggetableNode.AppendChild(caloriesNode);

            root.AppendChild(veggetableNode);

            doc.Save(PLATE_FILEPATH_XML);
        }

        public void addVegetableXML(XmlDocument vegetablesXml, string token)
        {
            XmlDocument doc = new XmlDocument();

            XmlReaderSettings settings = new XmlReaderSettings();
            settings.Schemas.Add(null, VEGETABLE_FILEPATH_SCHEMA);
            settings.ValidationType = ValidationType.Schema;

            XmlReader reader = XmlReader.Create(vegetablesXml.InnerXml, settings);
            doc.Load(reader);

            doc.Load(VEGETABLE_FILEPATH_XML);

            XmlNodeList veggiesList = vegetablesXml.SelectNodes("//food");

            XmlNode root = doc.SelectSingleNode("/foods");

            foreach (XmlNode veggieNode in veggiesList)
            {
                XmlElement foodNode = doc.CreateElement("food");
                foodNode.SetAttribute("id", getValidVegetableId().ToString());

                XmlNode vegetableNode = veggieNode.SelectSingleNode("/vegetable");
                foodNode.AppendChild(vegetableNode);

                XmlNode extraInfoNode = veggieNode.SelectSingleNode("/extraInfo");

                if(extraInfoNode != null)
                    foodNode.AppendChild(extraInfoNode);

                XmlNode caloriesNode = veggieNode.SelectSingleNode("/caloriesValue");
                foodNode.AppendChild(caloriesNode);

                root.AppendChild(foodNode);
            }

            doc.Save(PLATE_FILEPATH_XML);
        }

        public List<Activity> getActivitiesList()
        {
            throw new NotImplementedException();
        }

        public List<Plate> GetRestaurantsList()
        {
            throw new NotImplementedException();
        }

        public List<Vegetable> getVegetablesList()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(VEGETABLE_FILEPATH_XML);
            XmlNodeList nodes = nodes = doc.SelectNodes("//food");
            List<Vegetable> lista = new List<Vegetable>();
            
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
                XmlNode quantity = s.SelectSingleNode("quantityValue");
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
                XmlNode calories = s.SelectSingleNode("caloriesValue");
                int caloriesValue = int.Parse(calories.SelectSingleNode("value").InnerText);
                string unityCal = calories.SelectSingleNode("unity").InnerText;


                //lista.Add(new Vegetable(id, name, extraInfo, quantityValue, unityQuantity, caloriesValue, unityCal));
            }
            return lista;
        }

        private int getValidUserId()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(USERS_FILEPATH_XML);

            XmlNodeList idList = doc.SelectNodes("//@id");
            return idList.Count + 1;
        }

        private int getValidActivityId()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(ACTIVITY_FILEPATH_XML);

            XmlNodeList actsIds = doc.SelectNodes("//@id");
            return actsIds.Count + 1;
        }

        private int getValidPlateId()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(PLATE_FILEPATH_XML);

            XmlNodeList platesIds = doc.SelectNodes("//@id");
            return platesIds.Count + 1;
        }

        private int getValidVegetableId()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(VEGETABLE_FILEPATH_XML);

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
                doc.Load(USERS_FILEPATH_XML);

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
            doc.Load(USERS_FILEPATH_XML);

            XmlNode node = doc.SelectSingleNode("//user[username = '" + username + "']//@isAdmin");
            return node.InnerText.Equals("true");
        }

        private bool tokenExistsForToken(string username)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(TOKEN_FILEPATH_XML);

            XmlNode node = doc.SelectSingleNode("//token[username = '" + username + "']");
            if (node != null)
            {
                return true;

            }
           
          return false;
            
        }
    }
}

