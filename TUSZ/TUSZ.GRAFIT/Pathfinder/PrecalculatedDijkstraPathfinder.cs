using Hippie;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUSZ.Common.GRAFIT;
using TUSZ.GRAFIT.Graph;

namespace TUSZ.GRAFIT.Pathfinder
{
    public class PrecalculatedDijkstraPathfinder : DijkstraPathfinder
    {
        public PrecalculatedDijkstraPathfinder(TransitGraph graph) : base(graph)
        { }

        protected class Presearcher
        {
            private HashSet<Route> visitedRoutes;
            private Queue<RouteNode> fifo;
            private TransitGraph graph;

            public Presearcher(TransitGraph graph)
            {
                this.visitedRoutes = new HashSet<Route>();
                this.graph = graph;
                this.fifo = new Queue<RouteNode>();
            }

            protected struct RouteNode
            {
                public Route route;
                public List<Route> history;
            }

            public List<List<Route>> GetAllPossibleSequences(Stop sourceStop, Stop destinationStop)
            {
                var result = new List<List<Route>>();

                // Közvetlen járatok
                foreach (var route in sourceStop.knownRoutes.Select(r => graph.GetRouteByIndex(r)))
                {
                    this.fifo.Enqueue(new RouteNode
                        {
                            history = new Route[] { route }.ToList(),
                            route = route
                        });
                }

                // Át kell sétálni egy másik megállóba
                for (int i = 0; i < (sourceStop.nearbyStops.Length / 2); i++)
                {
                    if (sourceStop.nearbyStops[i * 2 + 1] <= graph.maxWalkingDistancePerChange)
                    {
                        var neighbourStop = graph.GetStopByIndex(sourceStop.nearbyStops[i * 2]);

                        foreach (var route in sourceStop.knownRoutes.Select(r => graph.GetRouteByIndex(r)))
                        {
                            this.fifo.Enqueue(new RouteNode
                            {
                                history = new Route[] { route }.ToList(),
                                route = route
                            });
                        }
                    }
                }

                // Keresési ciklus
                while (this.fifo.Count > 0)
                {
                    var subject = this.fifo.Dequeue();
                    bool possiblePath = LBFS(subject.route, destinationStop, subject.history);
                    if (possiblePath)
                    {
                        result.Add(subject.history);
                    }
                }

                return result;
            }

            protected bool LBFS(Route subject, Stop target, List<Route> history)
            {
                // Már megnéztem?
                if (this.visitedRoutes.Add(subject))
                {
                    return false;
                }

                var visitedRouteIdxs = new HashSet<int>(this.visitedRoutes.Select(h => h.idx));
                var toExamine = new HashSet<int>();

                // Limit elérve?
                if (graph.maxCountOfRoutes < history.Count)
                {
                    return false;
                }

                // Ez egy jó járat?
                if (subject.knownStops.Contains(target.idx))
                {
                    return true;
                }

                foreach (var stop in subject.knownStops.Select(idx => graph.GetStopByIndex(idx)))
                {
                    // A cél gyaloglási távolságra van?
                    for (int i = 0; i < (stop.nearbyStops.Length / 2); i++)
                    {
                        if (stop.nearbyStops[i * 2 + 1] <= graph.maxWalkingDistancePerChange)
                        {
                            if (stop.nearbyStops[i * 2] == target.idx)
                            {
                                return true;
                            }
                        }
                    }
                    
                    // Van még értelme mélyebbre menni?
                    if (graph.maxCountOfRoutes == history.Count)
                    {
                        return false;
                    }

                    // közvetlen szomszédok feltérképezése
                    foreach (var routeIdx in stop.knownRoutes)
                    {
                        if (!toExamine.Contains(routeIdx) && !visitedRouteIdxs.Contains(routeIdx))
                        {
                            toExamine.Add(routeIdx);
                        }
                    }

                    // átszálló szomszédok feltérképezése
                    for (int i = 0; i < (stop.nearbyStops.Length / 2); i++)
                    {
                        if (stop.nearbyStops[i * 2 + 1] <= graph.maxWalkingDistancePerChange)
                        {
                            var neighbourStop = graph.GetStopByIndex(stop.nearbyStops[i * 2]);

                            if (neighbourStop.knownRoutes == null)
                            {
                                continue;
                            }

                            foreach (var routeIdx in neighbourStop.knownRoutes)
                            {
                                if (!toExamine.Contains(routeIdx) && !visitedRouteIdxs.Contains(routeIdx))
                                {
                                    toExamine.Add(routeIdx);
                                }
                            }
                        }
                    }
                }

                // Új node-ok felvétele a fifo-ba
                foreach (var route in toExamine.Select(r => graph.GetRouteByIndex(r)))
                {
                    if (this.visitedRoutes.Contains(route))
                    {
                        continue;
                    }

                    var subHistory = new List<Route>(history);
                    subHistory.Add(route);
                    this.fifo.Enqueue(new RouteNode
                        {
                            history = subHistory,
                            route = route
                        });
                }

                return false;
            }
        }

        public override List<Instruction> CalculateShortestRoute(Stop sourceStop, Stop destinationStop, DateTime now)
        {
            var result = new List<List<Instruction>>();
            var allPossibleSequences = new Presearcher(graph).GetAllPossibleSequences(sourceStop, destinationStop);

            foreach (var seq in allPossibleSequences)
            {
                var toAdd = CalculateShortestRouteOnSequence(sourceStop, destinationStop, now, seq);
                if (toAdd == null)
                {
                    continue;
                }

                result.Add(toAdd);
            }

            if (result.Count < 1)
            {
                throw new Exception("Nem értem el a célt... Valószínűleg nem összefüggő a gráf.");
            }

            return result.OrderBy(r => r.Last().endDate).First();
        }

        protected virtual List<Instruction> CalculateShortestRouteOnSequence(Stop sourceStop, Stop destinationStop, DateTime now, List<Route> allowedSequence)
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

                    var instructionRoutes = nextNode.history.instructions.Select(ins => ins.route).Distinct().ToArray();

                    if (!CheckSequence(allowedSequence, instructionRoutes))
                    {
                        continue;
                    }

                    if (staticMap[nextNode.stop].Add(nextNode))
                    {
                        openSet.Add(nextNode);
                    }
                }
            }

            return null;
        }

        private bool CheckSequence(List<Route> allowedSequence, Route[] instructionRoutes)
        {
            if (instructionRoutes.Length > allowedSequence.Count)
            {
                return false;
            }

            for (int i = 0; i < instructionRoutes.Length; i++)
            {
                if (allowedSequence.ElementAt(i) != instructionRoutes[i])
                {
                    return false;
                }
            }

            return true;
        }
    }
}
