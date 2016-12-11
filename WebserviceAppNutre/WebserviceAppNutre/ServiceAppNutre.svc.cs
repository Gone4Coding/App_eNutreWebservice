using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Xml;

namespace WebserviceAppNutre
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class ServiceAppNutre : IServiceAppNutre
    {
        public void SignUp(User user, string token)
        {
            throw new NotImplementedException();
        }

        public void LogIn(string username, string password)
        {
            throw new NotImplementedException();
        }

        public void LogOut(string token)
        {
            throw new NotImplementedException();
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
