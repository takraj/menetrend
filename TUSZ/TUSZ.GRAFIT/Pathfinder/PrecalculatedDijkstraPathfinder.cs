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

            public TransferTree GetAllPossibleSequences(Stop sourceStop, Stop destinationStop)
            {
                var result = new TransferTree();

                // Közvetlen járatok
                foreach (var routeIdx in sourceStop.knownRoutes)
                {
                    var route = graph.GetRouteByIndex(routeIdx);

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

                        if (neighbourStop.knownRoutes == null)
                        {
                            continue;
                        }

                        foreach (var routeIdx in neighbourStop.knownRoutes)
                        {
                            var route = graph.GetRouteByIndex(routeIdx);

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
                        BuildTree(result, subject.history);
                    }
                }

                return result;
            }

            protected void BuildTree(TransferTree tree, IEnumerable<Route> path)
            {
                var subTree = tree.CreateOrGetTree(path.First());
                if (path.Count() > 1)
                {
                    BuildTree(subTree, path.Skip(1));
                }
            }

            protected void TryAddNewNode(Route route, List<Route> history, HashSet<Route> visited)
            {
                if (!visited.Contains(route))
                {
                    var subHistory = new List<Route>(history);
                    subHistory.Add(route);
                    this.fifo.Enqueue(new RouteNode
                    {
                        history = subHistory,
                        route = route
                    });
                }
            }

            protected bool LBFS(Route subjectRoute, Stop targetStop, List<Route> history)
            {
                // Már megnéztem? (nem igazán ad releváns megoldást)
                if (!this.visitedRoutes.Add(subjectRoute))
                {
                    return false;
                }

                // Limit elérve?
                if (graph.maxCountOfRoutes < history.Count)
                {
                    return false;
                }
                
                var transferMap = graph.GetTransferMap(subjectRoute);

                // Ez egy jó járat?
                if (transferMap.reachableStopIdxs.Contains(targetStop.idx))
                {
                    return true;
                }

                // Van még értelme mélyebbre menni?
                if (graph.maxCountOfRoutes == history.Count)
                {
                    return false;
                }

                var visited = this.visitedRoutes;
                //var visited = new HashSet<Route>(history);

                // OK, meggyőztél, menjünk tovább...
                foreach (var routeIdx in transferMap.reachableRouteIdxs)
                {
                    var route = graph.GetRouteByIndex(routeIdx);
                    TryAddNewNode(route, history, visited);
                }

                return false;
            }
        }

        public override List<Instruction> CalculateShortestRoute(Stop sourceStop, Stop destinationStop, DateTime now)
        {
            var result = new List<Instruction>();
            var tree = new Presearcher(graph).GetAllPossibleSequences(sourceStop, destinationStop);
            
            result = CalculateShortestRouteOnTree(sourceStop, destinationStop, now, tree);
            return result;
        }

        protected virtual List<Instruction> CalculateShortestRouteOnTree(Stop sourceStop, Stop destinationStop, DateTime now, TransferTree tree)
        {
            var staticMap = new Dictionary<Stop, SortedSet<DynamicNode>>();
            var openSet = HeapFactory.NewBinaryHeap<DynamicNode>();
            var firstDynamicNode = DynamicNode.CreateFirstDynamicNode(this.graph, sourceStop, now);
            firstDynamicNode.transferTree = tree;

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

                    if (staticMap[nextNode.stop].Add(nextNode))
                    {
                        openSet.Add(nextNode);
                    }
                }
            }

            throw new NoPathFoundException(sourceStop, destinationStop, now);
        }
    }
}
