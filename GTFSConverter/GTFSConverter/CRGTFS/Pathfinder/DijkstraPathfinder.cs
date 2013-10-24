using Hippie;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTFSConverter.CRGTFS.Pathfinder
{
    public class DijkstraPathfinder : IPathfinder
    {
        private TransitGraph graph;

        public DijkstraPathfinder(TransitGraph graph)
        {
            this.graph = graph;
        }

        public List<Action> CalculateShortestRoute(Stop sourceStop, Stop destinationStop, DateTime now)
        {
            var staticMap = new Dictionary<Stop, HashSet<DynamicNode>>();
            var closedSet = new HashSet<DynamicNode>();
            var openSet = HeapFactory.NewBinaryHeap<DynamicNode, DateTime>();

            var firstDynamicNode = new DynamicNode
            {
                stop = sourceStop,
                onlyTravelActionNextTime = false,
                history = new History
                {
                    actions = new List<Action>(),
                    lastUsedRoute = null,
                    usedRoutes = new HashSet<Route>(),
                    totalWalkingTime = 0
                },
                graph = this.graph,
                currentTime = now
            };

            staticMap[sourceStop] = new HashSet<DynamicNode>();
            staticMap[sourceStop].Add(firstDynamicNode);
            openSet.Add(firstDynamicNode, now);

            while (openSet.Count > 0)
            {
                var currentNode = openSet.RemoveMin().Value;
                closedSet.Add(currentNode);

                if (currentNode.stop == destinationStop)
                {
                    return currentNode.history.actions.ToList();
                }

                foreach (var nextNode in currentNode.GetNextDynamicNodes())
                {
                    if (closedSet.Contains(nextNode))
                    {
                        continue;
                    }

                    if (nextNode.history.totalWalkingTime > 10)
                    {
                        continue;
                    }

                    if (staticMap.ContainsKey(nextNode.stop))
                    {
                        foreach (var dyNode in staticMap[nextNode.stop])
                        {
                            if (dyNode.currentTime <= nextNode.currentTime)
                            {
                                nextNode.onlyTravelActionNextTime = true;
                                break;
                            }
                        }
                    }
                    else
                    {
                        staticMap[nextNode.stop] = new HashSet<DynamicNode>();
                    }

                    if (staticMap[nextNode.stop].Add(nextNode))
                    {
                        openSet.Add(nextNode, nextNode.currentTime);
                    }
                }
            }

            throw new Exception("Nem értem el a célt... Valószínűleg nem összefüggő a gráf.");
        }
    }
}
