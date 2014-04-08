using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Security;
using System.Web.SessionState;
using TransitPlannerLibrary.FlowerDataModel;
using TransitPlannerLibrary.FlowerGraphModel;
using TransitPlannerLibrary.PathfinderCore;
using TransitPlannerLibrary.PriorityQueues;

namespace TransitPlannerWcfHost
{
    public class Global : System.Web.HttpApplication
    {
        private const string CONFIG_KEY_TRANSIT_DB_LOCATION = "TransitDatabaseLocation";

        private class InvalidConfigurationException : ArgumentException
        {
            private const string MESSAGE = CONFIG_KEY_TRANSIT_DB_LOCATION + " is not configured in the root configuration file. (Web.config)";

            public InvalidConfigurationException() : base(MESSAGE) { }
        }

        protected void Application_Start(object sender, EventArgs e)
        {
            var transitDataLocation = WebConfigurationManager.AppSettings[CONFIG_KEY_TRANSIT_DB_LOCATION];

            if (transitDataLocation == null)
            {
                throw new InvalidConfigurationException();
            }

            var dataSource = new CsvDataSource(transitDataLocation);
            Common.repository = new MemoryRepository(dataSource);
            Common.pathfinder = new GenericPathfinder<FlowerNode, DateTime, DijkstraPathfinderState, BinaryHeapPriorityQueue<FlowerNode>>();
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}