using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MTR.BusinessLogic.Pathfinder.Dijkstra
{
    public class FastDijkstraPathfinder : DijkstraPathfinder
    {
        private Dictionary<Thread, PriorityQueue> _priorityQueues;

        public FastDijkstraPathfinder()
        {
            Initialize();
            _priorityQueues = new Dictionary<Thread, PriorityQueue>();
        }

        public override List<Edge> GetShortestRoute(int sourceStopId, int destinationStopId, DateTime when)
        {
            if (!_priorityQueues.ContainsKey(Thread.CurrentThread))
            {
                var storage = new BinaryHeapPriorityQueue();
                _priorityQueues.Add(Thread.CurrentThread, storage);
            }

            var result = new List<Edge>();

            #region Dijkstra
            {
                int iterationCount = 0;

                Console.WriteLine("Looking for route between '" + GetStop(sourceStopId).StopName + "' and '" + GetStop(destinationStopId).StopName + "'");

                var inclompleteNodes = new List<Node>();
                bool traceComplete = false;

                // Kezdetben csak a forráspont van kész, nem vezet hozzá semmi
                var currentNode = new CompleteNode
                {
                    stopId = sourceStopId,
                    departureTime = when.TimeOfDay,
                    viaNode = null,
                    usedEdges = new Stack<Edge>(),
                    usedRouteIds = new Stack<int>()
                };

                // Mindenki mást pedig várakozik a kifejtésre
                foreach (var stop in AllStops.Where(s => s.DbId != sourceStopId))
                {
                    inclompleteNodes.Add(new Node
                    {
                        stopId = stop.DbId
                    });
                }

                // Az útvonalkereső hurok
                while ((!traceComplete) && (inclompleteNodes.Count > 0))
                {
                    if ((iterationCount % 25) == 0)
                    {
                        Console.WriteLine("Iteration: " + ((iterationCount / 25) + 1) + " x 25 | with " + inclompleteNodes.Count + " icnodes");
                        iterationCount++;
                    }
                    
                    currentNode = GetNextCompleteNode(currentNode, inclompleteNodes, when);

                    if (currentNode == null)
                    {
                        Console.WriteLine("No route between '" + GetStop(sourceStopId).StopName + "' and '" + GetStop(destinationStopId).StopName + "'");
                        break;
                    }

                    // Áthelyezés a kész csomópontok közé
                    {
                        var removedNode = inclompleteNodes.Single(n => n.stopId == currentNode.stopId);
                        inclompleteNodes.Remove(removedNode);
                    }

                    // Ha megtaláltuk a célt, akkor leállunk
                    if (currentNode.stopId == destinationStopId)
                    {
                        traceComplete = true;
                        Console.WriteLine("Trace complete between '" + GetStop(sourceStopId).StopName + "' and '" + GetStop(destinationStopId).StopName + "'");
                        Console.WriteLine("Shortest path Found. # of used routes: " + currentNode.usedRouteIds.Count + " | arrival: " + currentNode.departureTime.ToString());

                        result.AddRange(currentNode.usedEdges.Reverse());
                        result.ForEach(
                            e =>
                            {
                                Console.WriteLine(e.GetType().Name + " " + e.ToString());
                            });
                    }
                }
            }
            #endregion

            if (_priorityQueues.ContainsKey(Thread.CurrentThread))
            {
                _priorityQueues.Remove(Thread.CurrentThread);
            }

            return result;
        }

        protected override CompleteNode GetNextCompleteNode(CompleteNode currentNode, List<Node> incompleteNodes, DateTime when)
        {
            var queue = _priorityQueues[Thread.CurrentThread];
            var candidates = new List<CompleteNode>();
            
            candidates.AddRange(GetRouteCandidates(currentNode, incompleteNodes, when, null));
            candidates.AddRange(GetTransferCandidates(currentNode, incompleteNodes, when));

            foreach (var cnode in candidates)
            {
                var qnode = queue.GetNode(cnode.stopId);

                if ((qnode == null) || (qnode.departureTime > cnode.departureTime))
                {
                    queue.UpdateNode(cnode);
                }
            }

            return queue.Pop();
        }
    }
}
