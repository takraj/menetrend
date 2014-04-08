using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using TransitPlannerContracts;
using TransitPlannerLibrary.FlowerGraphModel;
using TransitPlannerLibrary.PathfinderCore;

namespace TransitPlannerWcfHost.v1
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "SoapService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select SoapService.svc or SoapService.svc.cs at the Solution Explorer and start debugging.
    public class SoapService : ISoapService
    {
        public List<TransitPlannerContracts.TransitStop> GetAllStops()
        {
            return GetStops(string.Empty);
        }

        public List<TransitPlannerContracts.TransitStop> GetStops(string filter)
        {
            var lst = Common.FilterStops(filter);

            if (lst.Count < 1)
            {
                throw new ArgumentException();
            }

            return lst;
        }

        

        public TransitPlannerContracts.TransitStop GetStop(int id)
        {
            try
            {
                return Common.CreateTransitStop(id);
            }
            catch (IndexOutOfRangeException)
            {
                throw new ArgumentException();
            }
        }

        public TransitPlannerContracts.TransitPlan GetPlan(TransitPlannerContracts.TransitPlanRequestParameters parameters)
        {
            var delays = new Dictionary<int, TimeSpan>();
            foreach (var item in parameters.trip_delays)
            {
                delays[item.key] = new TimeSpan(0, item.value, 0);
            }

            var disabled_routes = new HashSet<int>(parameters.disabled_route_ids);
            var max_waiting = new TimeSpan(0, parameters.max_waiting_time, 0);
            var get_on_off_time = new TimeSpan(0, parameters.get_on_off_time, 0);
            var when = new DateTime(parameters.when.year, parameters.when.month, parameters.when.day, parameters.when.hour, parameters.when.minute, 0);

            var graph = new FlowerGraph(Common.repository, disabled_routes, delays, get_on_off_time, max_waiting, parameters.walking_speed, parameters.needs_wheelchair_support);

            try
            {
                Common.repository.GetStopById(parameters.from);
                Common.repository.GetStopById(parameters.to);
            }
            catch (IndexOutOfRangeException)
            {
                throw new ArgumentException();
            }

            if ((when < Common.repository.MetaInfo.MinDate) || (when > Common.repository.MetaInfo.MaxDate))
            {
                throw new ArgumentException();
            }

            var startNode = new WalkingNode(graph, parameters.from);
            var endNode = new WalkingNode(graph, parameters.to);

            DijkstraPathfinderState state = null;

            if (parameters.use_algorithm == Constants.ASTAR_ALGORITHM)
            {
                state = new AStarPathfinderState(graph, startNode, endNode, when);
            }
            else if (parameters.use_algorithm == Constants.DIJKSTRA_ALGORITHM)
            {
                state = new DijkstraPathfinderState(startNode, endNode, when);
            }
            else
            {
                throw new NotImplementedException();
            }

            return Common.CreateTransitPlan(state, graph);
        }

        public TransitPlannerContracts.TransitPlan GetSimplePlan(int from, int to, int year, int month, int day, int hour, int minute)
        {
            var parameters = new TransitPlanRequestParameters
            {
                disabled_route_ids = new List<int>(),
                from = from,
                get_on_off_time = Common.DEFAULT_GET_ON_OFF_TIME,
                max_waiting_time = Common.DEFAULT_MAX_WAITING_TIME,
                needs_wheelchair_support = false,
                to = to,
                trip_delays = new List<IntegerPair>(),
                walking_speed = Common.DEFAULT_WALKING_SPEED,
                use_algorithm = Constants.ASTAR_ALGORITHM,
                when = new TransitDateTime
                {
                    year = year,
                    month = month,
                    day = day,
                    hour = hour,
                    minute = minute
                }
            };

            return GetPlan(parameters);
        }
    }
}
