using MTR.DataAccess.EFDataManager.Entities;
using MTR.DataAccess.EFDataManager.Serialization;
using ProtoBuf;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTR.DataAccess.EFDataManager
{
    public class DbManager
    {
        private static string _appHomeDir = Directory.GetCurrentDirectory();
        private static string cacheRootDir = "cache";
        private static string cacheTimetablesDir = "timetables";
        private static string cacheExt = ".dat";

        /// <summary>
        /// Returns all stop records from the database
        /// </summary>
        /// <returns></returns>
        public static List<MTR.BusinessLogic.Common.POCO.Stop> GetAllStops()
        {
            var result = new List<MTR.BusinessLogic.Common.POCO.Stop>();

            using (var db = new EF_GtfsDbContext())
            {
                foreach (EF_Stop s in db.Stops.ToList())
                {
                    result.Add(new BusinessLogic.Common.POCO.Stop(s.Id, s.OriginalId, s.StopName, s.StopLatitude, s.StopLongitude, s.LocationType, (s.ParentStation != null) ? (int?)s.ParentStation.Id : null, s.WheelchairBoarding, s.GroupId));
                }
            }

            return result;
        }

        public static MTR.BusinessLogic.Common.POCO.Route GetRouteById(int routeId)
        {
            using (var db = new EF_GtfsDbContext())
            {
                return (from dbRoute in db.Routes
                       where dbRoute.Id == routeId
                       select new MTR.BusinessLogic.Common.POCO.Route
                       {
                           AgencyId = dbRoute.AgencyId.Id,
                           DbId = dbRoute.Id,
                           OriginalId = dbRoute.OriginalId,
                           RouteColor = dbRoute.RouteColor,
                           RouteDescription = dbRoute.RouteDescription,
                           RouteShortName = dbRoute.RouteShortName,
                           RouteTextColor = dbRoute.RouteTextColor,
                           RouteType = dbRoute.RouteType
                       }).Single();
            }
        }

        public static MTR.BusinessLogic.Common.POCO.Stop GetStopById(int stopId)
        {
            using (var db = new EF_GtfsDbContext())
            {
                var s = db.Stops.Single(stop => stop.Id == stopId);
                return new BusinessLogic.Common.POCO.Stop(s.Id, s.OriginalId, s.StopName, s.StopLatitude, s.StopLongitude, s.LocationType, (s.ParentStation != null) ? (int?)s.ParentStation.Id : null, s.WheelchairBoarding, s.GroupId);
            }
        }

        /// <summary>
        /// Visszaad minden lehetséges útvonalhoz egy Trip-et, listában
        /// </summary>
        /// <returns></returns>
        public static List<MTR.BusinessLogic.Common.POCO.Trip> GetTripsWithDistinctPaths()
        {
            var result = new List<MTR.BusinessLogic.Common.POCO.Trip>();
            var shapes = new HashSet<Int32>();

            using (var db = new EF_GtfsDbContext())
            {
                foreach (EF_Trip t in db.Trips.ToList())
                {
                    if (shapes.Add(t.ShapeId.Id))
                    {
                        result.Add(new MTR.BusinessLogic.Common.POCO.Trip { 
                            BlockId = t.BlockId,
                            DbId = t.Id,
                            DirectionId = t.DirectionId,
                            OriginalId = t.OriginalId,
                            RouteId = t.RouteId.Id,
                            ServiceId = t.ServiceId.Id,
                            ShapeId = t.ShapeId.Id,
                            TripHeadsign = t.TripHeadsign,
                            WheelchairAccessible = t.WheelchairAccessible
                        });
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Egy adott Trip-hez visszaadja, hogy milyen sorrendben jönnek a megállók (Stop lista)
        /// </summary>
        /// <param name="trip"></param>
        /// <returns></returns>
        public static List<MTR.BusinessLogic.Common.POCO.Stop> GetStopsOrderByTrip(MTR.BusinessLogic.Common.POCO.Trip trip)
        {
            var result = new List<MTR.BusinessLogic.Common.POCO.Stop>();

            using (var db = new EF_GtfsDbContext())
            {
                foreach (EF_StopTime stoptime in db.StopTimes.Where(st => st.TripId.Id == trip.DbId).OrderBy(st => st.StopSequence).ToList())
                {
                    var s = stoptime.StopId;
                    result.Add(new BusinessLogic.Common.POCO.Stop(s.Id, s.OriginalId, s.StopName, s.StopLatitude, s.StopLongitude, s.LocationType, (s.ParentStation != null) ? (int?)s.ParentStation.Id : null, s.WheelchairBoarding, s.GroupId));
                }
            }

            return result;
        }

        private static Dictionary<string, List<TimetableItem>> timetables = new Dictionary<string,List<TimetableItem>>();
        private static object lck = new Object();

        public static void LoadCache(string appHomeDir = "")
        {
            if (appHomeDir != "")
            {
                _appHomeDir = appHomeDir;
            }
            else
            {
                _appHomeDir = Directory.GetCurrentDirectory();
            }

            string timetableCachePathRoot = _appHomeDir + Path.DirectorySeparatorChar + cacheRootDir + Path.DirectorySeparatorChar + cacheTimetablesDir + Path.DirectorySeparatorChar;

            foreach (var path in Directory.GetFiles(timetableCachePathRoot, "*" + cacheExt))
            {
                string filename = timetableCachePathRoot + path.Split(Path.DirectorySeparatorChar).Last();
                lock (lck)
                {
                    if (!timetables.ContainsKey(filename))
                    {
                        using (var file = File.OpenRead(path))
                        {
                            timetables.Add(filename, Serializer.Deserialize<List<TimetableItem>>(file).ToList()); // Eleve rendezett lista!
                        }
                    }
                }
            }
        }

        /// <summary>
        /// A következő indulási időpontot adja meg, cacheből
        /// </summary>
        /// <param name="date"></param>
        /// <param name="referenceTime"></param>
        /// <param name="routeId"></param>
        /// <param name="stopId"></param>
        /// <returns></returns>
        public static TimeSpan? GetNextDepartureFromCache(DateTime date, TimeSpan referenceTime, int routeId, int stopId)
        {
            string timetableCachePathRoot = _appHomeDir + Path.DirectorySeparatorChar + cacheRootDir + System.IO.Path.DirectorySeparatorChar + cacheTimetablesDir + System.IO.Path.DirectorySeparatorChar;

            try
            {
                string filename = timetableCachePathRoot + routeId.ToString() + "-" + stopId.ToString() + cacheExt;
                List<TimetableItem> timetable = null;

                lock (lck)
                {
                    if (!timetables.ContainsKey(filename))
                    {
                        using (var file = File.OpenRead(filename))
                        {
                            timetables.Add(filename, Serializer.Deserialize<List<TimetableItem>>(file).ToList()); // Eleve rendezett lista!
                        }
                    }
                    timetable = timetables[filename];
                }

                // Szűrés napra:
                switch (date.DayOfWeek)
                {
                    case DayOfWeek.Monday:
                        timetable = timetable.Where(t => t.ValidOnMonday).ToList();
                        break;
                    case DayOfWeek.Tuesday:
                        timetable = timetable.Where(t => t.ValidOnTuesday).ToList();
                        break;
                    case DayOfWeek.Wednesday:
                        timetable = timetable.Where(t => t.ValidOnWednesday).ToList();
                        break;
                    case DayOfWeek.Thursday:
                        timetable = timetable.Where(t => t.ValidOnThursday).ToList();
                        break;
                    case DayOfWeek.Friday:
                        timetable = timetable.Where(t => t.ValidOnFriday).ToList();
                        break;
                    case DayOfWeek.Saturday:
                        timetable = timetable.Where(t => t.ValidOnSaturday).ToList();
                        break;
                    case DayOfWeek.Sunday:
                        timetable = timetable.Where(t => t.ValidOnSunday).ToList();
                        break;
                }

                return TimeSpan.FromTicks(timetable.First(ti => (ti.DepartureTick > referenceTime.Ticks)
                    && (ti.ValidFromTick <= date.Ticks) && (ti.ValidToTick >= date.Ticks)).DepartureTick);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// A következő indulási időpontot adja meg, közvetlenül adatbázisból
        /// </summary>
        /// <param name="date"></param>
        /// <param name="referenceTime"></param>
        /// <param name="routeId"></param>
        /// <param name="stopId"></param>
        /// <returns></returns>
        public static TimeSpan? GetNextDeparture(DateTime date, TimeSpan referenceTime, int routeId, int stopId)
        {
            using (var db = new EF_GtfsDbContext())
            {
                var query = from stoptimes in db.StopTimes
                            where ((stoptimes.StopId.Id == stopId)
                                && (stoptimes.TripId.RouteId.Id == routeId)
                                && (stoptimes.DepartureTime > referenceTime)
                                && (stoptimes.TripId.ServiceId.StartDate <= date)
                                && (stoptimes.TripId.ServiceId.EndDate >= date))
                            orderby stoptimes.DepartureTime ascending
                            select stoptimes;

                try
                {
                    switch (date.DayOfWeek)
                    {
                        case DayOfWeek.Monday:
                            return query.First(t => t.TripId.ServiceId.Monday).DepartureTime;
                        case DayOfWeek.Tuesday:
                            return query.First(t => t.TripId.ServiceId.Tuesday).DepartureTime;
                        case DayOfWeek.Wednesday:
                            return query.First(t => t.TripId.ServiceId.Wednesday).DepartureTime;
                        case DayOfWeek.Thursday:
                            return query.First(t => t.TripId.ServiceId.Thursday).DepartureTime;
                        case DayOfWeek.Friday:
                            return query.First(t => t.TripId.ServiceId.Friday).DepartureTime;
                        case DayOfWeek.Saturday:
                            return query.First(t => t.TripId.ServiceId.Saturday).DepartureTime;
                        case DayOfWeek.Sunday:
                            return query.First(t => t.TripId.ServiceId.Sunday).DepartureTime;
                        default:
                            return null;
                    }
                }
                catch
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// A szolgáltatási rendet adja vissza
        /// </summary>
        /// <param name="date">Melyik napra vagy kíváncsi?</param>
        /// <returns>RouteId, StopId -- ListOfTimeSpan (mikor indul onnan az adott járat)</returns>
        public static Dictionary<int, Dictionary<int, List<TimeSpan>>> GetTimetable(DateTime date)
        {
            var result = new Dictionary<int, Dictionary<int, List<TimeSpan>>>();

            using (var db = new EF_GtfsDbContext())
            {
                // Filter 1
                var trips = db.Trips.Where(t => ((t.ServiceId.StartDate <= date) && (t.ServiceId.EndDate >= date)));

                // Filter 2
                switch (date.DayOfWeek)
                {
                    case DayOfWeek.Monday:
                        trips = trips.Where(t => t.ServiceId.Monday);
                        break;
                    case DayOfWeek.Tuesday:
                        trips = trips.Where(t => t.ServiceId.Tuesday);
                        break;
                    case DayOfWeek.Wednesday:
                        trips = trips.Where(t => t.ServiceId.Wednesday);
                        break;
                    case DayOfWeek.Thursday:
                        trips = trips.Where(t => t.ServiceId.Thursday);
                        break;
                    case DayOfWeek.Friday:
                        trips = trips.Where(t => t.ServiceId.Friday);
                        break;
                    case DayOfWeek.Saturday:
                        trips = trips.Where(t => t.ServiceId.Saturday);
                        break;
                    case DayOfWeek.Sunday:
                        trips = trips.Where(t => t.ServiceId.Sunday);
                        break;
                }

                // for easy querying
                var tripids = new List<int>();
                trips.ToList().ForEach(t => tripids.Add(t.Id));
                trips = null;

                foreach (int tripid in tripids)
                {
                    // query
                    Console.WriteLine("Constructing easy query...");
                    var times = db.StopTimes.Where(st => (st.TripId.Id == tripid)).OrderBy(st => st.StopSequence);

                    // create result
                    Console.WriteLine("Running easy query...");
                    foreach (var t in times.ToArray())
                    {
                        // 1. szint
                        if (!result.ContainsKey(t.TripId.RouteId.Id))
                        {
                            result.Add(t.TripId.RouteId.Id, new Dictionary<int, List<TimeSpan>>());
                        }

                        Dictionary<int, List<TimeSpan>> timesOfStops;
                        result.TryGetValue(t.TripId.RouteId.Id, out timesOfStops);

                        // 2. szint
                        if (!timesOfStops.ContainsKey(t.StopId.Id))
                        {
                            timesOfStops.Add(t.StopId.Id, new List<TimeSpan>());
                        }

                        List<TimeSpan> timesOfStop;
                        timesOfStops.TryGetValue(t.StopId.Id, out timesOfStop);

                        // 3. szint
                        timesOfStop.Add(t.DepartureTime);

                        Console.Write("#" + t.Id);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// A szolgáltatási rendet gyorsítótárazza, asszociatív módon,
        /// fájlokba csoportosítva járatok-megállók alapján
        /// </summary>
        public static void CreateTimetableAssociativeCache(string appHomeDir = "")
        {
            if (appHomeDir != "")
            {
                _appHomeDir = appHomeDir;
            }
            else
            {
                _appHomeDir = Directory.GetCurrentDirectory();
            }

            ulong counter = 0;
            string timetableCachePathRoot = _appHomeDir + Path.DirectorySeparatorChar + cacheRootDir + Path.DirectorySeparatorChar + cacheTimetablesDir + Path.DirectorySeparatorChar;

            using (var db = new EF_GtfsDbContext())
            {
                // query
                Console.WriteLine("Constructing query...");
                var times = from stoptimes in db.StopTimes
                            select new TimetableItem
                            {
                                RouteDbId = stoptimes.TripId.RouteId.Id,
                                StopDbId = stoptimes.StopId.Id,
                                ValidFrom = stoptimes.TripId.ServiceId.StartDate,
                                ValidTo = stoptimes.TripId.ServiceId.EndDate,
                                ValidOnMonday = stoptimes.TripId.ServiceId.Monday,
                                ValidOnTuesday = stoptimes.TripId.ServiceId.Tuesday,
                                ValidOnWednesday = stoptimes.TripId.ServiceId.Wednesday,
                                ValidOnThursday = stoptimes.TripId.ServiceId.Thursday,
                                ValidOnFriday = stoptimes.TripId.ServiceId.Friday,
                                ValidOnSaturday = stoptimes.TripId.ServiceId.Saturday,
                                ValidOnSunday = stoptimes.TripId.ServiceId.Sunday,
                                Departure = stoptimes.DepartureTime
                            };

                // create result: Berakja egy szótárba, ahol a kulcs a megfelelő RouteId-StopId pár
                var timetableDict = new Dictionary<string, List<TimetableItem>>();
                Console.WriteLine("Recreating directory...");
                MTR.Common.Utility.recreateFolder(timetableCachePathRoot);
                Console.WriteLine("Running query...");
                foreach (var db_stoptime in db.StopTimes.OrderBy(e => e.TripId.RouteId.Id).ThenBy(e => e.StopId.Id))
                {
                    var t = new TimetableItem
                    {
                        RouteDbId = db_stoptime.TripId.RouteId.Id,
                        StopDbId = db_stoptime.StopId.Id,
                        ValidFrom = db_stoptime.TripId.ServiceId.StartDate,
                        ValidTo = db_stoptime.TripId.ServiceId.EndDate,
                        ValidOnMonday = db_stoptime.TripId.ServiceId.Monday,
                        ValidOnTuesday = db_stoptime.TripId.ServiceId.Tuesday,
                        ValidOnWednesday = db_stoptime.TripId.ServiceId.Wednesday,
                        ValidOnThursday = db_stoptime.TripId.ServiceId.Thursday,
                        ValidOnFriday = db_stoptime.TripId.ServiceId.Friday,
                        ValidOnSaturday = db_stoptime.TripId.ServiceId.Saturday,
                        ValidOnSunday = db_stoptime.TripId.ServiceId.Sunday,
                        Departure = db_stoptime.DepartureTime
                    };
                    string filename = t.RouteDbId.ToString() + "-" + t.StopDbId.ToString();

                    if (!timetableDict.ContainsKey(filename))
                    {
                        if (timetableDict.Keys.Count > 0)
                        {
                            Console.WriteLine();

                            Console.WriteLine("Creating files...");
                            foreach (var key in timetableDict.Keys)
                            {
                                var values = timetableDict[key];

                                // Flush
                                using (var file = File.Create(timetableCachePathRoot + key + ".dat"))
                                {
                                    Console.Write("-- WRITING FILE: " + timetableCachePathRoot + key + cacheExt + " --");
                                    Serializer.Serialize(file, values.OrderBy(v => v.DepartureTick)); // Bináris szerializáló! Nagyon gyors! (protobuf-net)
                                    Console.WriteLine(" [OK, " + values.Count + " items written]");
                                }
                            }

                            Console.WriteLine();
                        }
                        timetableDict.Clear();
                        timetableDict.Add(filename, new List<TimetableItem>());
                    }

                    timetableDict[filename].Add(t);

                    counter++;
                    if ((counter % 100000) == 0)
                    {
                        Console.WriteLine((counter / 100000).ToString() + " x 100K processed");
                    }
                }

                Console.WriteLine();

                Console.WriteLine("Creating files...");
                foreach (var key in timetableDict.Keys)
                {
                    var values = timetableDict[key];

                    // Flush
                    using (var file = File.Create(timetableCachePathRoot + key + ".dat"))
                    {
                        Console.Write("-- WRITING FILE: " + timetableCachePathRoot + key + cacheExt + " --");
                        Serializer.Serialize(file, values.OrderBy(v => v.DepartureTick)); // Bináris szerializáló! Nagyon gyors! (protobuf-net)
                        Console.WriteLine(" [OK, " + values.Count + " items written]");
                    }
                }
            }
        }

        /// <summary>
        /// Returns all stop times (stop sequences) that are valid for the specified date
        /// </summary>
        /// <param name="date"></param>
        /// <returns>Ordered list (StopSequence ASC)</returns>
        public static List<BusinessLogic.Common.POCO.StopTime> GetAllStopTimesForDate(DateTime date)
        {
            // Majd refaktorálni kellene, mert ez csak a megállók lehetséges sorrendjeit
            // adja meg (nincs benne az összes trip)
            var result = new List<BusinessLogic.Common.POCO.StopTime>();

            using (var db = new EF_GtfsDbContext())
            {
                var trips = db.Trips.Where(t => ((t.ServiceId.StartDate <= date) && (t.ServiceId.EndDate >= date)));

                switch (date.DayOfWeek)
                {
                    case DayOfWeek.Monday:
                        trips = trips.Where(t => t.ServiceId.Monday);
                        break;
                    case DayOfWeek.Tuesday:
                        trips = trips.Where(t => t.ServiceId.Tuesday);
                        break;
                    case DayOfWeek.Wednesday:
                        trips = trips.Where(t => t.ServiceId.Wednesday);
                        break;
                    case DayOfWeek.Thursday:
                        trips = trips.Where(t => t.ServiceId.Thursday);
                        break;
                    case DayOfWeek.Friday:
                        trips = trips.Where(t => t.ServiceId.Friday);
                        break;
                    case DayOfWeek.Saturday:
                        trips = trips.Where(t => t.ServiceId.Saturday);
                        break;
                    case DayOfWeek.Sunday:
                        trips = trips.Where(t => t.ServiceId.Sunday);
                        break;
                }

                // eliminate redundancy
                var rtrips = new List<EF_Trip>();
                foreach (EF_Trip t in trips.ToArray())
                {
                    if (!rtrips.Exists(rt => (rt.DirectionId.CompareTo(t.DirectionId) == 0) && (rt.TripHeadsign.Equals(t.TripHeadsign)))) {
                        rtrips.Add(t);
                    }
                }
                trips = null;

                // for easy querying
                var rtripids = new List<int>();
                rtrips.ForEach(rt => rtripids.Add(rt.Id));
                rtrips = null;

                // query
                var times = db.StopTimes.Where(st => rtripids.Contains(st.TripId.Id)).OrderBy(st => st.StopSequence);

                // create result
                foreach (EF_StopTime st in times.ToArray())
                {
                    result.Add(new BusinessLogic.Common.POCO.StopTime(st.Id, st.TripId.Id, st.StopId.Id, st.ArrivalTime, st.DepartureTime, st.StopSequence, st.ShapeDistanceTraveled));
                }
            }

            return result;
        }

        public static void AddEdgesToDatabase(IEnumerable<MTR.BusinessLogic.Common.POCO.StopRouteStopEdge> edgeList)
        {
            using (var db = new EF_GtfsDbContext())
            {
                foreach (var edge in edgeList)
                {
                    var source = db.Stops.First(s => s.Id == edge.StopId);
                    var destination = db.Stops.First(s => s.Id == edge.nextStopId);
                    var route = db.Routes.First(r => r.Id == edge.RouteId);

                    db.StopEdges.Add(new EF_StopRouteStopEdge {
                        SourceStop = source,
                        ViaRoute = route,
                        DestinationStop = destination
                    });
                }
                db.SaveChanges();
            }
        }

        public static List<MTR.BusinessLogic.Common.POCO.StopRouteStopEdge> GetEdgesFromDatabase()
        {
            using (var db = new EF_GtfsDbContext())
            {
                return (from edges in db.StopEdges
                       select new MTR.BusinessLogic.Common.POCO.StopRouteStopEdge
                       {
                           StopId = edges.SourceStop.Id,
                           RouteId = edges.ViaRoute.Id,
                           nextStopId = edges.DestinationStop.Id
                       }).ToList();
            }
        }

        public static void UpdateStopGroups(IEnumerable<MTR.BusinessLogic.Common.POCO.StopGroup> groups)
        {
            using (var db = new EF_GtfsDbContext())
            {
                var allStops = db.Stops.ToList();

                int i = 0;
                foreach (var sg in groups)
                {
                    i++;
                    foreach (var stop in sg.GetStops()) {
                        var subject = allStops.First(db_stop => db_stop.Id == stop.DbId);
                        subject.GroupId = i;
                        Console.WriteLine("stop.DbId = " + stop.DbId + " | group = " + i + " --> subject.GroupId = " + subject.GroupId);
                    }
                }
                db.SaveChanges();
            }
        }

        public static Dictionary<int, List<int>> GetStopGroups()
        {
            var result = new Dictionary<int, List<int>>();

            using (var db = new EF_GtfsDbContext())
            {
                foreach (var stop in db.Stops.ToArray())
                {
                    if (stop.GroupId != null)
                    {
                        if (!result.ContainsKey((int)stop.GroupId))
                        {
                            result.Add((int)stop.GroupId, new List<int>());
                        }

                        List<int> values;
                        result.TryGetValue((int)stop.GroupId, out values);
                        values.Add(stop.Id);
                    }
                }
            }

            return result;
        }
    }
}
