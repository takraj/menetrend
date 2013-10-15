using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTFSConverter.CRGTFS.Pathfinder
{
    public class AStarPathfinder
    {
        private TransitGraphWrapper graph;

        public AStarPathfinder(TransitDB tdb)
        {
            graph = new TransitGraphWrapper(tdb);
        }

        public List<Edge> GetShortestRoute(Stop fromStop, Stop toStop, DateTime when)
        {
            return null;
        }
    }
}
