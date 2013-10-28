using System;
using System.Collections.Generic;
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

        public readonly IStorageManager StorageManager;

        public RoutePlannerService()
        {
            StorageManager = new ZipStorageManager(@"C:\budapest_gtfs");
        }

        public List<Instruction> Plan(Stop source, Stop destination, DateTime when)
        {
            var graph = new TransitGraph(StorageManager);
            var pathfinder = new ParallelAStarPathfinder(graph, StorageManager.GetStopDistanceVector(destination.idx), 500);
            return pathfinder.CalculateShortestRoute(source, destination, when);
        }
    }
}