using MTR.Common.Gtfs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTR.BusinessLogic.Common.POCO
{
    public class Trip
    {
        public Int32 DbId;
        public String OriginalId;

        public int RouteId;
        public int ServiceId;
        public int ShapeId;

        public String TripHeadsign;
        public E_TripDirection DirectionId;
        public String BlockId;
        public E_WheelchairSupport WheelchairAccessible;
    }
}
