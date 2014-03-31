using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TransitPlannerLibrary.FlowerDataModel;
using TransitPlannerLibrary.FlowerGraphModel;
using TransitPlannerLibrary.PathfinderCore;

namespace TransitPlannerWcfHost
{
    public abstract class CommonData
    {
        public static IRepository repository = null;
        public static IPathfinder<FlowerNode, DateTime, DijkstraPathfinderState> pathfinder = null;
    }
}