using Hippie;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace GTFSConverter.CRGTFS.Pathfinder
{
    class ParallelDijkstraPathfinder : DijkstraPathfinder
    {
        public ParallelDijkstraPathfinder(TransitGraph graph) : base(graph) { }

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
            var openSet = HeapFactory.NewBinaryHeap<DynamicNode>();

            var firstDynamicNode = new DynamicNode
            {
                stop = sourceStop,
                onlyTravelActionNextTime = false,
                mustGetOn = false,
                history = new History
                {
                    actions = new List<Action>(),
                    lastUsedRoute = null,
                    usedRoutes = new HashSet<Route>(),
                    totalWalkingTime = 0,
                    totalDistance = 0
                },
                graph = this.graph,
                currentTime = now
            };

            staticMap[sourceStop] = new SortedSet<DynamicNode>();
            staticMap[sourceStop].Add(firstDynamicNode);
            openSet.Add(firstDynamicNode);

            while (openSet.Count > 0)
            {
                var currentNode = openSet.RemoveMin();

                if (currentNode.stop == destinationStop)
                {
                    return currentNode.history.actions.ToList();
                }

                lock (currentNode)
                {
                    foreach (var nextNode in currentNode.GetNextDynamicNodes())
                    {
                        if (nextNode.history.totalWalkingTime > graph.maxWalkingMinutes)
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
                            openSet.Add(nextNode);
                        }
                    }
                }
            }

            throw new Exception("Nem értem el a célt... Valószínűleg nem összefüggő a gráf.");
        }
    }
}
