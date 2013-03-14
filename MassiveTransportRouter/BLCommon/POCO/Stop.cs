using MTR.Common.Gtfs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTR.BusinessLogic.Common.POCO
{
    public class Stop
    {
        public String StopId;
        public String StopName;
        public Double StopLatitude;
        public Double StopLongitude;
        public E_LocationType LocationType;
        public String ParentStation;
        public E_WheelchairSupport WheelchairBoarding;

        public Stop(string id, string name, double lat, double lon, E_LocationType loctype, String parentStation, E_WheelchairSupport wheelchair)
        {
            this.StopId = id;
            this.StopName = name;
            this.StopLatitude = lat;
            this.StopLongitude = lon;
            this.LocationType = loctype;
            this.ParentStation = parentStation;
            this.WheelchairBoarding = wheelchair;
        }
    }
}
