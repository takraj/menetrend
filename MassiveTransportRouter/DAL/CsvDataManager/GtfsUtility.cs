using CsvHelper.Configuration;
using MTR.DataAccess.EFDataManager;
using MTR.DataAccess.EFDataManager.Entities;
using MTR.DataAccess.Models;
using System;
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
        private static List<GTFS_Agency> agencies = new List<GTFS_Agency>();
        private static List<GTFS_Route> routes = new List<GTFS_Route>();
        private static List<GTFS_Trip> trips = new List<GTFS_Trip>();
        private static List<GTFS_StopTime> stop_times = new List<GTFS_StopTime>();
        private static List<GTFS_Stop> stops = new List<GTFS_Stop>();
        private static List<GTFS_CalendarDate> calendar_dates = new List<GTFS_CalendarDate>();
        private static List<GTFS_Calendar> calendars = new List<GTFS_Calendar>();
        private static List<GTFS_Shape> shapes = new List<GTFS_Shape>();

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
                db.Agencies.ToList();
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
            #region DatabaseSeed
            {
                #region Agency
                agencies.AddRange(GtfsReader.Read<GTFS_Agency>(BasePath + GTFS_Agency.SourceFilename));

                using (var db = new EF_GtfsDbContext())
                {
                    agencies.ForEach(a => db.Agencies.Add(new EF_Agency(a)));
                    db.SaveChanges();
                }
                #endregion

                #region Calendars
                calendars.AddRange(GtfsReader.Read<GTFS_Calendar>(BasePath + GTFS_Calendar.SourceFilename));

                using (var db = new EF_GtfsDbContext())
                {
                    calendars.ForEach(c => db.Calendars.Add(new EF_Calendar(c)));
                    db.SaveChanges();
                }
                #endregion

                #region Routes
                routes.AddRange(GtfsReader.Read<GTFS_Route>(BasePath + GTFS_Route.SourceFilename));

                // Phase 1: Seed
                using (var db = new EF_GtfsDbContext())
                {
                    routes.ForEach(r => db.Routes.Add(new EF_Route(r)));
                    db.SaveChanges();
                }

                // Phase 2: Match
                using (var db = new EF_GtfsDbContext())
                {
                    foreach (GTFS_Route r in routes.Where(e => (e.AgencyId != null) && (e.AgencyId.Length > 0)))
                    {
                        var agency = db.Agencies.First(e => e.OriginalId.Equals(r.AgencyId));
                        db.Routes.First(e => e.OriginalId.Equals(r.RouteId)).AgencyId = agency;
                    }
                    db.SaveChanges();
                }
                #endregion

                #region Calendar Dates (Exceptions)
                calendar_dates.AddRange(GtfsReader.Read<GTFS_CalendarDate>(BasePath + GTFS_CalendarDate.SourceFilename));

                using (var db = new EF_GtfsDbContext())
                {
                    calendar_dates.ForEach(cd => db.CalendarDates.Add(new EF_CalendarDate(cd, db.Calendars.First(c => c.OriginalId.Equals(cd.ServiceId)))));
                    db.SaveChanges();
                }
                #endregion

                #region Stops
                stops.AddRange(GtfsReader.Read<GTFS_Stop>(BasePath + GTFS_Stop.SourceFilename));

                // Phase 1: Seed database
                using (var db = new EF_GtfsDbContext())
                {
                    stops.ForEach(s => db.Stops.Add(new EF_Stop(s)));
                    db.SaveChanges();
                }

                // Phase 2: Set parent stations
                using (var db = new EF_GtfsDbContext())
                {
                    foreach (GTFS_Stop s in stops.Where(s => (s.ParentStation != null) && (s.ParentStation.Length > 0)))
                    {
                        var parent = db.Stops.First(e => e.OriginalId.Equals(s.ParentStation));
                        var child = db.Stops.First(e => e.OriginalId.Equals(s.StopId));
                        child.ParentStation = parent;
                    }
                    db.SaveChanges();
                }
                #endregion

                #region Shapes
                shapes.AddRange(GtfsReader.Read<GTFS_Shape>(BasePath + GTFS_Shape.SourceFilename));

                using (var db = new EF_GtfsDbContext())
                {
                    //shapes.ForEach(s => db.Shapes.Add(new EF_Shape(s)));
                    //db.SaveChanges();
                }
                #endregion

                #region Trips
                trips.AddRange(GtfsReader.Read<GTFS_Trip>(BasePath + GTFS_Trip.SourceFilename));

                using (var db = new EF_GtfsDbContext())
                {
                    //trips.ForEach(t => db.Trips.Add(new EF_Trip(t, db.Routes.First(r => r.OriginalId.Equals(t.RouteId)), db.Calendars.First(c => c.OriginalId.Equals(t.ServiceId)), db.Shapes.First(s => s.OriginalId.Equals(t.ShapeId)))));
                    //db.SaveChanges();
                }
                #endregion

                #region Stop Times
                stop_times.AddRange(GtfsReader.Read<GTFS_StopTime>(BasePath + GTFS_StopTime.SourceFilename));

                using (var db = new EF_GtfsDbContext())
                {
                    //stop_times.ForEach(st => db.StopTimes.Add(new EF_StopTime(st, db.Trips.First(t => t.OriginalId.Equals(st.TripId)), db.Stops.First(s => s.OriginalId.Equals(st.StopId)))));
                    //db.SaveChanges();
                }
                #endregion
            }
            #endregion
        }

        public static List<MTR.BusinessLogic.Common.POCO.Stop> GetAllStops()
        {
            var result = new List<MTR.BusinessLogic.Common.POCO.Stop>();

            foreach (GTFS_Stop s in stops)
            {
                result.Add(new BusinessLogic.Common.POCO.Stop(s.StopId, s.StopName, s.StopLatitude, s.StopLongitude, s.LocationType, s.ParentStation, s.WheelchairBoarding));
            }

            return result;
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
