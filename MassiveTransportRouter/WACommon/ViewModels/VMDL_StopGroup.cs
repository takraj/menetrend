using MTR.DataAccess.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTR.WebApp.Common.ViewModels
{
    public class VMDL_StopGroup
    {
        private List<VMDL_Stop> stops;
        private bool hasDifferentNames;
        
        public double avgLatitude;
        public double avgLongitude;
        public string name;

        public bool HasDifferentNames
        {
            get
            {
                return hasDifferentNames;
            }
        }

        public List<VMDL_Stop> GetStops()
        {
            return new List<VMDL_Stop>(stops);
        }

        public VMDL_StopGroup(List<VMDL_Stop> stops, bool hasDifferentNames, double avgLatitude, double avgLongitude, string name)
        {
            this.stops = stops;
            this.hasDifferentNames = hasDifferentNames;
            this.avgLatitude = avgLatitude;
            this.avgLongitude = avgLongitude;
            this.name = name;
        }
    }
}
