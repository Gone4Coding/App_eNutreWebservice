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
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IServiceAppNutre
    {
       
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/signup?token={token}")]
        void SignUp(User user, string token); // admin only

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/login?username={username}&password={password}")]
        string LogIn(string username, string password);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/logout")]
        void LogOut(string token);
       
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/addactivity?token={token}")]
        void addActivity(Activity activity, string token); // admin only

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/addactivityxml?token={token}")]
        void addActivityXML(XmlDocument activitiesXml, string token); // admin only

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/addrestaurant?token={token}")]
        void addRestaurant(Restaurant restaurant, string token); // admin only
       
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/addrestaurantxml?token={token}")]
        void addRestaurantXML(XmlDocument restaurantsXml, string token); // admin only
        
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/addvegetable?token={token}")]
        void addVegetable(Vegetable vegetable, string token); // admin only

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/addvegetablexml?token={token}")]
        void addVegetableXML(XmlDocument vegetablesXml, string token); // admin only

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "/getactivitieslist")]
        List<Activity> getActivitiesList();

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "/getrestaurantlist")]
        List<Restaurant> GetRestaurantsList();

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "/getvegetableslist")]
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
        private List<string> extraInfo;
        private double quantityValue;
        private string unityQuantity;
        private int caloriesValue;
        private string unityCal;
        private int id;


        public Vegetable(int id,string name, List<string> extraInfo, double quantityValue, string unityQuantity, int caloriesValue,string unityCal)
        {
            this.id = id;
            this.name = name;
            this.extraInfo = extraInfo;
            this.quantityValue = quantityValue;
            this.unityQuantity = unityQuantity;
            this.caloriesValue = caloriesValue;
            this.unityCal = unityCal;
        }

        [DataMember]
        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        [DataMember]
        public string Name
        {
            get{ return name; }
            set{ name = value; }
        }

        [DataMember]
        public List<string> ExtraInfo
        {
            get{ return extraInfo; }
            set{ extraInfo = value; }
        }

        [DataMember]
        public double QuantityValue
        {
            get{ return quantityValue; }
            set{ quantityValue = value; }
        }

        [DataMember]
        public string UnityQuantity
        {
            get{ return unityQuantity; }
            set{ unityQuantity = value; }
        }

        [DataMember]
        public int CaloriesValue
        {
            get { return caloriesValue; }
            set { caloriesValue = value; }
        }

        [DataMember]
        public string UnityCal
        {
            get { return unityCal; }
            set { unityCal = value; }
        }
    }

    [DataContract]
    public class User
    {
        protected string username;
        protected string password;
        protected bool admin;

        public User(string username, string password, bool admin)
        {
            this.admin = admin;
            this.username = username;
            this.password = password;
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
    }
}
