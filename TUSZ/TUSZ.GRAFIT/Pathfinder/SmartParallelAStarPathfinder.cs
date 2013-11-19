using Hippie;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TUSZ.Common.GRAFIT;
using TUSZ.GRAFIT.Graph;

namespace TUSZ.GRAFIT.Pathfinder
{
    public class SmartParallelAStarPathfinder : ParallelAStarPathfinder
    {
        public SmartParallelAStarPathfinder(TransitGraph graph, int[] stopDistances, int fScale = 2000)
            : base(graph, stopDistances, fScale) { }

        public override List<Instruction> CalculateShortestRoute(Stop sourceStop, Stop destinationStop, DateTime now)
        {
            var staticMap = new Dictionary<Stop, SortedSet<DynamicNode>>();
            var openSet = HeapFactory.NewBinaryHeap<DynamicNode, long>();
            var firstDynamicNode = DynamicNode.CreateFirstDynamicNode(this.graph, sourceStop, destinationStop, now);

            staticMap[sourceStop] = new SortedSet<DynamicNode>();
            staticMap[sourceStop].Add(firstDynamicNode);
            openSet.Add(firstDynamicNode, fValue(firstDynamicNode, destinationStop, now));

            while (openSet.Count > 0)
            {
                var currentNode = openSet.RemoveMin();

                if (openSet.Count > 0)
                {
                    ThreadPool.QueueUserWorkItem(CalculateNextNodes, openSet.Min.Value);
                }

                if (currentNode.Value.stop == destinationStop)
                {
                    return currentNode.Value.history.instructions.ToList();
                }

                if (currentNode.Value.history.countOfRoutes > graph.maxCountOfRoutes)
                {
                    continue;
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

                        if (staticMap[nextNode.stop].Add(nextNode))
                        {
                            var f = fValue(nextNode, destinationStop, now);
                            openSet.Add(nextNode, f);
                        }
                    }
                }
            }

            throw new NoPathFoundException(sourceStop, destinationStop, now);
        }
    }
}
