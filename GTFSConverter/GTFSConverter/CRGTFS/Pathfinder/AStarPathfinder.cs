using Hippie;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTFSConverter.CRGTFS.Pathfinder
{
    public class AStarPathfinder : IPathfinder
    {
        private TransitGraph graph;
        private int[] stopDistances;

        public AStarPathfinder(TransitGraph graph, int[] stopDistances)
        {
            this.graph = graph;
            this.stopDistances = stopDistances;
        }

        protected long fValue(DynamicNode node, Stop destinationStop)
        {
            long walkingCost = stopDistances[node.stop.idx] / 1000; // 60 kmph
            return node.currentTime.AddMinutes(walkingCost).Ticks;
        }

        protected long fValue(DynamicNode node, Stop destinationStop, DateTime epoch)
        {
            long walkingCost = stopDistances[node.stop.idx];
            walkingCost += node.history.totalDistance;

            return walkingCost;
        }

        public List<Action> CalculateShortestRoute(Stop sourceStop, Stop destinationStop, DateTime now)
        {
            var staticMap = new Dictionary<Stop, SortedSet<DynamicNode>>();
            var openSet = HeapFactory.NewBinaryHeap<DynamicNode, long>();

            var firstDynamicNode = new DynamicNode
            {
                stop = sourceStop,
                onlyTravelActionNextTime = false,
                history = new History
                {
                    actions = new List<Action>(),
                    lastUsedRoute = null,
                    usedRoutes = new HashSet<Route>(),
                    totalWalkingTime = 0,
                    totalDistance = 0
                },
                graph = this.graph,
                currentTime = now,
                mustGetOn = false
            };

            staticMap[sourceStop] = new SortedSet<DynamicNode>();
            staticMap[sourceStop].Add(firstDynamicNode);
            openSet.Add(firstDynamicNode, fValue(firstDynamicNode, destinationStop));

            while (openSet.Count > 0)
            {
                var currentNode = openSet.RemoveMin();

                if (currentNode.Value.stop == destinationStop)
                {
                    return currentNode.Value.history.actions.ToList();
                }

                foreach (var nextNode in currentNode.Value.GetNextDynamicNodes())
                {
                    if (nextNode.history.totalWalkingTime > 10)
                    {
                        continue;
                    }

                    if (staticMap.ContainsKey(nextNode.stop))
                    {
                        if (staticMap[nextNode.stop].Min.CompareTo(nextNode) < 1)
                        {
                            nextNode.onlyTravelActionNextTime = true;
                        }
                    }
                    else
                    {
                        staticMap[nextNode.stop] = new SortedSet<DynamicNode>();
                    }

                    if (staticMap[nextNode.stop].Add(nextNode))
                    {
                        openSet.Add(nextNode, fValue(nextNode, destinationStop));
                    }
                }
            }

            throw new Exception("Nem értem el a célt... Valószínűleg nem összefüggő a gráf.");
        }
    }
}
