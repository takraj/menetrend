using Hippie;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GTFSConverter.CRGTFS.Pathfinder
{
    public class ParallelAStarPathfinder : AStarPathfinder
    {
        public ParallelAStarPathfinder(TransitGraph graph, int[] stopDistances, int fScale = 2000)
            : base(graph, stopDistances, fScale) { }

        private void CalculateNextNodes(Object node)
        {
            lock (node)
            {
                ((DynamicNode)node).GetNextDynamicNodes();
            }
        }

        public override List<Action> CalculateShortestRoute(Stop sourceStop, Stop destinationStop, DateTime now)
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
            openSet.Add(firstDynamicNode, fValue(firstDynamicNode, destinationStop, now));

            while (openSet.Count > 0)
            {
                var currentNode = openSet.RemoveMin();

                if (currentNode.Value.stop == destinationStop)
                {
                    return currentNode.Value.history.actions.ToList();
                }

                lock (currentNode.Value)
                {
                    foreach (var nextNode in currentNode.Value.GetNextDynamicNodes())
                    {
                        if (nextNode.history.totalWalkingTime > graph.maxTotalWalkingMinutes)
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

                        ThreadPool.QueueUserWorkItem(CalculateNextNodes, nextNode);

                        if (staticMap[nextNode.stop].Add(nextNode))
                        {
                            openSet.Add(nextNode, fValue(nextNode, destinationStop, now));
                        }
                    }
                }
            }

            throw new Exception("Nem értem el a célt... Valószínűleg nem összefüggő a gráf.");
        }
    }
}
