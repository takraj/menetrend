using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TransitPlannerLibrary.FlowerDataModel
{
    public class Trip
    {
        public string Headsign;
        public int RouteIdx, ServiceIdx, SequenceIdx;
        public int IntervalFrom, IntervalTo, Duration;
        public bool DayOverlap;
        public bool WheelchairAccessible;

        public override string ToString()
        {
            return string.Format("Trip({0} - {1})", this.IntervalFrom.ToString(), this.IntervalTo.ToString());
        }
    }
}
