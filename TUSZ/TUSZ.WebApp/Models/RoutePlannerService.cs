using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using TUSZ.Common.GRAFIT;
using TUSZ.GRAFIT.Graph;
using TUSZ.GRAFIT.Pathfinder;
using TUSZ.GRAFIT.Storage;

namespace TUSZ.WebApp.Models
{
    public class RoutePlannerService
    {
        private static RoutePlannerService _instance = null;
        private static Object lck = new Object();

        public static RoutePlannerService Instance
        {
            get
            {
                lock (lck)
                {
                    if (_instance == null)
                    {
                        _instance = new RoutePlannerService();
                    }
                }

                return _instance;
            }
        }

        /////// INSTANCE ///////

        private readonly IStorageManager StorageManager;
        public readonly VM_Stop[] Stops;

        private RoutePlannerService()
        {
            StorageManager = new ZipStorageManager(@"C:\budapest_gtfs");
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
                    pathfinder = new AStarPathfinder(graph, StorageManager.GetStopDistanceVector(destination.idx), 500);
                    break;
                case "ParallelAStar":
                    pathfinder = new ParallelAStarPathfinder(graph, StorageManager.GetStopDistanceVector(destination.idx), 500);
                    break;
                case "Dijkstra":
                    pathfinder = new DijkstraPathfinder(graph);
                    break;
                case "ParallelDijkstra":
                    pathfinder = new ParallelDijkstraPathfinder(graph);
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

    public class VM_Stop
    {
        public int id;
        public string name;
        public float lat;
        public float lng;
    }

    public class VM_Route
    {
        public int id;
        public string name;
        public string html_text_color;
        public string html_base_color;
    }

    public class VM_PassedStop
    {
        public VM_Stop stop;
        public DateTime when;
        public bool getOnOff;
    }

    public class VM_TravelGroup
    {
        public VM_Route route;

        public VM_Stop from;
        public DateTime from_time;

        public VM_Stop to;
        public DateTime to_time;

        public List<VM_PassedStop> passed_stops;
    }

    public class VM_Plan
    {
        public string algorythm_name;
        public TimeSpan plan_time;

        public DateTime plan_begins;
        public DateTime plan_ends;

        public VM_Stop source;
        public VM_Stop destination;

        public List<VM_TravelGroup> travel_groups;
    }
}