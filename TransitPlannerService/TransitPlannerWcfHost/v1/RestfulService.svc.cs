using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using TransitPlannerContracts;
using TransitPlannerLibrary.FlowerDataModel;
using TransitPlannerLibrary.FlowerGraphModel;
using TransitPlannerLibrary.PathfinderCore;

namespace TransitPlannerWcfHost
{
    public class RestfulService : IRestfulService
    {
        public IList<TransitStop> GetAllStops()
        {
            return GetStops(string.Empty);
        }

        public IList<TransitStop> GetStops(string filter)
        {
            var lst = Common.FilterStops(filter);

            if (lst.Count < 1)
            {
                if (filter != string.Empty)
                {
                    throw new WebFaultException(HttpStatusCode.NotFound);
                }
                else
                {
                    throw new WebFaultException(HttpStatusCode.NoContent);
                }
            }

            return lst;
        }

        public TransitPlan GetPlan(TransitPlanRequestParameters parameters)
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
                throw new WebFaultException(HttpStatusCode.NotFound);
            }

            if ((when < Common.repository.MetaInfo.MinDate) || (when > Common.repository.MetaInfo.MaxDate))
            {
                throw new WebFaultException(HttpStatusCode.NotFound);
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
                throw new WebFaultException(HttpStatusCode.NotImplemented);
            }

            try
            {
                return Common.CreateTransitPlan(state, graph);
            }
            catch (NoPathFoundException)
            {
                throw new WebFaultException(HttpStatusCode.NoContent);
            }
        }

        public TransitPlan GetSimplePlan(int from, int to, int year, int month, int day, int hour, int minute)
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

        public TransitStopInfo GetStop(int id)
        {
            try
            {
                var transitStop = Common.CreateTransitStop(id);
                return Common.CreateTransitStopInfo(transitStop);
            }
            catch (IndexOutOfRangeException)
            {
                throw new WebFaultException(HttpStatusCode.NotFound);
            }
        }

        public IList<TransitRoute> GetRoutes(string filter)
        {
            var lst = Common.FilterRoutes(filter);

            if (lst.Count < 1)
            {
                if (filter != string.Empty)
                {
                    throw new WebFaultException(HttpStatusCode.NotFound);
                }
                else
                {
                    throw new WebFaultException(HttpStatusCode.NoContent);
                }
            }

            return lst;
        }

        public TransitRoute GetRoute(int id)
        {
            try
            {
                return Common.CreateTransitRoute(id);
            }
            catch (IndexOutOfRangeException)
            {
                throw new WebFaultException(HttpStatusCode.NotFound);
            }
        }

        public TransitMetadata GetMetadata()
        {
            return Common.CreateMetaData();
        }

        public IList<TransitSequenceGroup> GetSchedule(int route_id, int year, int month, int day)
        {
            var when = new TransitDate
            {
                year = year,
                month = month,
                day = day
            };
            try
            {
                return Common.CreateSchedule(route_id, when);
            }
            catch (IndexOutOfRangeException)
            {
                throw new WebFaultException(HttpStatusCode.NotFound);
            }
        }

        public IList<TransitSequenceInfo> GetSequences(int route_id, int year, int month, int day)
        {
            var when = new TransitDate
            {
                year = year,
                month = month,
                day = day
            };
            try
            {
                return Common.CreateTransitSequenceInfoForRoute(route_id, when);
            }
            catch (IndexOutOfRangeException)
            {
                throw new WebFaultException(HttpStatusCode.NotFound);
            }
        }

        public IList<TransitSequenceElement> GetSequence(int id)
        {
            try
            {
                return Common.CreateTransitSequence(id);
            }
            catch (IndexOutOfRangeException)
            {
                throw new WebFaultException(HttpStatusCode.NotFound);
            }
        }
    }
}
