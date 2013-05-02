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
        public DijkstraPathfinder()
        {
            Initialize();
        }

        public override List<Edge> GetShortestRoute(int sourceStopId, int destinationStopId, DateTime when)
        {
            Console.WriteLine("Looking for route between '" + GetStop(sourceStopId).StopName + "' and '" + GetStop(destinationStopId).StopName + "'");

            var result = new List<Edge>();
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

                #region Következő csomópont keresése
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
                #endregion

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
                    Console.WriteLine("Shortest path Found. # of used routes: " + nextNode.usedRouteIds.Count + " | arrival: " + nextNode.departureTime.ToString());

                    result.AddRange(nextNode.usedEdges.Reverse());
                    result.ToList().ForEach(
                        e =>
                        {
                            Console.WriteLine(e.GetType().Name + " " + e.ToString());
                        });
                }
            }

            return result;
        }

        /// <summary>
        /// currentNode-ból járművel elérhető csomópontok és azok költsége (incompleteNodes-ból válogatva)
        /// </summary>
        /// <param name="currentNode"></param>
        /// <param name="incompleteNodes"></param>
        /// <param name="when"></param>
        /// <returns></returns>
        protected List<CompleteNode> GetRouteCandidates(CompleteNode currentNode, List<Node> incompleteNodes, DateTime when, int? limitOfRoutes = 5)
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

                            #region Check for hidden transfers
                            {
                                // Ha ugyanannál a megállónál száll át...
                                if ((usedEdges.Count > 0) && (usedEdges.Peek() is RouteEdge))
                                {
                                    if (((RouteEdge)usedEdges.Peek()).RouteId != edge.RouteId)
                                    {
                                        usedEdges.Push(new TransferEdge(edge.GetDestinationStopId(), currentNode.departureTime));
                                        edgeCost += usedEdges.Peek().GetCost();
                                    }
                                }
                            }
                            #endregion

                            usedEdges.Push(edge);

                            var usedRouteIds = new Stack<int>(currentNode.usedRouteIds.Reverse());
                            if (!isKnownRouteId)
                            {
                                usedRouteIds.Push(routeId);
                            }

                            if ((limitOfRoutes == null) || (usedRouteIds.Count <= limitOfRoutes))
                            {
                                candidates.Add(new CompleteNode
                                {
                                    stopId = stop.DbId,
                                    viaNode = currentNode,
                                    departureTime = currentNode.departureTime.Add(TimeSpan.FromMinutes((int)edgeCost)),
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

        /// <summary>
        /// currentNode-ból gyaloglással elérhető csomópontok és azok költsége (incompleteNodes-ból válogatva)
        /// </summary>
        /// <param name="currentNode"></param>
        /// <param name="incompleteNodes"></param>
        /// <param name="when"></param>
        /// <returns></returns>
        protected List<CompleteNode> GetTransferCandidates(CompleteNode currentNode, List<Node> incompleteNodes, DateTime when)
        {
            var candidates = new List<CompleteNode>();

            if ((currentNode.usedEdges.Count > 0) && (currentNode.usedEdges.Peek() is TransferEdge))
            {
                // Ha az előző művelet átszállás volt, akkor ne szálljunk át mégegyszer
                return candidates;
            }

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

                    var edge = new TransferEdge(stop.DbId, currentNode.departureTime, this.GetStop(currentNode.stopId), this.GetStop(stop.DbId));
                    var edgeCost = edge.GetCost();

                    if (edgeCost != null)
                    {
                        var usedEdges = new Stack<Edge>(currentNode.usedEdges.Reverse());
                        usedEdges.Push(edge);

                        candidates.Add(new CompleteNode
                        {
                            stopId = stop.DbId,
                            viaNode = currentNode,
                            departureTime = currentNode.departureTime.Add(TimeSpan.FromMinutes((int)edgeCost)),
                            usedEdges = usedEdges,
                            usedRouteIds = new Stack<int>(currentNode.usedRouteIds.Reverse())
                        });
                    }
                }
            }

            return candidates;
        }

        /// <summary>
        /// Új kifejtett csomópont választása
        /// </summary>
        /// <param name="currentNode"></param>
        /// <param name="incompleteNodes"></param>
        /// <param name="when"></param>
        /// <returns></returns>
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
