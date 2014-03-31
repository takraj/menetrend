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
        public List<TransitStop> GetAllStops()
        {
            return GetStops(string.Empty);
        }

        public List<TransitStop> GetStops(string filter)
        {
            var lst = new List<TransitStop>();
            var filterLower = filter.ToLower();

            for (int i = 0; i < CommonData.repository.Stops.Count(); i++)
            {
                var current = CommonData.repository.GetStopById(i);

                string currentNameLower = current.Name.ToLower();
                string currentPostalCodeLower = current.PostalCode.ToLower();
                string currentCityLower = current.City.ToLower();
                string currentStreetLower = current.Street.ToLower();

                bool nameMatches = (currentNameLower.Contains(filterLower));
                bool postalCodeMatches = (currentPostalCodeLower.Contains(filterLower));
                bool cityMatches = (currentCityLower.Contains(filterLower));
                bool streetMatches = (currentStreetLower.Contains(filterLower));

                if (nameMatches || postalCodeMatches || cityMatches || streetMatches)
                {
                    var stop = CreateTransitStop(i);
                    lst.Add(stop);
                }
            }

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

        public TransitStop GetStop(int id)
        {
            try
            {
                return CreateTransitStop(id);
            }
            catch (IndexOutOfRangeException)
            {
                throw new WebFaultException(HttpStatusCode.NotFound);
            }
        }

        private static TransitStop CreateTransitStop(int id)
        {
            var current = CommonData.repository.GetStopById(id);

            return new TransitStop
            {
                name = current.Name,
                latitude = current.Latitude,
                longitude = current.Longitude,
                id = id,
                city = current.City,
                has_wheelchair_support = current.HasWheelchairSupport,
                postal_code = current.PostalCode,
                street = current.Street
            };
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

            var graph = new FlowerGraph(CommonData.repository, disabled_routes, delays, get_on_off_time, max_waiting, parameters.walking_speed, parameters.needs_wheelchair_support);

            try
            {
                CommonData.repository.GetStopById(parameters.from);
                CommonData.repository.GetStopById(parameters.to);
            }
            catch (IndexOutOfRangeException)
            {
                throw new WebFaultException(HttpStatusCode.NotFound);
            }

            if ((when < CommonData.repository.MetaInfo.MinDate) || (when > CommonData.repository.MetaInfo.MaxDate))
            {
                throw new WebFaultException(HttpStatusCode.NotFound);
            }

            var startNode = new WalkingNode(graph, parameters.from);
            var endNode = new WalkingNode(graph, parameters.to);

            var state = new AStarPathfinderState(graph, startNode, endNode, when); // legyen kívülről állítható majd...

            try
            {
                return CreateTransitPlan(state, graph);
            }
            catch (NoPathFoundException)
            {
                throw new WebFaultException(HttpStatusCode.NoContent);
            }
        }

        public TransitPlan GetSimplePlan(int from, int to, int year, int month, int day, int hour, int minute)
        {
            const int DEFAULT_GET_ON_OFF_TIME = 1; // ezek is legyenek majd kívülről állíthatóak
            const int DEFAULT_MAX_WAITING_TIME = 30;
            const double DEFAULT_WALKING_SPEED = 5.3;

            var parameters = new TransitPlanRequestParameters
            {
                disabled_route_ids = new List<int>(),
                from = from,
                get_on_off_time = DEFAULT_GET_ON_OFF_TIME,
                max_waiting_time = DEFAULT_MAX_WAITING_TIME,
                needs_wheelchair_support = false,
                to = to,
                trip_delays = new List<IntegerPair>(),
                walking_speed = DEFAULT_WALKING_SPEED,
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

        private TransitPlan CreateTransitPlan(DijkstraPathfinderState state, FlowerGraph graph)
        {
            const int INVALID_INDEX = -1;

            var sw = new Stopwatch();
            sw.Start();
            var path = CommonData.pathfinder.GetPath(state).ToArray();
            sw.Stop();

            var result = new TransitPlan();
            result.plan_computation_time = sw.ElapsedMilliseconds / 1000.0 / 60.0;
            result.instructions = new List<TransitPlanInstruction>();

            if (state is AStarPathfinderState)
            {
                result.algorithm = "astar";
            }
            else
            {
                result.algorithm = "dijkstra";
            }

            var first_entry = path.First();
            result.base_time = new TransitDateTime
            {
                year = first_entry.Value.Year,
                month = first_entry.Value.Month,
                day = first_entry.Value.Day,
                hour = first_entry.Value.Hour,
                minute = first_entry.Value.Minute
            };

            var last_entry = path.Last();
            result.end_time = new TransitDateTime
            {
                year = last_entry.Value.Year,
                month = last_entry.Value.Month,
                day = last_entry.Value.Day,
                hour = last_entry.Value.Hour,
                minute = last_entry.Value.Minute
            };

            result.route_duration = (int)(last_entry.Value - first_entry.Value).TotalMinutes;
            result.route_length = 0.0;
            int lastStopId = INVALID_INDEX;

            var used_route_ids = new HashSet<int>();
            var used_trip_ids = new HashSet<int>();

            foreach (var item in path)
            {
                var node = item.Key;
                var time = item.Value;

                if (lastStopId == INVALID_INDEX)
                {
                    lastStopId = node.StopId;
                }
                else
                {
                    result.route_length += graph.GetDistanceBetween(lastStopId, node.StopId);
                }

                if (node is WalkingNode)
                {
                    var instruction = new TransitPlanInstruction
                    {
                        is_walking = true,
                        plan_minute = (int)(time - first_entry.Value).TotalMinutes,
                        stop = CreateTransitStop(node.StopId),
                        trip_id = INVALID_INDEX,
                        route_id = INVALID_INDEX
                    };

                    result.instructions.Add(instruction);
                }
                else if (node is TravellingNode)
                {
                    var tn = node as TravellingNode;
                    var trip = graph.Repository.GetTripById(tn.TripId);

                    var instruction = new TransitPlanInstruction
                    {
                        is_walking = false,
                        plan_minute = (int)(time - first_entry.Value).TotalMinutes,
                        stop = CreateTransitStop(node.StopId),
                        route_id = trip.RouteIdx,
                        trip_id = tn.TripId
                    };

                    result.instructions.Add(instruction);
                    used_route_ids.Add(trip.RouteIdx);
                    used_trip_ids.Add(tn.TripId);
                }
            }

            result.used_trips = new List<TransitTrip>();
            result.used_routes = new List<TransitRoute>();

            foreach (var i in used_route_ids)
            {
                var route = graph.Repository.GetRouteById(i);
                var transit_route = new TransitRoute
                {
                    Description = route.Description,
                    id = i,
                    LongName = route.LongName,
                    RouteColor = route.RouteColor,
                    RouteTextColor = route.RouteTextColor,
                    RouteType = route.RouteType,
                    ShortName = route.ShortName
                };

                result.used_routes.Add(transit_route);
            }

            foreach (var i in used_trip_ids)
            {
                var trip = graph.Repository.GetTripById(i);
                var transit_trip = new TransitTrip
                {
                    headsign = trip.Headsign,
                    id = i,
                    route_id = trip.RouteIdx,
                    sequence_id = trip.SequenceIdx
                };

                result.used_trips.Add(transit_trip);
            }

            return result;
        }
    }
}
