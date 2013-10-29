﻿using Hippie;
using System;
using System.Collections.Generic;
using System.Linq;
using TUSZ.Common.GRAFIT;
using TUSZ.GRAFIT.Graph;

namespace TUSZ.GRAFIT.Pathfinder
{
    public class DijkstraPathfinder : IPathfinder
    {
        protected TransitGraph graph;

        public DijkstraPathfinder(TransitGraph graph)
        {
            this.graph = graph;
        }

        public virtual List<Instruction> CalculateShortestRoute(Stop sourceStop, Stop destinationStop, DateTime now)
        {
            var staticMap = new Dictionary<Stop, SortedSet<DynamicNode>>();
            var openSet = HeapFactory.NewBinaryHeap<DynamicNode>();

            var firstDynamicNode = new DynamicNode
            {
                stop = sourceStop,
                onlyTravelActionNextTime = false,
                history = new History
                {
                    instructions = new List<Instruction>(),
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
            openSet.Add(firstDynamicNode);

            while (openSet.Count > 0)
            {
                var currentNode = openSet.RemoveMin();

                if (currentNode.stop == destinationStop)
                {
                    return currentNode.history.instructions.ToList();
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

            throw new Exception("Nem értem el a célt... Valószínűleg nem összefüggő a gráf.");
        }
    }
}