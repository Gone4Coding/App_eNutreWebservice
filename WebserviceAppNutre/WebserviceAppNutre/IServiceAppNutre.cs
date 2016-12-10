using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace WebserviceAppNutre
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IServiceAppNutre
    {
        [OperationContract]
        void addActivity(Activity activity, string token);

        [OperationContract]
        void addRestaurant(Restaurant restaurant, string token);

        [OperationContract]
        void addVegetable(Vegetable vegetable, string token);
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

        public string Nome
        {
            get { return nome; }
            set { nome = value; }
        }

        public int Calorias
        {
            get { return calorias; }
            set { calorias = value; }
        }

        public decimal Met
        {
            get { return met; }
            set { met = value; }
        }
    }

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

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public string Item
        {
            get { return item; }
            set { item = value; }
        }

        public string Quantity
        {
            get { return quantity; }
            set { quantity = value; }
        }

        public int Calories
        {
            get { return calories; }
            set { calories = value; }
        }
    }

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

        public string getName()
        {
            return name;
        }

        public string getExtraInfo()
        {
            return extraInfo;
        }

        public string getQuantity()
        {
            return quantity;
        }
    }
}
