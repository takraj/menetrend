using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTR.BusinessLogic.Common.POCO
{
    public class StopTime
    {
        public int DbId;
        public int tripId;
        public int stopId;

        public TimeSpan ArrivalTime { get; set; }
        public TimeSpan DepartureTime { get; set; }

        public int StopSequence { get; set; }
        public double ShapeDistanceTraveled { get; set; }

        public StopTime(int DbId, Int32 tripId, Int32 stopId, TimeSpan arrival, TimeSpan departure, Int32 sequence, Double distance)
        {
            this.DbId = DbId;
            this.tripId = tripId;
            this.stopId = stopId;
            this.ArrivalTime = arrival;
            this.DepartureTime = departure;
            this.StopSequence = sequence;
            this.ShapeDistanceTraveled = distance;
        }
    }
}
