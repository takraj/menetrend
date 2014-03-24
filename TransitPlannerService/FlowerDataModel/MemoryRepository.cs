using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowerDataModel
{
    public class MemoryRepository : IRepository
    {
        protected IDataSource _dataSource;

        protected Metadata _meta;
        protected List<Stop> _stops;
        protected List<Route> _routes;
        protected List<Trip> _trips;
        protected List<List<StopTime>> _sequences;
        protected HashSet<IntegerPair> _serviceAvailability;
        protected Dictionary<IntegerPair, SequenceLookupData> _sequenceLookup;

        public MemoryRepository(IDataSource dataSource)
        {
            _dataSource = dataSource;
            _meta = new Metadata();
            _stops = new List<Stop>();
            _routes = new List<Route>();
            _trips = new List<Trip>();
            _sequences = new List<List<StopTime>>();
            _serviceAvailability = new HashSet<IntegerPair>();
            _sequenceLookup = new Dictionary<IntegerPair,SequenceLookupData>();
        }

        public Metadata MetaInfo
        {
            get { return _meta; }
        }

        public IEnumerable<Stop> Stops
        {
            get { return _stops; }
        }

        public IEnumerable<Route> Routes
        {
            get { return _routes; }
        }

        public IEnumerable<Trip> Trips
        {
            get { return _trips; }
        }

        public Stop GetStopById(int id)
        {
            return _stops[id];
        }

        public Route GetRouteById(int id)
        {
            return _routes[id];
        }

        public Trip GetTripById(int id)
        {
            return _trips[id];
        }

        public bool IsServiceAvailableOnDay(int serviceId, int day)
        {
            return _serviceAvailability.Contains(new IntegerPair(serviceId, day));
        }

        public IEnumerable<StopTime> GetSequenceById(int id)
        {
            return _sequences[id];
        }

        public SequenceLookupData LookupNextStop(int sequenceId, int stopId)
        {
            var key = new IntegerPair(sequenceId, stopId);
            if (_sequenceLookup.ContainsKey(key))
            {
                return _sequenceLookup[key];
            }
            else
            {
                return null;
            }
        }

        #region Loaders

        protected void LoadMetadata()
        {
            foreach (var data in _dataSource.GetAllMetaInfo())
            {
                if (data["key"] == "MIN_DATE")
                {
                    _meta.MinDate = DateTime.ParseExact(data["value"], "yyyyMMdd", CultureInfo.InvariantCulture);
                }
                else if (data["key"] == "MAX_DATE")
                {
                    _meta.MaxDate = DateTime.ParseExact(data["value"], "yyyyMMdd", CultureInfo.InvariantCulture);
                }
                else if (data["key"] == "MAX_SPEED")
                {
                    _meta.MaxSpeed = int.Parse(data["value"]);
                }
                else if (data["key"] == "COUNT_OF_SERVICE_DAYS")
                {
                    _meta.CountOfServiceDays = int.Parse(data["value"]);
                }
                else if (data["key"] == "COUNT_OF_SEQUENCES")
                {
                    _meta.CountOfSequences = int.Parse(data["value"]);
                }
            }
        }

        protected void LoadSchedules()
        {
            foreach (var data in _dataSource.GetAllSchedule())
            {
                var service_day = int.Parse(data["service_day"]);
                var calendar_idx = int.Parse(data["calendar_idx"]);

                _serviceAvailability.Add(new IntegerPair(service_day, calendar_idx));
            }
        }

        protected void LoadSequences()
        {
            for (int i = 0; i < _meta.CountOfSequences; i++)
            {
                _sequences.Add(new List<StopTime>());
            }

            foreach (var data in _dataSource.GetAllSequence())
            {
                var sequence_idx = int.Parse(data["sequence_idx"]);

                var item = new StopTime
                {
                    ArrivalTime = int.Parse(data["arrival_time"]),
                    DepartureTime = int.Parse(data["departure_time"]),
                    StopIdx = int.Parse(data["stop_idx"])
                };

                _sequences[sequence_idx].Add(item);
            }
        }

        protected void LoadSequenceLookup()
        {
            foreach (var data in _dataSource.GetAllSequenceLookupData())
            {
                var key = new IntegerPair(int.Parse(data["sequence_idx"]), int.Parse(data["stop_idx"]));

                _sequenceLookup[key] = new SequenceLookupData
                {
                    DepartureTime = int.Parse(data["departure_time"]),
                    IdxInSequence = int.Parse(data["idx_in_seq"]),
                    NextStopArrivalTime = int.Parse(data["next_arrival_time"]),
                    NextStopIdx = int.Parse(data["next_stop_idx"])
                };
            }
        }

        protected void LoadStops()
        {
            foreach (var data in _dataSource.GetAllStop())
            {
                _stops.Add(new Stop
                {
                    Name = data["stop_name"],
                    Latitude = double.Parse(data["stop_lat"], CultureInfo.InvariantCulture),
                    Longitude = double.Parse(data["stop_lon"], CultureInfo.InvariantCulture)
                });
            }

            foreach (var data in _dataSource.GetAllStopRoutePair())
            {
                var stop_idx = int.Parse(data["stop_idx"]);
                var route_idx = int.Parse(data["route_idx"]);

                if (_stops[stop_idx].Routes == null)
                {
                    _stops[stop_idx].Routes = new List<int>();
                }

                _stops[stop_idx].Routes.Add(route_idx);
            }

            foreach (var data in _dataSource.GetAllTransferByWalkInfo())
            {
                var stop_idx1 = int.Parse(data["stop_idx1"]);

                if (_stops[stop_idx1].Distances == null)
                {
                    _stops[stop_idx1].Distances = new List<StopDistance>();
                }

                _stops[stop_idx1].Distances.Add(new StopDistance
                {
                    StopIdx = int.Parse(data["stop_idx2"]),
                    Distance = double.Parse(data["distance"], CultureInfo.InvariantCulture)
                });
            }
        }

        protected void LoadRoutes()
        {
            foreach (var data in _dataSource.GetAllRoute())
            {
                var item = new Route
                {
                    DayOverlap = data["day_overlap"] == "1",
                    Description = data["route_desc"],
                    Duration = int.Parse(data["day_duration"]),
                    LongName = data["route_long_name"],
                    RouteColor = data["route_color"],
                    RouteTextColor = data["route_text_color"],
                    RouteType = int.Parse(data["route_type"]),
                    ShortName = data["route_short_name"]
                };

                if (data["start_of_day"] == data["end_of_day"])
                {
                    item.StartOfDay = 0;
                    item.EndOfDay = 0;
                }
                else
                {
                    item.EndOfDay = int.Parse(data["end_of_day"]);
                    item.StartOfDay = int.Parse(data["start_of_day"]);
                }

                _routes.Add(item);
            }

            foreach (var data in _dataSource.GetAllTrip())
            {
                var item = new Trip
                {
                    DayOverlap = data["day_overlap"] == "1",
                    Duration = int.Parse(data["trip_duration"]),
                    IntervalFrom = int.Parse(data["trip_interval_from"]),
                    IntervalTo = int.Parse(data["trip_interval_to"]),
                    RouteIdx = int.Parse(data["route_idx"]),
                    SequenceIdx = int.Parse(data["sequence_idx"]),
                    ServiceIdx = int.Parse(data["service_idx"]),
                    WheelchairAccessible = data["wheelchair_accessible"] == "1"
                };

                var tripRoute = _routes[item.RouteIdx];
                var tripIdx = _trips.Count;

                if (tripRoute.TripsBySequence == null)
                {
                    tripRoute.TripsBySequence = new Dictionary<int, List<int>>();
                }

                if (!tripRoute.TripsBySequence.ContainsKey(item.SequenceIdx))
                {
                    tripRoute.TripsBySequence[item.SequenceIdx] = new List<int>();
                }

                tripRoute.TripsBySequence[item.SequenceIdx].Add(tripIdx);
                _trips.Add(item);
            }
        }

        #endregion
    }
}
