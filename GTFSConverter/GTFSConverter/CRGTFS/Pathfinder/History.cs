using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTFSConverter.CRGTFS.Pathfinder
{
    public struct History
    {
        public HashSet<Route> usedRoutes;
        public Route lastUsedRoute;
        public List<Action> actions;
        public double totalWalkingTime;
        public int totalDistance;
    }
}
