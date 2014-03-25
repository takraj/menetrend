using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransitPlannerLibrary.FlowerDataModel
{
    public class MemoryRepository : IRepository
    {
        protected IDataSource _dataSource;

        protected Metadata _meta;
        protected Stop[] _stops;
        protected Route[] _routes;
        protected Trip[] _trips;
        protected List<StopTime>[] _sequences;
        protected HashSet<int>[] _serviceAvailability;
        protected Dictionary<int, SequenceLookupData>[] _sequenceLookup;
        protected List<Trip>[] _tripsBySequences;

        #region CSV Constants

        private const string COLUMN_KEY = "key";
        private const string COLUMN_VALUE = "value";
        private const string META_KEY_COUNT_OF_SERVICE_DAYS = "COUNT_OF_SERVICE_DAYS";
        private const string META_KEY_MIN_DATE = "MIN_DATE";
        private const string META_KEY_MAX_DATE = "MAX_DATE";
        private const string META_KEY_MAX_SPEED = "MAX_SPEED";
        private const string META_KEY_COUNT_OF_SEQUENCES = "COUNT_OF_SEQUENCES";
        private const string META_KEY_COUNT_OF_TRIPS = "COUNT_OF_TRIPS";
        private const string META_KEY_COUNT_OF_ROUTES = "COUNT_OF_ROUTES";
        private const string META_KEY_COUNT_OF_STOPS = "COUNT_OF_STOPS";
        private const string META_KEY_COUNT_OF_CALENDARS = "COUNT_OF_CALENDARS";
        private const string COLUMN_SERVICE_DAY = "service_day";
        private const string COLUMN_CALENDAR_INDEX = "calendar_idx";
        private const string COLUMN_SEQUENCE_INDEX = "sequence_idx";
        private const string COLUMN_ARRIVAL_TIME = "arrival_time";
        private const string COLUMN_DEPARTURE_TIME = "departure_time";
        private const string COLUMN_STOP_INDEX = "stop_idx";
        private const string COLUMN_INDEX_IN_SEQUENCE = "idx_in_seq";
        private const string COLUMN_NEXT_ARRIVAL_TIME = "next_arrival_time";
        private const string COLUMN_NEXT_STOP_INDEX = "next_stop_idx";
        private const string COLUMN_STOP_LATITUDE = "stop_lat";
        private const string COLUMN_STOP_LONGITUDE = "stop_lon";
        private const string COLUMN_STOP_NAME = "stop_name";
        private const string COLUMN_ROUTE_INDEX = "route_idx";
        private const string COLUMN_FIRST_STOP_INDEX = "stop_idx1";
        private const string COLUMN_SECOND_STOP_INDEX = "stop_idx2";
        private const string COLUMN_DISTANCE = "distance";
        private const string YES = "1";
        private const string COLUMN_DAY_OVERLAP = "day_overlap";
        private const string COLUMN_ROUTE_DESCRIPTION = "route_desc";
        private const string COLUMN_DAY_DURATION = "day_duration";
        private const string COLUMN_ROUTE_LONG_NAME = "route_long_name";
        private const string COLUMN_ROUTE_COLOR = "route_color";
        private const string COLUMN_ROUTE_TEXT_COLOR = "route_text_color";
        private const string COLUMN_ROUTE_TYPE = "route_type";
        private const string COLUMN_ROUTE_SHORT_NAME = "route_short_name";
        private const string COLUMN_START_OF_DAY = "start_of_day";
        private const string COLUMN_END_OF_DAY = "end_of_day";
        private const string COLUMN_TRIP_DURATION = "trip_duration";
        private const string COLUMN_TRIP_INTERVAL_FROM = "trip_interval_from";
        private const string COLUMN_TRIP_INTERVAL_TO = "trip_interval_to";
        private const string COLUMN_SERVICE_INDEX = "service_idx";
        private const string COLUMN_WHEELCHAIR_ACCESSIBLE = "wheelchair_accessible";

        #endregion

        public MemoryRepository(IDataSource dataSource)
        {
            _dataSource = dataSource;
            _meta = new Metadata();

            this.LoadMetadata();

            Parallel.Invoke(
                () => this.LoadRoutes(),
                () => this.LoadStops(),
                () => this.LoadSchedules(),
                () => this.LoadSequences(),
                () => this.LoadSequenceLookup()
                );

            this.CalculateTripsBySequences();
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
            return _serviceAvailability[day].Contains(serviceId);
        }

        public IEnumerable<StopTime> GetSequenceById(int id)
        {
            return _sequences[id];
        }

        public IEnumerable<Trip> GetTripsBySequence(int id)
        {
            return _tripsBySequences[id];
        }

        public IEnumerable<int> GetSequencesByStop(int id)
        {
            return _sequenceLookup[id].Keys;
        }

        public SequenceLookupData LookupNextStop(int sequenceId, int stopId)
        {
            if (_sequenceLookup[stopId].ContainsKey(sequenceId))
            {
                return _sequenceLookup[stopId][sequenceId];
            }
            else
            {
                return null;
            }
        }

        #region Loaders

        private void LoadMetadata()
        {
            foreach (var data in _dataSource.GetAllMetaInfo())
            {
                if (data[COLUMN_KEY] == META_KEY_MIN_DATE)
                {
                    _meta.MinDate = DateTime.ParseExact(data[COLUMN_VALUE], "yyyyMMdd", CultureInfo.InvariantCulture);
                }
                else if (data[COLUMN_KEY] == META_KEY_MAX_DATE)
                {
                    _meta.MaxDate = DateTime.ParseExact(data[COLUMN_VALUE], "yyyyMMdd", CultureInfo.InvariantCulture);
                }
                else if (data[COLUMN_KEY] == META_KEY_MAX_SPEED)
                {
                    _meta.MaxSpeed = int.Parse(data[COLUMN_VALUE]);
                }
                else if (data[COLUMN_KEY] == META_KEY_COUNT_OF_SERVICE_DAYS)
                {
                    _meta.CountOfServiceDays = int.Parse(data[COLUMN_VALUE]);
                }
                else if (data[COLUMN_KEY] == META_KEY_COUNT_OF_SEQUENCES)
                {
                    _meta.CountOfSequences = int.Parse(data[COLUMN_VALUE]);
                }
                else if (data[COLUMN_KEY] == META_KEY_COUNT_OF_TRIPS)
                {
                    _meta.CountOfTrips = int.Parse(data[COLUMN_VALUE]);
                }
                else if (data[COLUMN_KEY] == META_KEY_COUNT_OF_ROUTES)
                {
                    _meta.CountOfRoutes = int.Parse(data[COLUMN_VALUE]);
                }
                else if (data[COLUMN_KEY] == META_KEY_COUNT_OF_STOPS)
                {
                    _meta.CountOfStops = int.Parse(data[COLUMN_VALUE]);
                }
                else if (data[COLUMN_KEY] == META_KEY_COUNT_OF_CALENDARS)
                {
                    _meta.CountOfCalendars = int.Parse(data[COLUMN_VALUE]);
                }
            }
        }

        private void LoadSchedules()
        {
            _serviceAvailability = new HashSet<int>[_meta.CountOfServiceDays];

            for (int i = 0; i < _meta.CountOfServiceDays; i++)
            {
                _serviceAvailability[i] = new HashSet<int>();
            }

            foreach (var data in _dataSource.GetAllSchedule())
            {
                var service_day = int.Parse(data[COLUMN_SERVICE_DAY]);
                var calendar_idx = int.Parse(data[COLUMN_CALENDAR_INDEX]);

                _serviceAvailability[service_day].Add(calendar_idx);
            }
        }

        private void LoadSequences()
        {
            _sequences = new List<StopTime>[_meta.CountOfSequences];

            for (int i = 0; i < _meta.CountOfSequences; i++)
            {
                _sequences[i] = new List<StopTime>();
            }

            foreach (var data in _dataSource.GetAllSequence())
            {
                var sequence_idx = int.Parse(data[COLUMN_SEQUENCE_INDEX]);

                var item = new StopTime
                {
                    ArrivalTime = int.Parse(data[COLUMN_ARRIVAL_TIME]),
                    DepartureTime = int.Parse(data[COLUMN_DEPARTURE_TIME]),
                    StopIdx = int.Parse(data[COLUMN_STOP_INDEX])
                };

                _sequences[sequence_idx].Add(item);
            }
        }

        private void LoadSequenceLookup()
        {
            _sequenceLookup = new Dictionary<int, SequenceLookupData>[_meta.CountOfStops];

            for (int i = 0; i < _meta.CountOfStops; i++)
            {
                _sequenceLookup[i] = new Dictionary<int, SequenceLookupData>();
            }

            foreach (var data in _dataSource.GetAllSequenceLookupData())
            {
                var seqId = int.Parse(data[COLUMN_SEQUENCE_INDEX]);
                var stopId = int.Parse(data[COLUMN_STOP_INDEX]);

                _sequenceLookup[stopId][seqId] = new SequenceLookupData
                {
                    DepartureTime = int.Parse(data[COLUMN_DEPARTURE_TIME]),
                    IdxInSequence = int.Parse(data[COLUMN_INDEX_IN_SEQUENCE]),
                    NextStopArrivalTime = int.Parse(data[COLUMN_NEXT_ARRIVAL_TIME]),
                    NextStopIdx = int.Parse(data[COLUMN_NEXT_STOP_INDEX])
                };
            }
        }

        private void LoadStops()
        {
            _stops = new Stop[_meta.CountOfStops];

            int i = 0;
            foreach (var data in _dataSource.GetAllStop())
            {
                _stops[i] = new Stop
                {
                    Name = data[COLUMN_STOP_NAME],
                    Latitude = double.Parse(data[COLUMN_STOP_LATITUDE], CultureInfo.InvariantCulture),
                    Longitude = double.Parse(data[COLUMN_STOP_LONGITUDE], CultureInfo.InvariantCulture)
                };

                i++;
            }

            foreach (var data in _dataSource.GetAllStopRoutePair())
            {
                var stop_idx = int.Parse(data[COLUMN_STOP_INDEX]);
                var route_idx = int.Parse(data[COLUMN_ROUTE_INDEX]);

                if (_stops[stop_idx].Routes == null)
                {
                    _stops[stop_idx].Routes = new List<int>();
                }

                _stops[stop_idx].Routes.Add(route_idx);
            }

            foreach (var data in _dataSource.GetAllTransferByWalkInfo())
            {
                var stop_idx1 = int.Parse(data[COLUMN_FIRST_STOP_INDEX]);

                if (_stops[stop_idx1].Distances == null)
                {
                    _stops[stop_idx1].Distances = new List<StopDistance>();
                }

                _stops[stop_idx1].Distances.Add(new StopDistance
                {
                    StopIdx = int.Parse(data[COLUMN_SECOND_STOP_INDEX]),
                    Distance = double.Parse(data[COLUMN_DISTANCE], CultureInfo.InvariantCulture)
                });
            }
        }

        private void LoadRoutes()
        {
            _routes = new Route[_meta.CountOfRoutes];

            int routeIdx = 0;
            foreach (var data in _dataSource.GetAllRoute())
            {
                var item = new Route
                {
                    DayOverlap = data[COLUMN_DAY_OVERLAP] == YES,
                    Description = data[COLUMN_ROUTE_DESCRIPTION],
                    Duration = int.Parse(data[COLUMN_DAY_DURATION]),
                    LongName = data[COLUMN_ROUTE_LONG_NAME],
                    RouteColor = data[COLUMN_ROUTE_COLOR],
                    RouteTextColor = data[COLUMN_ROUTE_TEXT_COLOR],
                    RouteType = int.Parse(data[COLUMN_ROUTE_TYPE]),
                    ShortName = data[COLUMN_ROUTE_SHORT_NAME]
                };

                if (data[COLUMN_START_OF_DAY] == data[COLUMN_END_OF_DAY])
                {
                    item.StartOfDay = 0;
                    item.EndOfDay = 0;
                }
                else
                {
                    item.StartOfDay = int.Parse(data[COLUMN_START_OF_DAY]);
                    item.EndOfDay = int.Parse(data[COLUMN_END_OF_DAY]);
                }

                _routes[routeIdx] = item;

                routeIdx++;
            }

            _trips = new Trip[_meta.CountOfTrips];

            int tripIdx = 0;
            foreach (var data in _dataSource.GetAllTrip())
            {
                var item = new Trip
                {
                    DayOverlap = data[COLUMN_DAY_OVERLAP] == YES,
                    Duration = int.Parse(data[COLUMN_TRIP_DURATION]),
                    IntervalFrom = int.Parse(data[COLUMN_TRIP_INTERVAL_FROM]),
                    IntervalTo = int.Parse(data[COLUMN_TRIP_INTERVAL_TO]),
                    RouteIdx = int.Parse(data[COLUMN_ROUTE_INDEX]),
                    SequenceIdx = int.Parse(data[COLUMN_SEQUENCE_INDEX]),
                    ServiceIdx = int.Parse(data[COLUMN_SERVICE_INDEX]),
                    WheelchairAccessible = data[COLUMN_WHEELCHAIR_ACCESSIBLE] == YES
                };

                var tripRoute = _routes[item.RouteIdx];

                if (tripRoute.TripsBySequence == null)
                {
                    tripRoute.TripsBySequence = new Dictionary<int, List<int>>();
                }

                if (!tripRoute.TripsBySequence.ContainsKey(item.SequenceIdx))
                {
                    tripRoute.TripsBySequence[item.SequenceIdx] = new List<int>();
                }

                tripRoute.TripsBySequence[item.SequenceIdx].Add(tripIdx);
                _trips[tripIdx] = item;

                tripIdx++;
            }
        }

        #endregion

        #region Calculators

        private void CalculateTripsBySequences()
        {
            _tripsBySequences = new List<Trip>[_meta.CountOfSequences];

            for (int i = 0; i < _meta.CountOfSequences; i++)
            {
                _tripsBySequences[i] = _trips.Where(t => t.SequenceIdx == i).ToList();
            }
        }

        #endregion
    }
}
