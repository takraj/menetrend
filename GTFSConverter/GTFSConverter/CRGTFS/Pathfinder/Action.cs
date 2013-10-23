using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTFSConverter.CRGTFS.Pathfinder
{
    public abstract class Action
    {
        public DateTime startDate;
        public DateTime endDate;
        public Stop stop;
        public Trip trip;
        public Route route;

        public TimeSpan Cost {
            get { return (endDate - startDate); }
        }
    }

    public class TravelAction : Action
    {
        public StopTime fromStopTime;
        public StopTime toStopTime;
    }

    public class GetOnAction : TravelAction { }
    public class GetOffAction : Action { }
}
