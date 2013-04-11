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
    public class DijkstraPathfinder
    {
        private Dictionary<int, Stop> _allStops;                            // StopId -> Stop
        private Dictionary<int, Dictionary<int, List<int>>> _graphMap;      // StopId -> RouteId -> nextStopId
        private Dictionary<int, List<int>> _stopGroups;                     // GroupId -> StopId

        public DijkstraPathfinder() {
            var dbStops = DbManager.GetAllStops();

            _allStops = dbStops.ToDictionary(s => s.DbId);
            _graphMap = GraphTransformer.GetGraphMap();
            _stopGroups = new Dictionary<int, List<int>>();

            // StopGroup asszociatív lista inicializációja
            foreach (var stop in dbStops.Where(s => s.GroupId != null))
            {
                if (!_stopGroups.ContainsKey((int)stop.GroupId))
                {
                    _stopGroups.Add((int)stop.GroupId, new List<int>());
                }

                List<int> values;
                _stopGroups.TryGetValue((int)stop.GroupId, out values);
                values.Add(stop.DbId);
            }
        }

        private Stop GetStopById(int stopId)
        {
            Stop s;
            _allStops.TryGetValue(stopId, out s);
            return s;
        }

        public void GetShortestRoute(int sourceStopId, int destinationStopId, DateTime when)
        {
            Console.WriteLine("Looking for route between '" + GetStopById(sourceStopId).StopName + "' and '" + GetStopById(destinationStopId).StopName + "'");

            var completeNodes = new List<CompleteNode>();
            var inclompleteNodes = new List<Node>();
            
            bool traceComplete = false;

            // Kezdetben csak a forráspont van kész, nem vezet hozzá semmi
            completeNodes.Add(new CompleteNode {
                stopId = sourceStopId,
                departureTime = when.TimeOfDay,
                viaNode = null,
                usedEdges = new Stack<Edge>(),
                usedRouteIds = new Stack<int>()
            });

            // Mindenki mást pedig várakozik a kifejtésre
            foreach (var stop in _allStops.Values.Where(s => s.DbId != sourceStopId))
            {
                inclompleteNodes.Add(new Node
                {
                    stopId = stop.DbId
                });
            }

            while ((!traceComplete) && (inclompleteNodes.Count > 0))
            {
                Console.WriteLine("Iteration: " + completeNodes.Count + " cnodes, " + inclompleteNodes.Count + " icnodes");
                CompleteNode nextNode = null;

                var lck = new Object();
                var garbage = new ConcurrentBag<CompleteNode>();

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

                if (nextNode == null)
                {
                    Console.WriteLine("No route between '" + GetStopById(sourceStopId).StopName + "' and '" + GetStopById(destinationStopId).StopName + "'");
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
                    Console.WriteLine("Trace complete between '" + GetStopById(sourceStopId).StopName + "' and '" + GetStopById(destinationStopId).StopName + "'");
                    Console.WriteLine("Shortest Route Found. # of used routes: " + nextNode.usedRouteIds.Count + " | arrival: " + nextNode.departureTime.ToString());

                    nextNode.usedEdges.Reverse().ToList().ForEach(
                        e => {
                            Console.WriteLine(e.GetType().Name + " " + e.ToString());
                        });
                }
            }
        }

        private CompleteNode GetNextCompleteNode(CompleteNode currentNode, List<Node> incompleteNodes, DateTime when)
        {
            try
            {
                var candidates = new List<CompleteNode>();

                Dictionary<int, List<int>> stopsOfRoutes;
                _graphMap.TryGetValue(currentNode.stopId, out stopsOfRoutes);

                #region Élek keresése útvonalak alapján (parallel)
                stopsOfRoutes.Keys.ToList().ForEach(
                    routeId =>
                    {
                        bool isKnownRouteId = currentNode.usedRouteIds.Contains(routeId);
                        if (!(isKnownRouteId && (currentNode.usedRouteIds.Peek() != routeId)))
                        {
                            // A járatok nem újrafelhasználhatóak

                            List<int> stops;
                            stopsOfRoutes.TryGetValue(routeId, out stops);

                            stops.ForEach(
                                stopId =>
                                {
                                    if (incompleteNodes.Exists(ic => ic.stopId == stopId))
                                    {
                                        // Csak akkor foglalkozzunk vele, ha még nem dolgoztuk fel

                                        var edge = new RouteEdge(stopId, routeId, when, currentNode.departureTime);
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
                                                    stopId = stopId,
                                                    viaNode = currentNode,
                                                    departureTime = currentNode.departureTime.Add(new TimeSpan(0, (int)edgeCost, 0)),
                                                    usedEdges = usedEdges,
                                                    usedRouteIds = new Stack<int>(usedRouteIds.Reverse())
                                                });
                                            }
                                        }
                                    }
                                });
                        }
                    });
                #endregion

                #region Élek keresése átszállás alapján (parallel)
                int? groupId = GetStopById(currentNode.stopId).GroupId;

                if (groupId != null)
                {
                    List<int> stopsInSameGroup = null;
                    _stopGroups.TryGetValue((int)groupId, out stopsInSameGroup);

                    if (stopsInSameGroup != null)
                    {
                        stopsInSameGroup.Where(id => id != currentNode.stopId).ToList().ForEach(
                            stopId =>
                            {
                                if (incompleteNodes.Exists(ic => ic.stopId == stopId))
                                {
                                    // Csak akkor foglalkozzunk vele, ha még nem dolgoztuk fel

                                    var edge = new TransferEdge();
                                    var edgeCost = edge.GetCost();

                                    if (edgeCost != null)
                                    {
                                        var usedEdges = new Stack<Edge>(currentNode.usedEdges.Reverse());
                                        usedEdges.Push(edge);

                                        candidates.Add(new CompleteNode
                                        {
                                            stopId = stopId,
                                            viaNode = currentNode,
                                            departureTime = currentNode.departureTime.Add(new TimeSpan(0, (int)edgeCost, 0)),
                                            usedEdges = usedEdges,
                                            usedRouteIds = new Stack<int>(currentNode.usedRouteIds.Reverse())
                                        });
                                    }
                                }
                            });
                    }
                }
                #endregion

                #region A legolcsóbb továbblépés megkeresése
                    var minimalDeparture = candidates.Min(c => c.departureTime);
                    return candidates.First(c => c.departureTime == minimalDeparture);
                #endregion
            }
            catch
            {
                return null;
            }
        }
    }
}
