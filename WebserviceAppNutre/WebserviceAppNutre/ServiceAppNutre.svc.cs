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

        private Dictionary<string, Token> tokens = new Dictionary<string, Token>();
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
            XmlNodeList tokens = doc.SelectNodes("//token[username = '" + username + "']");

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
                    tokens.Add(token.Value, token);
                    return token.Value;
                }

                if (tokenExistsForToken(username) == false)
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
                return null;
            }

            return null;
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

        public bool addActivity(Activity activity, string token)
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
            metValuetNode.InnerText = activity.Met;
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

            return true;
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

        public bool addRestaurant(Plate plate, string token)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(PLATE_FILEPATH_XML);

            XmlElement plateNode = doc.CreateElement("plate");
            plateNode.SetAttribute("id", getValidPlateId().ToString());

            XmlElement itemNode = doc.CreateElement("item");
            itemNode.InnerText = plate.Name;

            XmlElement quantityNode = doc.CreateElement("quantity");

            XmlElement quantityValueNode = doc.CreateElement("value");
            quantityValueNode.InnerText = plate.QuantityValue.ToString();
            quantityNode.AppendChild(quantityValueNode);

            XmlElement quantityDosageNode = doc.CreateElement("dosage");
            quantityDosageNode.InnerText = plate.QuantityDosage;
            quantityNode.AppendChild(quantityDosageNode);

            if (plate.QuantityExtraDosage != null)
            {
                XmlElement quantityExtraDosageNode = doc.CreateElement("extraDosage");
                quantityExtraDosageNode.InnerText = plate.QuantityExtraDosage;
                quantityNode.AppendChild(quantityExtraDosageNode);
            }

            XmlElement caloriesNode = doc.CreateElement("calories");

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

            XmlNode restaurantNode = doc.SelectSingleNode("//restaurant[@name = '" + plate.RestaurantName + "']");

            if (restaurantNode == null)
            {
                XmlElement restaurant = doc.CreateElement("restaurant");
                restaurant.SetAttribute("name", plate.RestaurantName);
                restaurant.AppendChild(plateNode);
                root.AppendChild(restaurant);
            }
            else
            {
                restaurantNode.AppendChild(plateNode);
            }

            doc.Save(PLATE_FILEPATH_XML);

            return true;

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

        public bool addVegetable(Vegetable vegetable, string token)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(VEGETABLE_FILEPATH_XML);

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

            XmlElement quantityNode = doc.CreateElement("quantity");

            XmlElement quantityValueNode = doc.CreateElement("value");
            quantityValueNode.InnerText = vegetable.QuantityValue.ToString();
            quantityNode.AppendChild(quantityValueNode);

            XmlElement quantityUnitNode = doc.CreateElement("unity");
            quantityUnitNode.InnerText = vegetable.UnityQuantity;
            quantityNode.AppendChild(quantityUnitNode);

            veggetableNode.AppendChild(quantityNode);

            XmlElement caloriesNode = doc.CreateElement("calories");

            XmlElement caloriesValueNode = doc.CreateElement("value");
            caloriesValueNode.InnerText = vegetable.CaloriesValue.ToString();
            caloriesNode.AppendChild(caloriesValueNode);

            XmlElement caloriesUnitNode = doc.CreateElement("unity");
            caloriesUnitNode.InnerText = vegetable.UnityCal;
            caloriesNode.AppendChild(caloriesUnitNode);

            veggetableNode.AppendChild(caloriesNode);

            root.AppendChild(veggetableNode);

            doc.Save(VEGETABLE_FILEPATH_XML);

            return true;
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

                if (extraInfoNode != null)
                    foodNode.AppendChild(extraInfoNode);

                XmlNode quantityNode = veggieNode.SelectSingleNode("/quantity");
                foodNode.AppendChild(quantityNode);


                XmlNode caloriesNode = veggieNode.SelectSingleNode("/calories");
                foodNode.AppendChild(caloriesNode);

                root.AppendChild(foodNode);
            }

            doc.Save(VEGETABLE_FILEPATH_XML);
        }

        public List<Activity> getActivitiesList()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(ACTIVITY_FILEPATH_XML);
            XmlNodeList nodes = nodes = doc.SelectNodes("//exercise");
            List<Activity> lista = new List<Activity>();

            foreach (XmlNode s in nodes)
            {
                int id = int.Parse(s.SelectSingleNode("@id").InnerText);
                string name = s.SelectSingleNode("activity").InnerText;

                XmlNode met = s.SelectSingleNode("met");
                string metName = met.SelectSingleNode("name").InnerText;
                string metValue = met.SelectSingleNode("value").InnerText;
                XmlNode calories = s.SelectSingleNode("caloriesValue");
                int caloriesValue = int.Parse(calories.SelectSingleNode("value").InnerText);
                string unityCal = calories.SelectSingleNode("unity").InnerText;

                Activity act = new Activity(name, caloriesValue, unityCal, metName, metValue);
                act.Id = id;

                lista.Add(act);
            }
            return lista;
        }

        public List<Plate> GetRestaurantsList()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(PLATE_FILEPATH_XML);
            XmlNodeList nodes = doc.SelectNodes("//plate");
            List<Plate> lista = new List<Plate>();
            int id = 0;
            string name = "";
            string quantityValue = "";
            string dosage = "";
            string extraDosage = "";
            int caloriesValue = 0;
            string caloriesUnity = "";

            foreach (XmlNode node in nodes)
            {
                string restaurantName = node.SelectSingleNode("parent::restaurant/@name").InnerText;
                id = int.Parse(node.SelectSingleNode("@id").InnerText);
                name = node.SelectSingleNode("item").InnerText;
                XmlNode quantity = node.SelectSingleNode("quantity");
                quantityValue = quantity.SelectSingleNode("value").InnerText;
                dosage = quantity.SelectSingleNode("dosage").InnerText;
                if (quantity.SelectSingleNode("extraDosage") != null)
                {
                    extraDosage = quantity.SelectSingleNode("extraDosage").InnerText;
                }
                else
                {
                    extraDosage = "";
                }
                XmlNode calories = node.SelectSingleNode("calories");
                caloriesValue = int.Parse(calories.SelectSingleNode("value").InnerText);
                caloriesUnity = calories.SelectSingleNode("unity").InnerText;

                Plate plate = new Plate(name, restaurantName, quantityValue, dosage, extraDosage, caloriesValue,
                    caloriesUnity);
                plate.Id = id;

                lista.Add(plate);
            }
            return lista;
        }

        public List<Vegetable> getVegetablesList()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(VEGETABLE_FILEPATH_XML);
            XmlNodeList nodes = doc.SelectNodes("//food");
            List<Vegetable> lista = new List<Vegetable>();


            foreach (XmlNode s in nodes)
            {
                List<string> extraInfo = new List<string>();

                int id = int.Parse(s.SelectSingleNode("@id").InnerText);
                string name = s.SelectSingleNode("vegetable").InnerText;

                XmlNode nodesExtra = s.SelectSingleNode("extraInfo");
                if (nodesExtra != null)
                    extraInfo.Add(nodesExtra.InnerText);
                /*foreach (XmlNode extra in nodesExtra)
                {
                    string extraInformacao = extra.SelectSingleNode("extraInfo").InnerText;
                    extraInfo.Add(extraInformacao);
                }*/

                XmlNode quantity = s.SelectSingleNode("quantity");
                string quantityValue = quantity.SelectSingleNode("value").InnerText;
                string unityQuantity = quantity.SelectSingleNode("unity").InnerText;
                XmlNode calories = s.SelectSingleNode("calories");
                int caloriesValue = int.Parse(calories.SelectSingleNode("value").InnerText);
                string unityCal = calories.SelectSingleNode("unity").InnerText;
                Vegetable veggie = new Vegetable(name, extraInfo, quantityValue, unityQuantity, caloriesValue, unityCal);
                veggie.Id = id;

                lista.Add(veggie);
            }
            return lista;
        }

        public bool removeActivity(int id, string token)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(ACTIVITY_FILEPATH_XML);
            XmlNode root = doc.DocumentElement;
            XmlNode nodeToRemove = doc.SelectSingleNode("//exercise[@id='" + id + "']");
            root.RemoveChild(nodeToRemove);

            doc.Save(ACTIVITY_FILEPATH_XML);

            return true;
        }

        public bool removePlate(int id, string token)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(PLATE_FILEPATH_XML);
            XmlNode root = doc.DocumentElement;
            XmlNode nodeRestaurant = doc.SelectSingleNode("//restaurant[plate[@id='" + id + "']]");
            XmlNode nodeToRemove = doc.SelectSingleNode("//plate[@id='" + id + "']");
            // root.RemoveChild(nodeToRemove);
            nodeRestaurant.RemoveChild(nodeToRemove);
            doc.Save(PLATE_FILEPATH_XML);

            return true;
        }

        public bool removeVegetable(int id, string token)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(VEGETABLE_FILEPATH_XML);
            XmlNode root = doc.DocumentElement;

            XmlNode nodeToRemove = doc.SelectSingleNode("//food[@id='" + id + "']");
            root.RemoveChild(nodeToRemove);

            doc.Save(VEGETABLE_FILEPATH_XML);

            return true;
        }

        public string[] getCaloriesByActivity(int id)
        {
            string[] calorieString = { "", "" };
            XmlDocument doc = new XmlDocument();
            doc.Load(ACTIVITY_FILEPATH_XML);
            XmlNode node = doc.SelectSingleNode("//exercise[@id='" + id + "']");
            XmlNode activity = node.SelectSingleNode("caloriesValue");

            calorieString[0] = activity.SelectSingleNode("value").InnerText;
            calorieString[1] = activity.SelectSingleNode("unity").InnerText;

            return calorieString;
        }

        public string[] getCaloriesByVeggie(int id)
        {
            string[] calorieString = { "", "" };
            XmlDocument doc = new XmlDocument();
            doc.Load(VEGETABLE_FILEPATH_XML);
            XmlNode node = doc.SelectSingleNode("//food[@id='" + id + "']");
            XmlNode vegetable = node.SelectSingleNode("calories");

            calorieString[0] = vegetable.SelectSingleNode("value").InnerText;
            calorieString[1] = vegetable.SelectSingleNode("unity").InnerText;

            return calorieString;
        }

        public string[] getCaloriesByPlate(int id)
        {
            string[] calorieString = { "", "" };
            XmlDocument doc = new XmlDocument();
            doc.Load(PLATE_FILEPATH_XML);
            XmlNode node = doc.SelectSingleNode("//plate[@id='" + id + "']");
            XmlNode vegetable = node.SelectSingleNode("calories");

            calorieString[0] = vegetable.SelectSingleNode("value").InnerText;
            calorieString[1] = vegetable.SelectSingleNode("unity").InnerText;

            return calorieString;
        }

        public List<Activity> getActivitiesListByCalories(int calories, string unity)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(ACTIVITY_FILEPATH_XML);
            double limiteMax = 0;

            if (calories < 100 && unity.Equals("cal"))
            {
                limiteMax = calories + 20;

            }
            else if (calories >= 100 && unity.Equals("cal") || unity.Equals("kcal"))
            {
                limiteMax = calories + calories * 0.2;

            }

            XmlNodeList nodes = nodes = doc.SelectNodes("//exercise");
            List<Activity> lista = new List<Activity>();

            foreach (XmlNode s in nodes)
            {
                int id = int.Parse(s.SelectSingleNode("@id").InnerText);
                string name = s.SelectSingleNode("activity").InnerText;

                XmlNode met = s.SelectSingleNode("met");
                string metName = met.SelectSingleNode("name").InnerText;
                string metValue = met.SelectSingleNode("value").InnerText;
                XmlNode caloriesNode = s.SelectSingleNode("caloriesValue");
                int caloriesValue = int.Parse(caloriesNode.SelectSingleNode("value").InnerText);
                string unityCal = caloriesNode.SelectSingleNode("unity").InnerText;


                if (caloriesValue > calories && caloriesValue <= limiteMax)
                {
                    lista.Add(new Activity(name, caloriesValue, unityCal, metName, metValue));
                }
            }
            return lista;

        }

        public List<Vegetable> getVegetablesListBycalories(int calories, string unity)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(VEGETABLE_FILEPATH_XML);

            double limiteMax = 0;
            double limiteMin = 0;
            if (calories < 100 && unity.Equals("cal") && calories >= 10)
            {
                if (calories < 20)
                {
                    limiteMax = calories + 3;
                    limiteMin = calories - 3;
                }
                else
                {
                    limiteMax = calories + 10;
                    limiteMin = calories - 10;
                }
            }
            else if (calories >= 100 && unity.Equals("cal") || unity.Equals("kcal"))
            {
                limiteMax = calories + calories / 10;
                limiteMin = calories - calories / 10;
            }
            else if (calories < 10 && unity.Equals("cal"))
            {
                limiteMax = calories + 2;
                limiteMin = calories - 2;
            }

            XmlNodeList nodes = doc.SelectNodes("//food");
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
                XmlNode quantity = s.SelectSingleNode("quantity");
                string quantityValue = quantity.SelectSingleNode("value").InnerText;
                string unityQuantity = quantity.SelectSingleNode("unity").InnerText;
                XmlNode caloriesNode = s.SelectSingleNode("calories");
                int caloriesValue = int.Parse(caloriesNode.SelectSingleNode("value").InnerText);
                string unityCal = caloriesNode.SelectSingleNode("unity").InnerText;


                if ((caloriesValue >= calories && caloriesValue <= limiteMax) ||
                    (caloriesValue <= calories && caloriesValue >= limiteMin))
                {
                    Vegetable veggie = new Vegetable(name, extraInfo, quantityValue, unityQuantity, caloriesValue,
                        unityCal);
                    veggie.Id = id;

                    lista.Add(veggie);
                }
            }
            return lista;
        }

        public Activity GetActivityById(int _id)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(ACTIVITY_FILEPATH_XML);
            XmlNode node = doc.SelectSingleNode("//exercise[@id = " + _id + "]");
            int id = _id;
            string name = node.SelectSingleNode("activity").InnerText;
            string metName = "Metabolic Equivalent";
            int caloriasValue = int.Parse(node.SelectSingleNode("caloriesValue/value").InnerText);
            string caloriasUnit = node.SelectSingleNode("caloriesValue/unity").InnerText;
            string met = node.SelectSingleNode("met/value").InnerText;

            Activity act = new Activity(name, caloriasValue, caloriasUnit, metName, met);
            act.Id = id;

            return act;
        }

        public Plate GetPlateById(int _id)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(PLATE_FILEPATH_XML);
            XmlNode node = doc.SelectSingleNode("//plate[@id = " + _id + "]");
            int id = _id;
            string name = node.SelectSingleNode("item").InnerText;
            string restaurantName = node.SelectSingleNode("parent::restaurant/@name").InnerText;
            string quantityValue = node.SelectSingleNode("quantity/value").InnerText;
            string quantityDosage = node.SelectSingleNode("quantity/dosage").InnerText;
            string quantityExtraDosage = "";
            XmlNode selectSingleNode = node.SelectSingleNode("quantity/extraDosage");
            if (selectSingleNode != null)
            {
                quantityExtraDosage = selectSingleNode.InnerText;
            }
            int caloriesValue = int.Parse(node.SelectSingleNode("calories/value").InnerText);
            string caloriasUnit = node.SelectSingleNode("calories/unity").InnerText;

            Plate plate = new Plate(name, restaurantName, quantityValue, quantityDosage, quantityExtraDosage, caloriesValue, caloriasUnit);
            plate.Id = id;

            return plate;
        }

        public Vegetable GetVegetableById(int _id)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(VEGETABLE_FILEPATH_XML);
            XmlNode node = doc.SelectSingleNode("//food[@id = " + _id + "]");

            List<string> extraInfo = new List<string>();
            int id = _id;
            string name = node.SelectSingleNode("vegetable").InnerText;

            XmlNode nodesExtra = node.SelectSingleNode("extraInfo");
            if (nodesExtra != null)
                extraInfo.Add(nodesExtra.InnerText);
            /*foreach (XmlNode extra in nodesExtra)
            {
                string extraInformacao = extra.SelectSingleNode("extraInfo").InnerText;
                extraInfo.Add(extraInformacao);
            }*/

            string quantityValue = node.SelectSingleNode("quantity/value").InnerText;
            string unityQuantity = node.SelectSingleNode("quantity/unity").InnerText;
            int caloriesValue = int.Parse(node.SelectSingleNode("calories/value").InnerText);
            string unityCal = node.SelectSingleNode("calories/unity").InnerText;

            Vegetable veggie = new Vegetable(name, extraInfo, quantityValue, unityQuantity, caloriesValue, unityCal);
            veggie.Id = id;

            return veggie;
        }

        public bool UpdateActivity(Activity activity, int _id, string token)
        {
            if (checkAuthentication(token))
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(ACTIVITY_FILEPATH_XML);
                XmlNode node = doc.SelectSingleNode("//exercise[@id = " + _id + "]");
                node.SelectSingleNode("activity").InnerText = activity.Nome;
                node.SelectSingleNode("caloriesValue/value").InnerText = activity.CaloriasValue.ToString();
                node.SelectSingleNode("caloriesValue/unity").InnerText = activity.CaloriasUnit;
                node.SelectSingleNode("met/value").InnerText = activity.Met;

                doc.Save(ACTIVITY_FILEPATH_XML);
                return true;
            }
            return false;
        }

        public bool UpdatePlate(Plate plate, int _id, string token)
        {
            if (checkAuthentication(token))
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(PLATE_FILEPATH_XML);
                XmlNode node = doc.SelectSingleNode("//plate[@id = " + _id + "]");
                node.SelectSingleNode("item").InnerText = plate.Name;
                node.SelectSingleNode("parent::restaurant/@name").InnerText = plate.RestaurantName;
                node.SelectSingleNode("quantity/value").InnerText = plate.QuantityValue;
                node.SelectSingleNode("quantity/dosage").InnerText = plate.QuantityDosage;
                if (plate.QuantityExtraDosage != null)
                    node.SelectSingleNode("quantity/extraDosage").InnerText = plate.QuantityExtraDosage;
                node.SelectSingleNode("calories/value").InnerText = plate.CaloriesValue.ToString();
                node.SelectSingleNode("calories/unity").InnerText = plate.CaloriasUnit;

                doc.Save(PLATE_FILEPATH_XML);

                return true;
            }
            return false;
        }

        public bool UpdateVegetable(Vegetable vegetable, int _id, string token)
        {
            if (checkAuthentication(token))
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(VEGETABLE_FILEPATH_XML);
                XmlNode node = doc.SelectSingleNode("//food[@id = " + _id + "]");

                node.SelectSingleNode("vegetable").InnerText = vegetable.Name;
                if (vegetable.ExtraInfo != null)
                {
                    string extra = "";
                    foreach (string extraInfo in vegetable.ExtraInfo)
                    {
                        if (extraInfo.IndexOf(extraInfo) == 0)
                        {
                            extra = extraInfo;
                        }
                        else
                        {
                            extra += ", " + extraInfo;
                        }
                    }

                    XmlNode nodeExtraInfo = node.SelectSingleNode("extraInfo");
                    if (nodeExtraInfo != null)
                    {
                        nodeExtraInfo.InnerText = extra;
                    }
                    else
                    {
                        XmlNode nodeQuatity = node.SelectSingleNode("quantity");
                        XmlElement extraInfoElement = doc.CreateElement("extraInfo");
                        extraInfoElement.InnerText = extra;
                        node.InsertBefore(extraInfoElement, nodeQuatity);
                    }
                }
                node.SelectSingleNode("quantity/value").InnerText = vegetable.QuantityValue;
                node.SelectSingleNode("quantity/unity").InnerText = vegetable.UnityQuantity;
                node.SelectSingleNode("calories/value").InnerText = vegetable.CaloriesValue.ToString();
                node.SelectSingleNode("calories/unity").InnerText = vegetable.UnityCal;

                doc.Save(VEGETABLE_FILEPATH_XML);

                return true;
            }
            return false;
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

            XmlNode node = doc.SelectSingleNode("//user[username = '" + username + "']");
            string verifica = node.SelectSingleNode("@isAdmin").InnerText;

            if (verifica.Equals("true"))
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        private bool checkAuthentication(string token)
        {
            try
            {
                Token tokenObject = tokens[token];
            }
            catch (KeyNotFoundException)
            {
                return false;
            }

            return true;
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

