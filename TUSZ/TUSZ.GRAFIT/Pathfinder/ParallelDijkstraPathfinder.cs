using Hippie;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using TUSZ.Common.GRAFIT;
using TUSZ.GRAFIT.Graph;

namespace TUSZ.GRAFIT.Pathfinder
{
    public class ParallelDijkstraPathfinder : DijkstraPathfinder
    {
        public ParallelDijkstraPathfinder(TransitGraph graph) : base(graph) { }

        private void CalculateNextNodes(Object node)
        {
            lock (node)
            {
                ((DynamicNode)node).GetNextDynamicNodes();
            }
        }

        public override List<Instruction> CalculateShortestRoute(Stop sourceStop, Stop destinationStop, DateTime now)
        {
            var staticMap = new Dictionary<Stop, SortedSet<DynamicNode>>();
            var openSet = HeapFactory.NewBinaryHeap<DynamicNode>();
            var firstDynamicNode = DynamicNode.CreateFirstDynamicNode(this.graph, sourceStop, now);

            staticMap[sourceStop] = new SortedSet<DynamicNode>();
            staticMap[sourceStop].Add(firstDynamicNode);
            openSet.Add(firstDynamicNode);

            while (openSet.Count > 0)
            {
                var currentNode = openSet.RemoveMin();

                if (currentNode.stop == destinationStop)
                {
                    return currentNode.history.instructions.ToList();
                }

                if (currentNode.history.countOfRoutes > graph.maxCountOfRoutes)
                {
                    continue;
                }

                lock (currentNode)
                {
                    foreach (var nextNode in currentNode.GetNextDynamicNodes())
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
                            openSet.Add(nextNode);
                        }
                    }
                }
            }

            throw new NoPathFoundException(sourceStop, destinationStop, now);
        }
    }
}
