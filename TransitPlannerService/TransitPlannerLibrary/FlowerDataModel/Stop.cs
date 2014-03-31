using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TransitPlannerLibrary.FlowerDataModel
{
    public class Stop
    {
        public string Name, PostalCode, City, Street;
        public double Latitude, Longitude;
        public bool HasWheelchairSupport;
        public List<int> Routes = new List<int>(); // ezt meg kell változtatni Trips-re!
        public List<StopDistance> Distances = new List<StopDistance>();

        public override string ToString()
        {
            return string.Format("Stop(\"{0}\")", this.Name);
        }
    }
}
