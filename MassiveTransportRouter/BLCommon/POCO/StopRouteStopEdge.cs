using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTR.BusinessLogic.Common.POCO
{
    public class StopRouteStopEdge
    {
        public int StopId { get; set; }
        public int RouteId { get; set; }
        public int nextStopId { get; set; }
    }
}
