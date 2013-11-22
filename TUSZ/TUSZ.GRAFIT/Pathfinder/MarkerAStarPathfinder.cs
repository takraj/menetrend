using Hippie;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUSZ.Common.GRAFIT;
using TUSZ.GRAFIT.Graph;

namespace TUSZ.GRAFIT.Pathfinder
{
    public class MarkerAStarPathfinder : AStarPathfinder
    {
        public MarkerAStarPathfinder(TransitGraph graph, int[] stopDistances, int fScale = 2000) : base(graph, stopDistances, fScale)
        { }

        public override List<Instruction> CalculateShortestRoute(Common.GRAFIT.Stop sourceStop, Common.GRAFIT.Stop destinationStop, DateTime now)
        {
            var marker = new Dictionary<string, DateTime>();
            var openSet = HeapFactory.NewBinaryHeap<DynamicNode, long>();
            var firstDynamicNode = DynamicNode.CreateFirstDynamicNode(this.graph, sourceStop, destinationStop, now);

            openSet.Add(firstDynamicNode, fValue(firstDynamicNode, sourceStop, destinationStop, now));

            while (openSet.Count > 0)
            {
                var currentNode = openSet.RemoveMin();

                if (currentNode.Value.stop == destinationStop)
                {
                    return currentNode.Value.history.instructions.ToList();
                }

                if (currentNode.Value.history.countOfRoutes > graph.maxCountOfRoutes)
                {
                    continue;
                }

                foreach (var nextNode in currentNode.Value.GetNextDynamicNodes())
                {
                    if (nextNode.history.totalWalkingTime > graph.maxTotalWalkingMinutes)
                    {
                        continue;
                    }
                    
                    if (nextNode.CurrentTrip != null)
                    {
                        string key = String.Concat("stop_", nextNode.stop.idx, "-route_", nextNode.CurrentTrip.routeIndex);

                        if (marker.ContainsKey(key) && (marker[key] <= nextNode.currentTime))
                        {
                            continue;
                        }

                        marker[key] = nextNode.currentTime;
                    }
                    
                    openSet.Add(nextNode, fValue(nextNode, sourceStop, destinationStop, now));
                }
            }

            throw new NoPathFoundException(sourceStop, destinationStop, now);
        }
    }
}
