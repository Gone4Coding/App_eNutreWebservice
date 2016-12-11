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
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IServiceAppNutre
    {
        [OperationContract]
        void SignUp(User user, string token); // admin only

        [OperationContract]
        void LogIn(string username, string password);

        [OperationContract]
        void LogOut(string token);

        [OperationContract]
        void addActivity(Activity activity, string token); // admin only

        [OperationContract]
        void addActivity(XmlDocument activitiesXml, string token); // admin only

        [OperationContract]
        void addRestaurant(Restaurant restaurant, string token); // admin only

        [OperationContract]
        void addRestaurant(XmlDocument restaurantsXml, string token); // admin only

        [OperationContract]
        void addVegetable(Vegetable vegetable, string token); // admin only

        [OperationContract]
        void addVegetable(XmlDocument vegetablesXml, string token); // admin only

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "/listing/activities")]
        List<Activity> getActivitiesList();

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "/listing/restaurants")]
        List<Restaurant> GetRestaurantsList();

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "/listing/vegetables")]
        List<Vegetable> getVegetablesList();


    }

    // Use a data contract as illustrated in the sample below to add composite types to service operations.
    [DataContract]
    public class Activity
    {
        private string nome;
        private int calorias;
        private decimal met;

        public Activity(string nome, int calorias, decimal met)
        {
            this.nome = nome;
            this.met = met;
            this.calorias = calorias;
        }
        
        [DataMember]
        public string Nome
        {
            get { return nome; }
            set { nome = value; }
        }

        [DataMember]
        public int Calorias
        {
            get { return calorias; }
            set { calorias = value; }
        }

        [DataMember]
        public decimal Met
        {
            get { return met; }
            set { met = value; }
        }
    }

    [DataContract]
    public class Restaurant
    {
        private string name;
        private string item;
        private string quantity;
        private int calories;

        public Restaurant(string name, string item, string quantity, int calories)
        {
            this.name = name;
            this.item = item;
            this.quantity = quantity;
            this.calories = calories;
        }

        [DataMember]
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        [DataMember]
        public string Item
        {
            get { return item; }
            set { item = value; }
        }

        [DataMember]
        public string Quantity
        {
            get { return quantity; }
            set { quantity = value; }
        }

        [DataMember]
        public int Calories
        {
            get { return calories; }
            set { calories = value; }
        }
    }

    [DataContract]
    public class Vegetable
    {
        private string name;
        private string extraInfo;
        private string quantity;
        private string calories;

        public Vegetable(string name, string quantity, string calories)
        {
            this.name = name;
            this.quantity = quantity;
            this.calories = calories;
        }

        public Vegetable(string name, string extraInfo, string quantity, string calories)
        {
            this.name = name;
            this.quantity = quantity;
            this.calories = calories;
            this.extraInfo = extraInfo;
        }

        [DataMember]
        public string Name
        {
            get{ return name; }
            set{ name = value; }
        }

        [DataMember]
        public string ExtraInfo
        {
            get{ return extraInfo; }
            set{ extraInfo = value; }
        }

        [DataMember]
        public string Quantity
        {
            get{ return quantity; }
            set{ quantity = value; }
        }

        [DataMember]
        public string Calories
        {
            get{ return calories; }
            set{ calories = value; }
        }
    }

    [DataContract]
    public class User
    {
        protected int id;
        protected string username;
        protected string password;
        protected bool admin;
        protected static string FILEPATH = Path.Combine(HostingEnvironment.ApplicationPhysicalPath, "App_Data", "users.xml");

        public User(string username, string password, bool admin)
        {
            this.admin = admin;
            this.username = username;
            this.password = password;
            this.id = getValidId();
        }

        [DataMember]
        public bool Admin
        {
            get { return admin; }
            set { admin = value; }
        }

        [DataMember]
        public string Username
        {
            get { return username; }
            set { username = value; }
        }

        [DataMember]
        public string Password
        {
            get { return password; }
            set { password = value; }
        }

        [DataMember]
        public int Id
        {
            get { return id; }
        }

        private int getValidId()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(FILEPATH);

            XmlNodeList idList = doc.SelectNodes("//@id");
            return idList.Count + 1;
        }
         
    }
    
}
