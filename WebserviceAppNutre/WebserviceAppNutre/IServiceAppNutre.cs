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
        bool addActivity(Activity activity, string token); // admin only

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/addactivityxml?token={token}")]
        void addActivityXML(XmlDocument activitiesXml, string token); // admin only

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/addrestaurant?token={token}")]
        bool addRestaurant(Plate plate, string token); // admin only
       
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/addrestaurantxml?token={token}")]
        void addRestaurantXML(XmlDocument platesXml, string token); // admin only
        
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/addvegetable?token={token}")]
        bool addVegetable(Vegetable vegetable, string token); // admin only

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/addvegetablexml?token={token}")]
        void addVegetableXML(XmlDocument vegetablesXml, string token); // admin only

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "/getactivitieslist")]
        List<Activity> getActivitiesList();

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "/getrestaurantlist")]
        List<Plate> GetRestaurantsList();

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "/getvegetableslist")]
        List<Vegetable> getVegetablesList();

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/removeactivity?id={id}&token={token}")]
        bool removeActivity(int id, string token);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/removeplates?id={id}&token={token}")]
        bool removePlate(int id, string token);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/removevegetables?id={id}&token={token}")]
        bool removeVegetable(int id, string token);


        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "/getcaloriesbyactivity?id={id}")]
        string [] getCaloriesByActivity(int id);

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "/getcaloriesbyveggie?id={id}")]
        string[] getCaloriesByVeggie(int id);

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "/getcaloriesbyplate?id={id}")]
        string[] getCaloriesByPlate(int id);

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "/getactivitieslistbycalories?calories={calories}&unity={unity}")]
        List<Activity> getActivitiesListByCalories(int calories, string unity);

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "/getvegetableslistbycalories?calories={calories}&unity={unity}")]
        List<Vegetable> getVegetablesListBycalories(int calories,string unity);

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "/getactivitybyid?_id={_id}")]
        Activity GetActivityById(int _id);

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "/getplatebyid?_id={_id}")]
        Plate GetPlateById(int _id);

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "/getvegetablebyid?_id={_id}")]
        Vegetable GetVegetableById(int _id);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/getactivitybyid?_id={_id}&token={token}")]
        bool UpdateActivity(Activity activity, int _id, string token);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/getplatebyid?_id={_id}&token={token}")]
        bool UpdatePlate(Plate plate, int _id, string token);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/getvegetablebyid?_id={_id}&token={token}")]
        bool UpdateVegetable(Vegetable vegetable, int _id, string token);

    }

    // Use a data contract as illustrated in the sample below to add composite types to service operations.
    [DataContract]
    public class Activity
    {
        private int id;
        private string nome;
        private string metName;
        private int caloriasValue;
        private string caloriasUnit;
        private string met;

        public Activity(string nome, int caloriasValue, string caloriasUnit,string metName, string met)
        {
            this.nome = nome;
            this.met = met;
            this.caloriasUnit = caloriasUnit;
            this.caloriasValue = caloriasValue;
            this.metName = metName;
        }

        [DataMember]
        public int Id
        {
            get { return id; }
            set { id = value; }
        }


        [DataMember]
        public string Nome
        {
            get { return nome; }
            set { nome = value; }
        }


        [DataMember]
        public string MetName
        {
            get { return metName; }
            set { metName = value; }
        }

        [DataMember]
        public int CaloriasValue
        {
            get { return caloriasValue; }
            set { caloriasValue = value; }
        }

        [DataMember]
        public string CaloriasUnit
        {
            get { return caloriasUnit; }
            set { caloriasUnit = value; }
        }

        [DataMember]
        public string Met
        {
            get { return met; }
            set { met = value; }
        }
    }

    [DataContract]
    public class Plate
    {
        private int id;
        private string name;
        private string restaurantName;
        private string quantityValue;
        private string quantityDosage;
        private string quantityExtraDosage;
        private int caloriesValue;
        private string caloriasUnit;

        public Plate(string name, string restaurantName, string quantityValue, string quantityDosage, string quantityExtraDosage, 
            int caloriesValue, string caloriasUnit)
        {
            this.name = name;
            this.restaurantName = restaurantName;
            this.quantityValue = quantityValue;
            this.quantityDosage = quantityDosage;
            this.quantityExtraDosage = quantityExtraDosage;
            this.caloriesValue = caloriesValue;
            this.caloriasUnit = caloriasUnit;
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
            get { return name; }
            set { name = value; }
        }

        [DataMember]
        public string RestaurantName
        {
            get { return restaurantName; }
            set { restaurantName = value; }
        }

        [DataMember]
        public string QuantityValue
        {
            get { return quantityValue; }
            set { quantityValue = value; }
        }

        [DataMember]
        public string QuantityDosage
        {
            get { return quantityDosage; }
            set { quantityDosage = value; }
        }

        [DataMember]
        public string QuantityExtraDosage
        {
            get { return quantityExtraDosage; }
            set { quantityExtraDosage = value; }
        }

        [DataMember]
        public int CaloriesValue
        {
            get { return caloriesValue; }
            set { caloriesValue = value; }
        }

        [DataMember]
        public string CaloriasUnit
        {
            get { return caloriasUnit; }
            set { caloriasUnit = value; }
        }
    }

    [DataContract]
    public class Vegetable
    {
        private int id;
        private string name;
        private List<string> extraInfo;
        private string quantityValue;
        private string unityQuantity;
        private int caloriesValue;
        private string unityCal;


        public Vegetable(string name, List<string> extraInfo, string quantityValue, string unityQuantity, int caloriesValue,string unityCal)
        {
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
        public string QuantityValue
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
