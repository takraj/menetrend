using MTR.BusinessLogic.Common.POCO;
using MTR.BusinessLogic.DataManager;
using MTR.BusinessLogic.DataTransformer;
using MTR.DataAccess.EFDataManager;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTR.BusinessLogic.Pathfinder.Dijkstra
{
    public class DijkstraPathfinder : PathfinderAlgorithm
    {
        protected bool _parallel = false;

        public DijkstraPathfinder(bool parallel = true)
        {
            Initialize();
            _parallel = parallel;
        }

        public override void GetShortestRoute(int sourceStopId, int destinationStopId, DateTime when)
        {
            Console.WriteLine("Looking for route between '" + GetStop(sourceStopId).StopName + "' and '" + GetStop(destinationStopId).StopName + "'");

            var completeNodes = new List<CompleteNode>();
            var inclompleteNodes = new List<Node>();

            bool traceComplete = false;

            // Kezdetben csak a forráspont van kész, nem vezet hozzá semmi
            completeNodes.Add(new CompleteNode
            {
                stopId = sourceStopId,
                departureTime = when.TimeOfDay,
                viaNode = null,
                usedEdges = new Stack<Edge>(),
                usedRouteIds = new Stack<int>()
            });

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
                Console.WriteLine("Iteration: " + completeNodes.Count + " cnodes, " + inclompleteNodes.Count + " icnodes");
                CompleteNode nextNode = null;

                var lck = new Object();
                var garbage = new ConcurrentBag<CompleteNode>();

                if (_parallel)
                {
                    completeNodes.AsParallel().ForAll(
                    currentNode =>
                    {
                        var candidate = GetNextCompleteNode(currentNode, inclompleteNodes, when);

                        if (candidate != null)
                        {
                            // van kimenő él

                            // atomi művelet!!
                            lock (lck)
                            {
                                if ((nextNode == null) || (candidate.departureTime < nextNode.departureTime))
                                {
                                    nextNode = candidate;
                                }
                            }
                        }
                        else
                        {
                            garbage.Add(currentNode);
                        }
                    });
                }
                else
                {
                    completeNodes.ForEach(
                    currentNode =>
                    {
                        var candidate = GetNextCompleteNode(currentNode, inclompleteNodes, when);

                        if (candidate != null)
                        {
                            // van kimenő él

                            // atomi művelet!!
                            lock (lck)
                            {
                                if ((nextNode == null) || (candidate.departureTime < nextNode.departureTime))
                                {
                                    nextNode = candidate;
                                }
                            }
                        }
                        else
                        {
                            garbage.Add(currentNode);
                        }
                    });
                }

                if (nextNode == null)
                {
                    Console.WriteLine("No route between '" + GetStop(sourceStopId).StopName + "' and '" + GetStop(destinationStopId).StopName + "'");
                    break;
                }

                // Takarítás
                garbage.ToList().ForEach(g => completeNodes.Remove(g));
                garbage = new ConcurrentBag<CompleteNode>();

                // Áthelyezés a kész csomópontok közé
                completeNodes.Add(nextNode);
                var removedNode = inclompleteNodes.Single(n => n.stopId == nextNode.stopId);
                inclompleteNodes.Remove(removedNode);

                // Ha megtaláltuk a célt, akkor leállunk
                if (nextNode.stopId == destinationStopId)
                {
                    traceComplete = true;
                    Console.WriteLine("Trace complete between '" + GetStop(sourceStopId).StopName + "' and '" + GetStop(destinationStopId).StopName + "'");
                    Console.WriteLine("Shortest Route Found. # of used routes: " + nextNode.usedRouteIds.Count + " | arrival: " + nextNode.departureTime.ToString());

                    nextNode.usedEdges.Reverse().ToList().ForEach(
                        e =>
                        {
                            Console.WriteLine(e.GetType().Name + " " + e.ToString());
                        });
                }
            }
        }

        protected List<CompleteNode> GetRouteCandidates(CompleteNode currentNode, List<Node> incompleteNodes, DateTime when)
        {
            var candidates = new List<CompleteNode>();
            var routes = GetRoutes(currentNode.stopId);

            if (routes == null)
            {
                return candidates;
            }

            foreach (var routeId in routes)
            {
                bool isKnownRouteId = currentNode.usedRouteIds.Contains(routeId);
                if (!(isKnownRouteId && (currentNode.usedRouteIds.Peek() != routeId)))
                {
                    // A járatok nem újrafelhasználhatóak!

                    foreach (var stop in GetEndpoints(currentNode.stopId, routeId))
                    {
                        if (!incompleteNodes.Exists(ic => ic.stopId == stop.DbId))
                        {
                            // Csak akkor foglalkozzunk vele, ha még nem dolgoztuk fel
                            continue;
                        }

                        var edge = new RouteEdge(stop.DbId, routeId, when, currentNode.departureTime);
                        var edgeCost = edge.GetCost();

                        if (edgeCost != null)
                        {
                            var usedEdges = new Stack<Edge>(currentNode.usedEdges.Reverse());
                            usedEdges.Push(edge);

                            var usedRouteIds = new Stack<int>(currentNode.usedRouteIds.Reverse());
                            if (!isKnownRouteId)
                            {
                                usedRouteIds.Push(routeId);
                            }

                            if (usedRouteIds.Count <= 5)
                            {
                                candidates.Add(new CompleteNode
                                {
                                    stopId = stop.DbId,
                                    viaNode = currentNode,
                                    departureTime = currentNode.departureTime.Add(new TimeSpan(0, (int)edgeCost, 0)),
                                    usedEdges = usedEdges,
                                    usedRouteIds = new Stack<int>(usedRouteIds.Reverse())
                                });
                            }
                        }
                    }
                }
            }

            return candidates;
        }

        protected List<CompleteNode> GetTransferCandidates(CompleteNode currentNode, List<Node> incompleteNodes, DateTime when)
        {
            var candidates = new List<CompleteNode>();

            int? groupId = GetStop(currentNode.stopId).GroupId;

            if (groupId != null)
            {
                var stopsInGroup = GetStopsInGroup(groupId);
                if (stopsInGroup == null)
                {
                    return candidates;
                }

                foreach (var stop in stopsInGroup)
                {
                    if (stop.DbId == currentNode.stopId)
                    {
                        // Saját magunkkal nem foglalkozunk
                        continue;
                    }

                    if (!incompleteNodes.Exists(ic => ic.stopId == stop.DbId))
                    {
                        // Csak akkor foglalkozzunk vele, ha még nem dolgoztuk fel
                        continue;
                    }

                    var edge = new TransferEdge();
                    var edgeCost = edge.GetCost();

                    if (edgeCost != null)
                    {
                        var usedEdges = new Stack<Edge>(currentNode.usedEdges.Reverse());
                        usedEdges.Push(edge);

                        candidates.Add(new CompleteNode
                        {
                            stopId = stop.DbId,
                            viaNode = currentNode,
                            departureTime = currentNode.departureTime.Add(new TimeSpan(0, (int)edgeCost, 0)),
                            usedEdges = usedEdges,
                            usedRouteIds = new Stack<int>(currentNode.usedRouteIds.Reverse())
                        });
                    }
                }
            }

            return candidates;
        }

        protected virtual CompleteNode GetNextCompleteNode(CompleteNode currentNode, List<Node> incompleteNodes, DateTime when)
        {
            try
            {
                var candidates = new List<CompleteNode>();
                candidates.AddRange(GetRouteCandidates(currentNode, incompleteNodes, when));
                candidates.AddRange(GetTransferCandidates(currentNode, incompleteNodes, when));

                #region A legolcsóbb továbblépés megkeresése
                {
                    var minimalDeparture = candidates.Min(c => c.departureTime);
                    return candidates.First(c => c.departureTime == minimalDeparture);
                }
                #endregion
            }
            catch
            {
                return null;
            }
        }
    }
}
