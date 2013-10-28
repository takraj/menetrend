using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUSZ.Common.GRAFIT;

namespace TUSZ.GRAFIT.Graph
{
    public struct History
    {
        public HashSet<Route> usedRoutes;
        public Route lastUsedRoute;
        public List<Instruction> instructions;
        public double totalWalkingTime;
        public int totalDistance;
    }
}
