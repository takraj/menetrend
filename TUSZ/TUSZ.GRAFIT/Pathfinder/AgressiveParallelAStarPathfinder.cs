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
    public class AgressiveParallelAStarPathfinder : ParallelAStarPathfinder
    {
        public AgressiveParallelAStarPathfinder(TransitGraph graph, int[] stopDistances, int fScale = 2000)
            : base(graph, stopDistances, fScale) { }

        /// <summary>
        /// NEM A LEGRÖVIDEBB UTAT KERESI! Az utolsó járatra optimalizál.
        /// </summary>
        /// <param name="sourceStop"></param>
        /// <param name="destinationStop"></param>
        /// <param name="now"></param>
        /// <returns></returns>
        public override List<Instruction> CalculateShortestRoute(Stop sourceStop, Stop destinationStop, DateTime now)
        {
            var staticMap = new Dictionary<Stop, SortedSet<DynamicNode>>();
            var openSet = HeapFactory.NewBinaryHeap<DynamicNode, long>();
            var firstDynamicNode = DynamicNode.CreateFirstDynamicNode(this.graph, sourceStop, destinationStop, now);
            var unfoldedTrips = new HashSet<int>();

            staticMap[sourceStop] = new SortedSet<DynamicNode>();
            staticMap[sourceStop].Add(firstDynamicNode);
            openSet.Add(firstDynamicNode, fValue(firstDynamicNode, sourceStop, destinationStop, now));

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

                #region Agresszív rész
                // -------------------
                if ((currentNode.Value.CurrentTrip != null)
                    && !unfoldedTrips.Contains(currentNode.Value.CurrentTrip.idx)
                    && currentNode.Value.history.lastInstruction is TravelAction)
                {
                    unfoldedTrips.Add(currentNode.Value.CurrentTrip.idx);
                    DynamicNode byTravel = currentNode.Value.GetNextDynamicNodeByTravelAction((TravelAction)currentNode.Value.history.lastInstruction);
                    var byTransfers = new List<DynamicNode>();

                    while (byTravel != null)
                    {
                        if (byTravel.stop == destinationStop)
                        {
                            return byTravel.history.instructions.ToList();
                        }

                        foreach (var transfer in byTravel.GetNextDynamicNodesByWalkAction(byTravel.history.lastInstruction))
                        {
                            if (transfer.stop == destinationStop)
                            {
                                byTransfers.Add(transfer);
                            }
                        }

                        byTravel = byTravel.GetNextDynamicNodeByTravelAction((TravelAction)byTravel.history.lastInstruction);
                    }

                    if (byTransfers.Count > 0)
                    {
                        return byTransfers.Min().history.instructions.ToList();
                    }
                }
                // -------------------
                #endregion

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
                            var f = fValue(nextNode, sourceStop, destinationStop, now);
                            openSet.Add(nextNode, f);
                        }
                    }
                }
            }

            throw new NoPathFoundException(sourceStop, destinationStop, now);
        }
    }
}
