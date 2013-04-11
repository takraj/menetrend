using MTR.BusinessLogic.Common.POCO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTR.BusinessLogic.Pathfinder.Dijkstra
{
    class CompleteNode : Node
    {
        public TimeSpan departureTime;
        public CompleteNode viaNode;
        public Stack<Edge> usedEdges;
        public Stack<int> usedRouteIds;
    }
}
