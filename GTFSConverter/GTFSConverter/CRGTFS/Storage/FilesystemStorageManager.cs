using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTFSConverter.CRGTFS.Storage
{
    public class FilesystemStorageManager : IStorageManager
    {
        private string rootDirectory;
        private Stop[] stops;
        private Route[] routes;
        private Trip[] trips;
        private string[] headsigns;

        #region Constants
        private const string CORE_DIR = "Core";
        private const string SHAPES_DIR = "Shapes";
        private const string STOP_DISTANCE_MATRIX_DIR = "StopDistanceMatrix";
        private const string TRIP_DATES_DIR = "TripDates";

        private const string ROUTES_DAT = "routes.dat";
        private const string TRIPS_DAT = "trips.dat";
        private const string STOPS_DAT = "stops.dat";
        private const string HEADSIGNS_DAT = "headsigns.dat";

        private const string SHAPE_FS_DAT = "shape_{0}.dat";
        private const string STOP_FS_DAT = "stop_{0}.dat";
        private const string ROUTE_FS_DIR = "route_{0}.dat";
        private const string TRIPS_FOR_DATE_FS_DAT = "trips_for_date_{0}.dat";
        #endregion

        public FilesystemStorageManager(string rootDirectory)
        {
            this.rootDirectory = rootDirectory;
        }

        #region Helper methods
        /// <summary>
        /// Törli és újra létrehozza az adott erőforrás mappát.
        /// Ha nem létezett, akkor csak létrehozza.
        /// </summary>
        /// <param name="dirname">A létrehozandó erőforrás mappa</param>
        /// <returns>Az erőforrás mappa teljes elérési útja</returns>
        private string CreateResourceDirectory(string dirname)
        {
            string path = Path.Combine(rootDirectory, dirname);

            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }

            Directory.CreateDirectory(path);

            return path;
        }
        #endregion

        public void CreateDatabase(TransitDB tdb)
        {
            #region Core
            {
                var coredir = CreateResourceDirectory(CORE_DIR);

                using (var file = System.IO.File.Create(Path.Combine(coredir, ROUTES_DAT)))
                {
                    ProtoBuf.Serializer.Serialize(file, tdb.routes);
                }

                using (var file = System.IO.File.Create(Path.Combine(coredir, TRIPS_DAT)))
                {
                    ProtoBuf.Serializer.Serialize(file, tdb.trips);
                }

                using (var file = System.IO.File.Create(Path.Combine(coredir, STOPS_DAT)))
                {
                    ProtoBuf.Serializer.Serialize(file, tdb.stops);
                }

                using (var file = System.IO.File.Create(Path.Combine(coredir, HEADSIGNS_DAT)))
                {
                    ProtoBuf.Serializer.Serialize(file, tdb.headsigns);
                }
            }
            #endregion

            #region Shapes
            {
                var shapesdir = CreateResourceDirectory(SHAPES_DIR);

                for (int i = 0; i < tdb.shapeMatrix.Count; i++)
                {
                    string filename = String.Format(SHAPE_FS_DAT, i);
                    using (var file = System.IO.File.Create(Path.Combine(shapesdir, filename)))
                    {
                        ProtoBuf.Serializer.Serialize(file, tdb.shapeMatrix.ElementAt(i));
                    }
                }
            }
            #endregion

            #region StopDistanceMatrix
            {
                var dmatrixdir = CreateResourceDirectory(STOP_DISTANCE_MATRIX_DIR);

                for (int i = 0; i < tdb.stops.Length; i++)
                {
                    var data = new List<float>();

                    for (int j = 0; j < tdb.stops.Length; j++)
                    {
                        data.Add(tdb.stopDistanceMatrix[(i * tdb.stops.Length) + j]);
                    }

                    string filename = String.Format(STOP_FS_DAT, i);
                    using (var file = System.IO.File.Create(Path.Combine(dmatrixdir, filename)))
                    {
                        ProtoBuf.Serializer.Serialize(file, data.ToArray());
                    }
                }
            }
            #endregion

            #region TripDates
            {
                var tripDatesDir = CreateResourceDirectory(TRIP_DATES_DIR);

                foreach (var route in tdb.routeDatesMap.Keys)
                {
                    string routeDirName = String.Format(ROUTE_FS_DIR, route.idx);
                    var routeDir = CreateResourceDirectory(Path.Combine(tripDatesDir, routeDirName));

                    var dates = tdb.routeDatesMap[route].GroupBy(td => td.date).ToDictionary(
                        td => td.Key, td => td.Select(tripdate => tripdate.tripIndex));

                    foreach (var date in dates.Keys)
                    {
                        string filename = String.Format(TRIPS_FOR_DATE_FS_DAT, date);
                        using (var file = System.IO.File.Create(Path.Combine(routeDir, filename)))
                        {
                            var data = dates[date].OrderBy(tripIndex => tdb.trips[tripIndex].stopTimes[0].arrivalTime);
                            ProtoBuf.Serializer.Serialize(file, data.ToArray());
                        }
                    }
                }
            }
            #endregion
        }

        public void LoadDatabase()
        {
            using (var file = System.IO.File.OpenRead(Path.Combine(rootDirectory, CORE_DIR, ROUTES_DAT)))
            {
                routes = ProtoBuf.Serializer.Deserialize<Route[]>(file);
            }

            using (var file = System.IO.File.OpenRead(Path.Combine(rootDirectory, CORE_DIR, TRIPS_DAT)))
            {
                trips = ProtoBuf.Serializer.Deserialize<Trip[]>(file);
            }

            using (var file = System.IO.File.OpenRead(Path.Combine(rootDirectory, CORE_DIR, STOPS_DAT)))
            {
                stops = ProtoBuf.Serializer.Deserialize<Stop[]>(file);
            }

            using (var file = System.IO.File.OpenRead(Path.Combine(rootDirectory, CORE_DIR, HEADSIGNS_DAT)))
            {
                headsigns = ProtoBuf.Serializer.Deserialize<string[]>(file);
            }
        }

        public string GetHeadsign(int headsignIndex)
        {
            return headsigns[headsignIndex];
        }

        public Route GetRoute(int routeIndex)
        {
            return routes[routeIndex];
        }

        public Stop GetStop(int stopIndex)
        {
            return stops[stopIndex];
        }

        public Trip GetTrip(int tripIndex)
        {
            return trips[tripIndex];
        }

        public ShapeVector GetShape(int shapeIndex)
        {
            string shapeFilename = String.Format(SHAPE_FS_DAT, shapeIndex);
            ShapeVector shapeVector;

            using (var file = System.IO.File.OpenRead(Path.Combine(rootDirectory, SHAPES_DIR, shapeFilename)))
            {
                shapeVector = ProtoBuf.Serializer.Deserialize<ShapeVector>(file);
            }

            return shapeVector;
        }

        public int[] GetStopDistanceVector(int stopIndex)
        {
            string vectorFilename = String.Format(STOP_FS_DAT, stopIndex);
            int[] dstVector;

            using (var file = System.IO.File.OpenRead(Path.Combine(rootDirectory, STOP_DISTANCE_MATRIX_DIR, vectorFilename)))
            {
                dstVector = ProtoBuf.Serializer.Deserialize<int[]>(file);
            }

            return dstVector;
        }

        public TripDate[] GetTripsForDate(int routeIndex, ushort day)
        {
            string dateFilename = String.Format(TRIPS_FOR_DATE_FS_DAT, day);
            string routeFolder = String.Format(ROUTE_FS_DIR, routeIndex);
            TripDate[] dateVector;

            using (var file = System.IO.File.OpenRead(Path.Combine(rootDirectory, TRIP_DATES_DIR, routeFolder, dateFilename)))
            {
                dateVector = ProtoBuf.Serializer.Deserialize<TripDate[]>(file);
            }

            return dateVector;
        }

        public int CountOfStops
        {
            get { return stops.Length; }
        }

        #region Enumerator Factory
        public IEnumerator<Stop> CreateStopEnumerator()
        {
            return ((IEnumerable<Stop>)stops).GetEnumerator();
        }

        public IEnumerator<Trip> CreateTripEnumerator()
        {
            return ((IEnumerable<Trip>)trips).GetEnumerator();
        }

        public IEnumerator<ShapeVector> CreateShapeVectorEnumerator()
        {
            return new ShapeVectorEnumerator(Path.Combine(rootDirectory, "Shapes"), "shape_{0}.dat");
        }

        public IEnumerator<int[]> CreateStopDistanceVectorEnumerator()
        {
            return new StopDistanceVectorEnumerator(Path.Combine(rootDirectory, "StopDistanceMatrix"), "stop_{0}.dat");
        }

        public IEnumerator<string> CreateHeadsignEnumerator()
        {
            return ((IEnumerable<string>)headsigns).GetEnumerator();
        }

        public IEnumerator<Route> CreateRouteEnumerator()
        {
            return ((IEnumerable<Route>)routes).GetEnumerator();
        }
        #endregion

        #region Enumerated Properties
        public Proxies.StopListProxy Stops
        {
            get { return new Proxies.StopListProxy(this, stops.Length); }
        }

        public Proxies.TripListProxy Trips
        {
            get { return new Proxies.TripListProxy(this, trips.Length); }
        }

        public Proxies.ShapeVectorListProxy Shapes
        {
            get
            {
                string dir = Path.Combine(rootDirectory, "Shapes");
                int fileCount = Directory.GetFiles(dir, "*.dat", SearchOption.TopDirectoryOnly).Length;
                return new Proxies.ShapeVectorListProxy(this, fileCount);
            }
        }

        public Proxies.StopDistanceVectorListProxy StopDistanceMatrix
        {
            get { return new Proxies.StopDistanceVectorListProxy(this, stops.Length); }
        }

        public Proxies.HeadsignListProxy Headsigns
        {
            get { return new Proxies.HeadsignListProxy(this, headsigns.Length); }
        }

        public Proxies.RouteListProxy Routes
        {
            get { return new Proxies.RouteListProxy(this, routes.Length); }
        }
        #endregion

        #region Enumerator Implementations
        public abstract class AbstractDirectoryEnumerator<T> : IEnumerator<T>
        {
            protected string directory;
            protected string pattern;
            private int count;
            private int i;

            public AbstractDirectoryEnumerator(string directory, string pattern)
            {
                this.directory = directory;
                this.pattern = pattern;
                this.count = Directory.GetFiles(directory, "*.dat", SearchOption.TopDirectoryOnly).Length;
            }

            public T Current
            {
                get { return this.GetElement(this.i); }
            }

            public void Dispose()
            {
                return;
            }

            object System.Collections.IEnumerator.Current
            {
                get { return this.Current; }
            }

            public bool MoveNext()
            {
                this.i++;
                return (this.i < this.count);
            }

            public void Reset()
            {
                this.i = 0;
            }

            public abstract T GetElement(int i);
        }

        public class ShapeVectorEnumerator : AbstractDirectoryEnumerator<ShapeVector>
        {
            public ShapeVectorEnumerator(string directory, string pattern)
                : base(directory, pattern) { }

            public override ShapeVector GetElement(int i)
            {
                ShapeVector result;

                using (var stream = File.OpenRead(Path.Combine(directory, String.Format(pattern, i))))
                {
                    result = ProtoBuf.Serializer.Deserialize<ShapeVector>(stream);
                }

                return result;
            }
        }

        public class StopDistanceVectorEnumerator : AbstractDirectoryEnumerator<int[]>
        {
            public StopDistanceVectorEnumerator(string directory, string pattern)
                : base(directory, pattern) { }

            public override int[] GetElement(int i)
            {
                int[] result;

                using (var stream = File.OpenRead(Path.Combine(directory, String.Format(pattern, i))))
                {
                    result = ProtoBuf.Serializer.Deserialize<int[]>(stream);
                }

                return result;
            }
        }
        #endregion
    }
}
