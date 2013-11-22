using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using TUSZ.Common.GRAFIT;
using TUSZ.Common.ViewModels;
using TUSZ.GRAFIT.Graph;
using TUSZ.GRAFIT.Pathfinder;
using TUSZ.GRAFIT.Storage;

namespace TUSZ.WebApp.Models
{
    public class RoutePlannerSingleton
    {
        private static RoutePlannerSingleton _instance = null;
        private static Object lck = new Object();

        public static RoutePlannerSingleton Instance
        {
            get
            {
                lock (lck)
                {
                    if (_instance == null)
                    {
                        _instance = new RoutePlannerSingleton();
                    }
                }

                return _instance;
            }
        }

        /////// INSTANCE ///////

        private readonly IStorageManager StorageManager;
        public readonly VM_Stop[] Stops;

        private RoutePlannerSingleton()
        {
            StorageManager = new ZipStorageManager(@"C:\budapest_gtfs", useCaching: true);
            StorageManager.LoadDatabase();
            Stops = StorageManager.Stops.Select(
                s => new VM_Stop
                {
                    id=s.idx,
                    name=s.name,
                    lat = s.position.latitude,
                    lng = s.position.longitude
                }).ToArray();
        }

        public List<Instruction> Plan(Stop source, Stop destination, DateTime when, string algorythm)
        {
            var graph = new TransitGraph(StorageManager);

            IPathfinder pathfinder = null;

            switch (algorythm)
            {
                case "AStar":
                    pathfinder = new AStarPathfinder(graph, StorageManager.GetStopDistanceVector(destination.idx), 1000);
                    break;
                case "SpeedAnalyzerAStar":
                    pathfinder = new SpeedAnalyzerAStarPathfinder(graph, StorageManager.GetStopDistanceVector(destination.idx), 1000);
                    break;
                case "MarkerAStar":
                    pathfinder = new MarkerAStarPathfinder(graph, StorageManager.GetStopDistanceVector(destination.idx), 1000);
                    break;
                case "ParallelAStar":
                    pathfinder = new ParallelAStarPathfinder(graph, StorageManager.GetStopDistanceVector(destination.idx), 1000);
                    break;
                case "Dijkstra":
                    pathfinder = new DijkstraPathfinder(graph);
                    break;
                case "ParallelDijkstra":
                    pathfinder = new ParallelDijkstraPathfinder(graph);
                    break;
                case "SmartParallelAStar":
                    pathfinder = new SmartParallelAStarPathfinder(graph, StorageManager.GetStopDistanceVector(destination.idx), 1000);
                    break;
                case "AgressiveParallelAStar":
                    pathfinder = new AgressiveParallelAStarPathfinder(graph, StorageManager.GetStopDistanceVector(destination.idx), 1000);
                    break;
                case "PrecalculatedDijkstra":
                    pathfinder = new PrecalculatedDijkstraPathfinder(graph);
                    break;
            }

            return pathfinder.CalculateShortestRoute(source, destination, when);
        }

        public VM_Plan Plan(int stopIndex1, int stopIndex2, DateTime when, string algorythm)
        {
            var stop1 = StorageManager.GetStop(stopIndex1);
            var stop2 = StorageManager.GetStop(stopIndex2);

            var beginDate = DateTime.Now;
            var instructions = Plan(stop1, stop2, when, algorythm);

            var travelGroups = new List<VM_TravelGroup>();
            foreach (var instruction in instructions)
            {
                if ((travelGroups.Count < 1) || (travelGroups.Last().route.id != instruction.route.idx))
                {
                    var passedStops = new List<VM_PassedStop>();
                    passedStops.Add(new VM_PassedStop
                        {
                            when = instruction.endDate,
                            stop = new VM_Stop
                            {
                                id = instruction.stop.idx,
                                name = instruction.stop.name,
                                lat = instruction.stop.position.latitude,
                                lng = instruction.stop.position.longitude
                            },
                            getOnOff = !(instruction is TravelAction)
                        });

                    travelGroups.Add(new VM_TravelGroup
                        {
                            from = new VM_Stop
                            {
                                id = instruction.stop.idx,
                                name = instruction.stop.name,
                                lat = instruction.stop.position.latitude,
                                lng = instruction.stop.position.longitude
                            },
                            from_time = instruction.endDate,
                            route = new VM_Route
                            {
                                id = instruction.route.idx,
                                name = instruction.route.name,
                                html_base_color = ColorTranslator.ToHtml(Color.FromArgb(instruction.route.colour.r, instruction.route.colour.g, instruction.route.colour.b)),
                                html_text_color = ColorTranslator.ToHtml(Color.FromArgb(instruction.route.textColour.r, instruction.route.textColour.g, instruction.route.textColour.b))
                            },
                            passed_stops = passedStops
                        });
                }
                else
                {
                    var lastGroup = travelGroups.Last();

                    lastGroup.to = new VM_Stop
                        {
                            id = instruction.stop.idx,
                            name = instruction.stop.name,
                            lat = instruction.stop.position.latitude,
                            lng = instruction.stop.position.longitude
                        };

                    lastGroup.to_time = instruction.endDate;

                    lastGroup.passed_stops.Add(new VM_PassedStop
                    {
                        when = instruction.endDate,
                        stop = new VM_Stop
                        {
                            id = instruction.stop.idx,
                            name = instruction.stop.name,
                            lat = instruction.stop.position.latitude,
                            lng = instruction.stop.position.longitude
                        },
                        getOnOff = !(instruction is TravelAction)
                    });
                }
            }

            return new VM_Plan
            {
                algorythm_name = algorythm,
                plan_time = DateTime.Now - beginDate,
                plan_begins = when,
                plan_ends = instructions.Last().endDate,
                source = new VM_Stop
                {
                    id = stop1.idx,
                    lat = stop1.position.latitude,
                    lng = stop1.position.longitude,
                    name = stop1.name
                },
                destination = new VM_Stop {
                    id = stop2.idx,
                    lat = stop2.position.latitude,
                    lng = stop2.position.longitude,
                    name = stop2.name
                },
                travel_groups = travelGroups
            };
        }
    }
}