using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TransitPlannerLibrary.FlowerDataModel
{
    public class Stop
    {
        public string Name;
        public double Latitude, Longitude;
        public List<int> Routes = new List<int>(); // ezt meg kell változtatni Trips-re!
        public List<StopDistance> Distances = new List<StopDistance>();

        public override string ToString()
        {
            return string.Format("Stop(\"{0}\")", this.Name);
        }
    }
}
