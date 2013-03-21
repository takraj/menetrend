using CsvHelper.Configuration;
using MTR.DataAccess.EFDataManager;
using MTR.DataAccess.EFDataManager.Entities;
using MTR.DataAccess.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTR.DataAccess.CsvDataManager
{
    /// <summary>
    /// Temporary in-memory database
    /// </summary>
    public class GtfsDatabase
    {
        /// <summary>
        /// Sets up the database initializer & runs a query to create the database
        /// if it does not exist.
        /// </summary>
        /// <param name="BasePath">Directory path to the GTFS files</param>
        public static void initDatabase(string BasePath)
        {
            Database.SetInitializer<EF_GtfsDbContext>(new GtfsDbInitializer(BasePath));
            using (var db = new EF_GtfsDbContext())
            {
                db.Agencies.Load();
            }
        }

        private class GtfsDbInitializer : DropCreateDatabaseIfModelChanges<EF_GtfsDbContext>
        {
            private string basepath;

            public GtfsDbInitializer(string BasePath)
            {
                basepath = BasePath;
            }

            protected override void Seed(EF_GtfsDbContext context)
            {
                importCsv(basepath);
            }
        }

        public static void importCsv(string BasePath) {
            var db_agencies = new ConcurrentDictionary<String, EF_Agency>();
            var db_calendars = new ConcurrentDictionary<String, EF_Calendar>();
            var db_routes = new ConcurrentDictionary<String, EF_Route>();
            var db_stops = new ConcurrentDictionary<String, EF_Stop>();
            var db_shapes = new ConcurrentDictionary<String, EF_Shape>();
            var db_trips = new ConcurrentDictionary<String, EF_Trip>();

            const int BATCH_INSERT_MAX = 10000;

            #region DatabaseSeed
            {
                #region Agency
                {
                    var agencies = new List<GTFS_Agency>();
                    agencies.AddRange(GtfsReader.Read<GTFS_Agency>(BasePath + GTFS_Agency.SourceFilename));
                    var bulkList = new List<EF_Agency>();

                    // locking support
                    var lockobject = new Object();
                    int iteration = 0;

                    // semi-parallel insertion
                    agencies.AsParallel().ForAll(a => {
                        lock (lockobject)
                        {
                            if (iteration > BATCH_INSERT_MAX)
                            {
                                iteration = 0;
                                EF_Agency.BulkInsertEntities(bulkList);
                                bulkList = new List<EF_Agency>();
                            }
                            bulkList.Add(new EF_Agency(a));
                            iteration++;
                        }
                    });
                    EF_Agency.BulkInsertEntities(bulkList);

                    using (var db = new EF_GtfsDbContext())
                    {
                        db.Agencies.AsParallel().ForAll(e => db_agencies.TryAdd(e.OriginalId, e));
                    }
                }
                #endregion

                #region Calendars
                {
                    var calendars = new List<GTFS_Calendar>();
                    calendars.AddRange(GtfsReader.Read<GTFS_Calendar>(BasePath + GTFS_Calendar.SourceFilename));
                    var bulkList = new List<EF_Calendar>();

                    // locking support
                    var lockobject = new Object();
                    int iteration = 0;

                    // semi-parallel insertion
                    calendars.AsParallel().ForAll(c => {
                        lock (lockobject)
                        {
                            if (iteration > BATCH_INSERT_MAX)
                            {
                                iteration = 0;
                                EF_Calendar.BulkInsertEntities(bulkList);
                                bulkList = new List<EF_Calendar>();
                            }
                            bulkList.Add(new EF_Calendar(c));
                            iteration++;
                        }
                    });
                    EF_Calendar.BulkInsertEntities(bulkList);

                    using (var db = new EF_GtfsDbContext())
                    {
                        db.Calendars.AsParallel().ForAll(e => db_calendars.TryAdd(e.OriginalId, e));
                    }
                }
                #endregion

                #region Routes
                {
                    var routes = new List<GTFS_Route>();
                    routes.AddRange(GtfsReader.Read<GTFS_Route>(BasePath + GTFS_Route.SourceFilename));
                    var bulkList = new List<EF_Route>();

                    // locking support
                    var lockobject = new Object();
                    int iteration = 0;

                    // semi-parallel insertion
                    routes.AsParallel().ForAll(r => {
                        EF_Agency agency;
                        db_agencies.TryGetValue(r.AgencyId, out agency);

                        lock (lockobject)
                        {
                            if (iteration > BATCH_INSERT_MAX)
                            {
                                iteration = 0;
                                EF_Route.BulkInsertEntities(bulkList);
                                bulkList = new List<EF_Route>();
                            }
                            bulkList.Add(new EF_Route(r, agency));
                            iteration++;
                        }
                    });
                    EF_Route.BulkInsertEntities(bulkList);

                    using (var db = new EF_GtfsDbContext())
                    {
                        db.Routes.AsParallel().ForAll(e => db_routes.TryAdd(e.OriginalId, e));
                    }
                }
                #endregion

                #region Calendar Dates (Exceptions)
                {
                    var calendar_dates = new List<GTFS_CalendarDate>();
                    calendar_dates.AddRange(GtfsReader.Read<GTFS_CalendarDate>(BasePath + GTFS_CalendarDate.SourceFilename));
                    var bulkList = new List<EF_CalendarDate>();

                    // locking support
                    var lockobject = new Object();
                    int iteration = 0;

                    // semi-parallel insertion
                    calendar_dates.AsParallel().ForAll(cd => {
                        EF_Calendar calendar;
                        db_calendars.TryGetValue(cd.ServiceId, out calendar);

                        lock (lockobject)
                        {
                            if (iteration > BATCH_INSERT_MAX)
                            {
                                iteration = 0;
                                EF_CalendarDate.BulkInsertEntities(bulkList);
                                bulkList = new List<EF_CalendarDate>();
                            }
                            bulkList.Add(new EF_CalendarDate(cd, calendar));
                            iteration++;
                        }
                    });
                    EF_CalendarDate.BulkInsertEntities(bulkList);
                }
                #endregion

                #region Stops
                {
                    var stops = new List<GTFS_Stop>();
                    stops.AddRange(GtfsReader.Read<GTFS_Stop>(BasePath + GTFS_Stop.SourceFilename));

                    // Phase 1: Seed database
                    {
                        var bulkList = new List<EF_Stop>();

                        // locking support
                        var lockobject = new Object();
                        int iteration = 0;

                        // semi-parallel insertion
                        stops.AsParallel().ForAll(s =>
                        {
                            lock (lockobject)
                            {
                                if (iteration > BATCH_INSERT_MAX)
                                {
                                    iteration = 0;
                                    EF_Stop.BulkInsertEntities(bulkList);
                                    bulkList = new List<EF_Stop>();
                                }
                                bulkList.Add(new EF_Stop(s));
                                iteration++;
                            }
                        });
                        EF_Stop.BulkInsertEntities(bulkList);
                    }

                    // Phase 2: Set parent stations
                    using (var db = new EF_GtfsDbContext())
                    {
                        db.Stops.AsParallel().ForAll(e => db_stops.TryAdd(e.OriginalId, e));
                        var filteredStops = stops.AsParallel().Where(s => (s.ParentStation != null) && (s.ParentStation.Length > 0));
                        filteredStops.AsParallel().ForAll(s =>
                        {
                            EF_Stop parent;
                            EF_Stop child;
                            db_stops.TryGetValue(s.ParentStation, out parent);
                            db_stops.TryGetValue(s.StopId, out child);
                            child.ParentStation = parent;
                        });
                        db.SaveChanges();
                        db_stops = new ConcurrentDictionary<string, EF_Stop>();
                        db.Stops.AsParallel().ForAll(e => db_stops.TryAdd(e.OriginalId, e));
                    }
                }
                #endregion

                #region Shapes
                {
                    var shapes = new List<GTFS_Shape>();
                    shapes.AddRange(GtfsReader.Read<GTFS_Shape>(BasePath + GTFS_Shape.SourceFilename));
                    var bulkList = new List<EF_Shape>();

                    // locking support
                    var lockobject = new Object();
                    int iteration = 0;

                    // semi-parallel insertion
                    shapes.AsParallel().ForAll(s => {
                        lock (lockobject)
                        {
                            if (iteration > BATCH_INSERT_MAX)
                            {
                                iteration = 0;
                                EF_Shape.BulkInsertEntities(bulkList);
                                bulkList = new List<EF_Shape>();
                            }
                            bulkList.Add(new EF_Shape(s));
                            iteration++;
                        }
                    });
                    EF_Shape.BulkInsertEntities(bulkList);

                    using (var db = new EF_GtfsDbContext())
                    {
                        db.Shapes.AsParallel().ForAll(e => db_shapes.TryAdd(e.OriginalId, e));
                    }
                }
                #endregion

                #region Trips
                {
                    var trips = new List<GTFS_Trip>();
                    trips.AddRange(GtfsReader.Read<GTFS_Trip>(BasePath + GTFS_Trip.SourceFilename));
                    var bulkList = new List<EF_Trip>();

                    // locking support
                    var lockobject = new Object();
                    int iteration = 0;

                    // semi-parallel insertion
                    trips.AsParallel().ForAll(t => {
                        EF_Route route;
                        EF_Calendar calendar;
                        EF_Shape shape;
                        db_routes.TryGetValue(t.RouteId, out route);
                        db_calendars.TryGetValue(t.ServiceId, out calendar);
                        db_shapes.TryGetValue(t.ShapeId, out shape);

                        lock (lockobject)
                        {
                            if (iteration > BATCH_INSERT_MAX)
                            {
                                iteration = 0;
                                EF_Trip.BulkInsertEntities(bulkList);
                                bulkList = new List<EF_Trip>();
                            }
                            bulkList.Add(new EF_Trip(t, route, calendar, shape));
                            iteration++;
                        }
                    });
                    EF_Trip.BulkInsertEntities(bulkList);

                    using (var db = new EF_GtfsDbContext())
                    {
                        db.Trips.AsParallel().ForAll(e => db_trips.TryAdd(e.OriginalId, e));
                    }
                }
                #endregion

                #region Stop Times
                {
                    // cleanup (because of heavy memory usage)
                    db_agencies = null;
                    db_calendars = null;
                    db_routes = null;
                    db_shapes = null;

                    var stop_times = new List<GTFS_StopTime>();
                    stop_times.AddRange(GtfsReader.Read<GTFS_StopTime>(BasePath + GTFS_StopTime.SourceFilename));
                    var bulkList = new List<EF_StopTime>();

                    // locking support
                    var lockobject = new Object();
                    int iteration = 0;

                    // semi-parallel insertion
                    stop_times.AsParallel().ForAll(st => {
                        EF_Trip trip;
                        EF_Stop stop;
                        db_trips.TryGetValue(st.TripId, out trip);
                        db_stops.TryGetValue(st.StopId, out stop);

                        lock (lockobject)
                        {
                            if (iteration > BATCH_INSERT_MAX)
                            {
                                iteration = 0;
                                EF_StopTime.BulkInsertEntities(bulkList);
                                bulkList = new List<EF_StopTime>();
                            }
                            bulkList.Add(new EF_StopTime(st, trip, stop));
                            iteration++;
                        }
                    });
                    EF_StopTime.BulkInsertEntities(bulkList.ToList());
                }
                #endregion
            }
            #endregion
        }
    }

    class GtfsReader
    {
        /// <summary>
        /// Reads in a CSV file
        /// </summary>
        /// <typeparam name="T">Type of instances to return</typeparam>
        /// <param name="fileName">Name of the file</param>
        /// <returns>List of instances with data from csv</returns>
        public static IEnumerable<T> Read<T>(string fileName) where T : class
        {
            var fileReader = System.IO.File.OpenText(fileName);
            var config = new CsvConfiguration();
            config.UseInvariantCulture = true;
            var csvReader = new CsvHelper.CsvReader(fileReader, config);
            return csvReader.GetRecords<T>();
        }
    }
}
