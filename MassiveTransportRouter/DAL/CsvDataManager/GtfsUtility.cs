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
using System.Threading;
using System.Threading.Tasks;

namespace MTR.DataAccess.CsvDataManager
{
    /// <summary>
    /// Temporary in-memory database
    /// </summary>
    public class GtfsDatabase
    {
        private static bool _databaseRecreated = false;

        /// <summary>
        /// Sets up the database initializer & runs a query to create the database
        /// if it does not exist.
        /// </summary>
        /// <param name="BasePath">Directory path to the GTFS files</param>
        public static bool initDatabase(string BasePath)
        {
            _databaseRecreated = false;

            Database.SetInitializer<EF_GtfsDbContext>(new GtfsDbInitializer(BasePath));
            using (var db = new EF_GtfsDbContext())
            {
                db.Agencies.Load();
            }

            return _databaseRecreated;
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
                _databaseRecreated = true;
            }
        }

        /// <summary>
        /// Imports the GTFS (CSV) database to MSSQL
        /// </summary>
        /// <param name="BasePath">Path to files (root directory)</param>
        public static void importCsv(string BasePath) {
            var db_agencies = new ConcurrentDictionary<String, EF_Agency>();
            var db_calendars = new ConcurrentDictionary<String, EF_Calendar>();
            var db_routes = new ConcurrentDictionary<String, EF_Route>();
            var db_stops = new ConcurrentDictionary<String, EF_Stop>();
            var db_shapes = new ConcurrentDictionary<String, EF_Shape>();
            var db_trips = new ConcurrentDictionary<String, EF_Trip>();

            const int BATCH_INSERT_MAX = 100000;

            #region DatabaseSeed
            {
                #region Agency
                {
                    var agencies = GtfsReader.Read<GTFS_Agency>(BasePath + GTFS_Agency.SourceFilename);
                    var bulkList = new List<EF_Agency>();
                    int iteration = 0;

                    foreach (var a in agencies)
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
                    EF_Agency.BulkInsertEntities(bulkList);

                    using (var db = new EF_GtfsDbContext())
                    {
                        foreach (var e in db.Agencies)
                        {
                            db_agencies.TryAdd(e.OriginalId, e);
                        }
                    }
                }
                #endregion

                #region Calendars
                {
                    var calendars = GtfsReader.Read<GTFS_Calendar>(BasePath + GTFS_Calendar.SourceFilename);
                    var bulkList = new List<EF_Calendar>();
                    int iteration = 0;

                    foreach (var c in calendars)
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
                    EF_Calendar.BulkInsertEntities(bulkList);

                    using (var db = new EF_GtfsDbContext())
                    {
                        foreach (var e in db.Calendars)
                        {
                            db_calendars.TryAdd(e.OriginalId, e);
                        }
                    }
                }
                #endregion

                #region Stops
                {
                    var stops = GtfsReader.Read<GTFS_Stop>(BasePath + GTFS_Stop.SourceFilename).ToList();

                    // Phase 1: Seed database
                    {
                        var bulkList = new List<EF_Stop>();
                        int iteration = 0;

                        foreach (var s in stops)
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
                        EF_Stop.BulkInsertEntities(bulkList);
                    }

                    // Phase 2: Set parent stations
                    using (var db = new EF_GtfsDbContext())
                    {
                        foreach (var e in db.Stops)
                        {
                            db_stops.TryAdd(e.OriginalId, e);
                        }
                        stops = stops.Where(s => (s.ParentStation != null) && (s.ParentStation.Length > 0)).ToList();

                        stops.ForEach(s => { db_stops[s.StopId].ParentStation = db_stops[s.ParentStation]; });
                        db.SaveChanges();
                        foreach (var e in db.Stops)
                        {
                            db_stops.TryAdd(e.OriginalId, e);
                        }
                    }
                }
                #endregion

                #region Shapes
                {
                    var shapes = GtfsReader.Read<GTFS_Shape>(BasePath + GTFS_Shape.SourceFilename);
                    var bulkList = new List<EF_Shape>();
                    int iteration = 0;

                    foreach (var s in shapes)
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

                    EF_Shape.BulkInsertEntities(bulkList);

                    using (var db = new EF_GtfsDbContext())
                    {
                        foreach (var e in db.Shapes)
                        {
                            db_shapes.TryAdd(e.OriginalId, e);
                        }
                    }
                }
                #endregion

                #region Routes
                {
                    var routes = GtfsReader.Read<GTFS_Route>(BasePath + GTFS_Route.SourceFilename);
                    var bulkList = new List<EF_Route>();
                    int iteration = 0;

                    foreach (var r in routes)
                    {
                        if (iteration > BATCH_INSERT_MAX)
                        {
                            iteration = 0;
                            EF_Route.BulkInsertEntities(bulkList);
                            bulkList = new List<EF_Route>();
                        }
                        bulkList.Add(new EF_Route(r, db_agencies[r.AgencyId]));
                        iteration++;
                    }
                    EF_Route.BulkInsertEntities(bulkList);

                    using (var db = new EF_GtfsDbContext())
                    {
                        foreach (var e in db.Routes)
                        {
                            db_routes.TryAdd(e.OriginalId, e);
                        }
                    }
                }
                #endregion

                #region Calendar Dates (Exceptions)
                {
                    var calendar_dates = GtfsReader.Read<GTFS_CalendarDate>(BasePath + GTFS_CalendarDate.SourceFilename);
                    var bulkList = new List<EF_CalendarDate>();
                    int iteration = 0;

                    foreach (var cd in calendar_dates)
                    {
                        if (iteration > BATCH_INSERT_MAX)
                        {
                            iteration = 0;
                            EF_CalendarDate.BulkInsertEntities(bulkList);
                            bulkList = new List<EF_CalendarDate>();
                        }
                        bulkList.Add(new EF_CalendarDate(cd, db_calendars[cd.ServiceId]));
                        iteration++;
                    }
                    EF_CalendarDate.BulkInsertEntities(bulkList);
                }
                #endregion

                #region Trips
                {
                    var trips = GtfsReader.Read<GTFS_Trip>(BasePath + GTFS_Trip.SourceFilename);
                    var bulkList = new List<EF_Trip>();
                    int iteration = 0;

                    foreach (var t in trips)
                    {
                        if (iteration > BATCH_INSERT_MAX)
                        {
                            iteration = 0;
                            EF_Trip.BulkInsertEntities(bulkList);
                            bulkList = new List<EF_Trip>();
                        }
                        bulkList.Add(new EF_Trip(t, db_routes[t.RouteId], db_calendars[t.ServiceId], db_shapes[t.ShapeId]));
                        iteration++;
                    }

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

                    var stop_times = GtfsReader.Read<GTFS_StopTime>(BasePath + GTFS_StopTime.SourceFilename);
                    var bulkList = new List<EF_StopTime>();
                    Task insertTask = null;

                    // non-parallel insertion (because of heavy memory usage)
                    int numInserted = 0;
                    foreach (var st in stop_times)
                    {
                        if (numInserted > BATCH_INSERT_MAX)
                        {
                            numInserted = 0;

                            if ((insertTask != null) && !insertTask.IsCompleted)
                            {
                                insertTask.Wait();
                            }
                            insertTask = Task.Factory.StartNew(() => EF_StopTime.BulkInsertEntities(bulkList));
                            bulkList = new List<EF_StopTime>();
                        }
                        bulkList.Add(new EF_StopTime(st, db_trips[st.TripId], db_stops[st.StopId]));
                        numInserted++;
                    }

                    if ((insertTask != null) && !insertTask.IsCompleted)
                    {
                        insertTask.Wait();
                    }
                    EF_StopTime.BulkInsertEntities(bulkList);
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
