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

        public override string ToString()
        {
            try
            {
                return String.Format("{0}: ['{1}' via {2}] {3} ({4}) @ {5}",
                    this.GetType().Name, this.route.name, this.trip.idx, this.stop.name, this.stop.idx, this.endDate);
            }
            catch (Exception)
            {
                return base.ToString();
            }
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
