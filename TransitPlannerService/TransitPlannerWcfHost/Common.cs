using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using TransitPlannerContracts;
using TransitPlannerLibrary.FlowerDataModel;
using TransitPlannerLibrary.FlowerGraphModel;
using TransitPlannerLibrary.PathfinderCore;

namespace TransitPlannerWcfHost
{
    public abstract class Common
    {
        public const int DEFAULT_GET_ON_OFF_TIME = 1;
        public const int DEFAULT_MAX_WAITING_TIME = 30;
        public const double DEFAULT_WALKING_SPEED = 5.3;

        public static IRepository repository = null;
        public static IPathfinder<FlowerNode, DateTime, DijkstraPathfinderState> pathfinder = null;

        public static TransitStop CreateTransitStop(int id)
        {
            var current = repository.GetStopById(id);

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

        public static TransitRoute CreateTransitRoute(int id)
        {
            var current = repository.GetRouteById(id);

            return new TransitRoute
            {
                Description = current.Description,
                id = id,
                LongName = current.LongName,
                RouteColor = current.RouteColor,
                RouteTextColor = current.RouteTextColor,
                RouteType = current.RouteType,
                ShortName = current.ShortName
            };
        }

        public static TransitStopInfo CreateTransitStopInfo(TransitStop transitStop)
        {
            var availableTripIds = (from sequenceId in repository.GetSequencesByStop(transitStop.id)
                                    select repository.GetTripsBySequence(sequenceId)).SelectMany(x => x).Distinct();

            var availableRouteIds = (from tripId in availableTripIds
                                     select repository.GetTripById(tripId).RouteIdx).Distinct();

            return new TransitStopInfo
            {
                stop = transitStop,
                available_routes = (from routeId in availableRouteIds
                                    select CreateTransitRoute(routeId)).ToList()
            };
        }

        public static List<TransitSequenceElement> CreateTransitSequence(int sequenceId)
        {
            var result = new List<TransitSequenceElement>();

            var sequence = Common.repository.GetSequenceById(sequenceId).ToList();
            for (int i = 0; i < sequence.Count(); i++)
            {
                var sequence_element = new TransitSequenceElement()
                {
                    order = i,
                    stop = Common.CreateTransitStop(sequence[i].StopIdx),
                    arrival = sequence[i].ArrivalTime,
                    departure = sequence[i].DepartureTime
                };
                result.Add(sequence_element);
            }

            return result;
        }

        public static List<TransitSequenceInfo> CreateTransitSequenceInfoForRoute(int routeId, TransitDate when)
        {
            var result = new List<TransitSequenceInfo>();

            var route = Common.repository.GetRouteById(routeId);

            foreach (var sequenceId in route.TripsBySequence.Keys)
            {
                int count_of_trips_available_on_this_day = route.TripsBySequence[sequenceId]
                    .Select(tripid => Common.repository.GetTripById(tripid))
                    .Count(trip => Common.repository.IsServiceAvailableOnDay(trip.ServiceIdx, Common.GetServiceDay(when)));

                if (count_of_trips_available_on_this_day < 1)
                {
                    continue;
                }

                var sequence = Common.repository.GetSequenceById(sequenceId).ToList();
                var trip_sample = route.TripsBySequence[sequenceId].First();

                var sequence_info = new TransitSequenceInfo()
                {
                    id = sequenceId,
                    headsign = Common.repository.GetTripById(trip_sample).Headsign,
                    count_of_stops = sequence.Count(),
                    first_stop = Common.CreateTransitStop(sequence.First().StopIdx),
                    last_stop = Common.CreateTransitStop(sequence.Last().StopIdx)
                };
                result.Add(sequence_info);
            }

            return result;
        }

        public static TransitPlan CreateTransitPlan(DijkstraPathfinderState state, FlowerGraph graph)
        {
            const int INVALID_INDEX = -1;

            var sw = new Stopwatch();
            sw.Start();
            var path = Common.pathfinder.GetPath(state).ToArray();
            sw.Stop();

            var result = new TransitPlan();
            result.plan_computation_time = sw.ElapsedMilliseconds / 1000.0 / 60.0;
            result.instructions = new List<TransitPlanInstruction>();

            if (state is AStarPathfinderState)
            {
                result.algorithm = Constants.ASTAR_ALGORITHM;
            }
            else
            {
                result.algorithm = Constants.DIJKSTRA_ALGORITHM;
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
                        stop = Common.CreateTransitStop(node.StopId),
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
                        stop = Common.CreateTransitStop(node.StopId),
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

        public static List<TransitStop> FilterStops(string filter)
        {
            var lst = new List<TransitStop>();
            var filterLower = (filter ?? "").ToLower();

            for (int i = 0; i < Common.repository.Stops.Count(); i++)
            {
                var current = Common.repository.GetStopById(i);

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
                    var stop = Common.CreateTransitStop(i);
                    lst.Add(stop);
                }
            }
            return lst;
        }

        public static List<TransitRoute> FilterRoutes(string filter)
        {
            var lst = new List<TransitRoute>();
            var filterLower = (filter ?? "").ToLower();

            for (int i = 0; i < Common.repository.Routes.Count(); i++)
            {
                var current = Common.repository.GetRouteById(i);

                string currentShortNameLower = current.ShortName.ToLower();
                string currentLongNameLower = current.LongName.ToLower();
                string currentDescriptionLower = current.Description.ToLower();

                bool shortNameMatches = (currentShortNameLower.Contains(filterLower));
                bool longNameMatches = (currentLongNameLower.Contains(filterLower));
                bool descriptionMatches = (currentDescriptionLower.Contains(filterLower));

                if (shortNameMatches || longNameMatches || descriptionMatches)
                {
                    var route = Common.CreateTransitRoute(i);
                    lst.Add(route);
                }
            }
            return lst;
        }

        public static TransitMetadata CreateMetaData()
        {
            return new TransitMetadata()
            {
                valid_from = new TransitDate()
                {
                    year = Common.repository.MetaInfo.MinDate.Year,
                    month = Common.repository.MetaInfo.MinDate.Month,
                    day = Common.repository.MetaInfo.MinDate.Day
                },
                valid_to = new TransitDate()
                {
                    year = Common.repository.MetaInfo.MaxDate.Year,
                    month = Common.repository.MetaInfo.MaxDate.Month,
                    day = Common.repository.MetaInfo.MaxDate.Day
                },
                valid_duration = Common.repository.MetaInfo.CountOfServiceDays
            };
        }

        public static List<TransitSequenceGroup> CreateSchedule(int routeId, TransitDate when)
        {
            var route = Common.repository.GetRouteById(routeId);
            var result = new List<TransitSequenceGroup>();

            foreach (var sequenceId in route.TripsBySequence.Keys)
            {
                int count_of_trips_available_on_this_day = route.TripsBySequence[sequenceId]
                    .Select(tripid => Common.repository.GetTripById(tripid))
                    .Count(trip => Common.repository.IsServiceAvailableOnDay(trip.ServiceIdx, Common.GetServiceDay(when)));

                if (count_of_trips_available_on_this_day < 1)
                {
                    continue;
                }

                var sequence = Common.repository.GetSequenceById(sequenceId).ToList();
                var trip_sample = route.TripsBySequence[sequenceId].First();

                var schedule = new TransitSequenceGroup()
                {
                    sequence_base_times = new List<TransitDateTime>(),
                    sequence_trip_ids = new List<int>(),
                    sequence_elements = new List<TransitSequenceElement>(),
                    sequence_info = new TransitSequenceInfo()
                    {
                        id = sequenceId,
                        headsign = Common.repository.GetTripById(trip_sample).Headsign,
                        count_of_stops = sequence.Count(),
                        first_stop = Common.CreateTransitStop(sequence.First().StopIdx),
                        last_stop = Common.CreateTransitStop(sequence.Last().StopIdx)
                    }
                };

                for (int i = 0; i < sequence.Count(); i++)
                {
                    var sequence_element = new TransitSequenceElement()
                    {
                        order = i,
                        stop = Common.CreateTransitStop(sequence[i].StopIdx),
                        arrival = sequence[i].ArrivalTime,
                        departure = sequence[i].DepartureTime
                    };
                    schedule.sequence_elements.Add(sequence_element);
                }

                foreach (var tripId in route.TripsBySequence[sequenceId])
                {
                    var trip = Common.repository.GetTripById(tripId);

                    if (!Common.repository.IsServiceAvailableOnDay(trip.ServiceIdx, Common.GetServiceDay(when)))
                    {
                        continue;
                    }

                    DateTime trip_start = new DateTime(when.year, when.month, when.day).AddMinutes(trip.IntervalFrom);

                    var base_time = new TransitDateTime()
                    {
                        year = trip_start.Year,
                        month = trip_start.Month,
                        day = trip_start.Day,
                        hour = trip_start.Hour,
                        minute = trip_start.Minute
                    };
                    schedule.sequence_base_times.Add(base_time);
                    schedule.sequence_trip_ids.Add(tripId);
                }

                result.Add(schedule);
            }

            return result;
        }

        private static int GetServiceDay(TransitDate when)
        {
            var meta = Common.repository.MetaInfo;
            return (int)(when.AsDate - meta.MinDate).TotalDays;
        }
    }
}