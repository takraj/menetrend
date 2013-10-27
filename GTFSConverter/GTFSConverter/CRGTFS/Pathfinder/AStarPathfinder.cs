﻿using Hippie;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTFSConverter.CRGTFS.Pathfinder
{
    public class AStarPathfinder : IPathfinder
    {
        protected TransitGraph graph;
        protected int[] stopDistances;
        protected int fScale; // TRUE: felülről becsül, FALSE: alulról becsül

        public AStarPathfinder(TransitGraph graph, int[] stopDistances, int fScale = 2000)
        {
            this.graph = graph;
            this.stopDistances = stopDistances;
            this.fScale = fScale;
        }

        protected virtual long fValue(DynamicNode node, Stop destinationStop, DateTime epoch)
        {
            long remainingDistance = stopDistances[node.stop.idx];
            return node.currentTime.AddMinutes(remainingDistance / fScale).Ticks;
        }

        public virtual List<Action> CalculateShortestRoute(Stop sourceStop, Stop destinationStop, DateTime now)
        {
            var staticMap = new Dictionary<Stop, SortedSet<DynamicNode>>();
            var openSet = HeapFactory.NewBinaryHeap<DynamicNode, long>();

            var firstDynamicNode = new DynamicNode
            {
                stop = sourceStop,
                onlyTravelActionNextTime = false,
                history = new History
                {
                    actions = new List<Action>(),
                    lastUsedRoute = null,
                    usedRoutes = new HashSet<Route>(),
                    totalWalkingTime = 0,
                    totalDistance = 0
                },
                graph = this.graph,
                currentTime = now,
                mustGetOn = false
            };

            staticMap[sourceStop] = new SortedSet<DynamicNode>();
            staticMap[sourceStop].Add(firstDynamicNode);
            openSet.Add(firstDynamicNode, fValue(firstDynamicNode, destinationStop, now));

            while (openSet.Count > 0)
            {
                var currentNode = openSet.RemoveMin();

                if (currentNode.Value.stop == destinationStop)
                {
                    return currentNode.Value.history.actions.ToList();
                }

                foreach (var nextNode in currentNode.Value.GetNextDynamicNodes())
                {
                    if (nextNode.history.totalWalkingTime > 10)
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
                        openSet.Add(nextNode, fValue(nextNode, destinationStop, now));
                    }
                }
            }

            throw new Exception("Nem értem el a célt... Valószínűleg nem összefüggő a gráf.");
        }
    }
}
