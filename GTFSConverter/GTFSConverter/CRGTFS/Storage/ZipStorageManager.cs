using Ionic.Zip;
using Ionic.Zlib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTFSConverter.CRGTFS.Storage
{
    class ZipStorageManager : IStorageManager
    {
        private bool useNoCompression;

        private string rootDirectory;
        private Stop[] stops;
        private Route[] routes;
        private Trip[] trips;
        private string[] headsigns;

        #region Constants
        private const string CORE_ZIP = "Core.zip";
        private const string SHAPES_ZIP = "Shapes.zip";
        private const string STOP_DISTANCE_MATRIX_ZIP = "StopDistanceMatrix.zip";
        private const string TRIP_DATES_ZIP = "TripDates.zip";

        private const string ROUTES_DAT = "routes.dat";
        private const string TRIPS_DAT = "trips.dat";
        private const string STOPS_DAT = "stops.dat";
        private const string HEADSIGNS_DAT = "headsigns.dat";

        private const string SHAPE_FS_DAT = "shape_{0}.dat";
        private const string STOP_FS_DAT = "stop_{0}.dat";
        private const string TRIPS_FOR_DATE_FS_DAT = "route_{0}_trips_for_date_{1}.dat";
        #endregion

        public ZipStorageManager(string rootDirectory, bool useNoCompression = false)
        {
            this.rootDirectory = rootDirectory;
            this.useNoCompression = useNoCompression;
        }

        public void CreateDatabase(TransitDB tdb)
        {
            #region Core
            using (ZipFile zip = new ZipFile())
            {
                if (this.useNoCompression)
                {
                    zip.CompressionLevel = CompressionLevel.None;
                }

                using (var stream = new MemoryStream())
                {
                    ProtoBuf.Serializer.Serialize(stream, tdb.routes);
                    zip.AddEntry(ROUTES_DAT, stream.ToArray());
                }

                using (var stream = new MemoryStream())
                {
                    ProtoBuf.Serializer.Serialize(stream, tdb.trips);
                    zip.AddEntry(TRIPS_DAT, stream.ToArray());
                }

                using (var stream = new MemoryStream())
                {
                    ProtoBuf.Serializer.Serialize(stream, tdb.stops);
                    zip.AddEntry(STOPS_DAT, stream.ToArray());
                }

                using (var stream = new MemoryStream())
                {
                    ProtoBuf.Serializer.Serialize(stream, tdb.headsigns);
                    zip.AddEntry(HEADSIGNS_DAT, stream.ToArray());
                }

                zip.Save(Path.Combine(rootDirectory, CORE_ZIP));
            }
            #endregion
            
            #region Shapes
            using (ZipFile zip = new ZipFile())
            {
                if (this.useNoCompression)
                {
                    zip.CompressionLevel = CompressionLevel.None;
                }

                for (int i = 0; i < tdb.shapeMatrix.Count; i++)
                {
                    using (var stream = new MemoryStream())
                    {
                        ProtoBuf.Serializer.Serialize(stream, tdb.shapeMatrix.ElementAt(i));
                        zip.AddEntry(String.Format(SHAPE_FS_DAT, i), stream.ToArray());
                    }
                }

                zip.Save(Path.Combine(rootDirectory, SHAPES_ZIP));
            }
            #endregion

            #region StopDistanceMatrix
            using (ZipFile zip = new ZipFile())
            {
                if (this.useNoCompression)
                {
                    zip.CompressionLevel = CompressionLevel.None;
                }

                for (int i = 0; i < tdb.stops.Length; i++)
                {
                    var data = new List<int>();

                    for (int j = 0; j < tdb.stops.Length; j++)
                    {
                        data.Add(tdb.stopDistanceMatrix[(i * tdb.stops.Length) + j]);
                    }

                    using (var stream = new MemoryStream())
                    {
                        ProtoBuf.Serializer.Serialize(stream, data.ToArray());
                        zip.AddEntry(String.Format(STOP_FS_DAT, i), stream.ToArray());
                    }
                }

                zip.Save(Path.Combine(rootDirectory, STOP_DISTANCE_MATRIX_ZIP));
            }
            #endregion

            #region TripDates
            using (ZipFile zip = new ZipFile())
            {
                zip.CompressionLevel = CompressionLevel.BestSpeed;

                if (this.useNoCompression)
                {
                    zip.CompressionLevel = CompressionLevel.None;
                }

                foreach (var route in tdb.routeDatesMap.Keys)
                {
                    var dates = tdb.routeDatesMap[route].GroupBy(td => td.date).ToDictionary(
                        td => td.Key, td => td.Select(tripdate => tripdate.tripIndex));

                    foreach (var date in dates.Keys)
                    {
                        using (var stream = new MemoryStream())
                        {
                            var data = dates[date].OrderBy(tripIndex => tdb.trips[tripIndex].stopTimes[0].arrivalTime);
                            ProtoBuf.Serializer.Serialize(stream, data.ToArray());
                            zip.AddEntry(String.Format(TRIPS_FOR_DATE_FS_DAT, route.idx, date), stream.ToArray());
                        }
                    }
                }

                zip.Save(Path.Combine(rootDirectory, TRIP_DATES_ZIP));
            }
            #endregion
        }

        public void LoadDatabase()
        {
            using (var zip = new ZipFile(Path.Combine(rootDirectory, CORE_ZIP)))
            {
                using (var stream = new MemoryStream())
                {
                    zip[ROUTES_DAT].Extract(stream);
                    stream.Position = 0;
                    routes = ProtoBuf.Serializer.Deserialize<Route[]>(stream);
                }

                using (var stream = new MemoryStream())
                {
                    zip[TRIPS_DAT].Extract(stream);
                    stream.Position = 0;
                    trips = ProtoBuf.Serializer.Deserialize<Trip[]>(stream);
                }

                using (var stream = new MemoryStream())
                {
                    zip[STOPS_DAT].Extract(stream);
                    stream.Position = 0;
                    stops = ProtoBuf.Serializer.Deserialize<Stop[]>(stream);
                }

                using (var stream = new MemoryStream())
                {
                    zip[HEADSIGNS_DAT].Extract(stream);
                    stream.Position = 0;
                    headsigns = ProtoBuf.Serializer.Deserialize<string[]>(stream);
                }
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
            ShapeVector result;

            using (var zip = new ZipFile(Path.Combine(rootDirectory, SHAPES_ZIP)))
            {
                using (var stream = new MemoryStream())
                {
                    zip[String.Format(SHAPE_FS_DAT, shapeIndex)].Extract(stream);
                    stream.Position = 0;
                    result = ProtoBuf.Serializer.Deserialize<ShapeVector>(stream);
                }
            }

            return result;
        }

        public int[] GetStopDistanceVector(int stopIndex)
        {
            int[] result;

            using (var zip = new ZipFile(Path.Combine(rootDirectory, STOP_DISTANCE_MATRIX_ZIP)))
            {
                using (var stream = new MemoryStream())
                {
                    zip[String.Format(STOP_FS_DAT, stopIndex)].Extract(stream);
                    stream.Position = 0;
                    result = ProtoBuf.Serializer.Deserialize<int[]>(stream);
                }
            }

            return result;
        }

        public TripDate[] GetTripsForDate(int routeIndex, ushort day)
        {
            TripDate[] result;

            using (var zip = new ZipFile(Path.Combine(rootDirectory, TRIP_DATES_ZIP)))
            {
                using (var stream = new MemoryStream())
                {
                    zip[String.Format(TRIPS_FOR_DATE_FS_DAT, routeIndex, day)].Extract(stream);
                    stream.Position = 0;
                    result = ProtoBuf.Serializer.Deserialize<int[]>(stream)
                        .Select(idx => new TripDate { tripIndex = idx, date = day }).ToArray();
                }
            }

            return result;
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
            return new ShapeVectorEnumerator(Path.Combine(rootDirectory, SHAPES_ZIP), SHAPE_FS_DAT);
        }

        public IEnumerator<int[]> CreateStopDistanceVectorEnumerator()
        {
            return new StopDistanceVectorEnumerator(Path.Combine(rootDirectory, STOP_DISTANCE_MATRIX_ZIP), STOP_FS_DAT);
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
                int result;

                using (var zip = new ZipFile(Path.Combine(rootDirectory, SHAPES_ZIP)))
                {
                    result = zip.Count;
                }

                return new Proxies.ShapeVectorListProxy(this, result);
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
        public abstract class AbstractZipEnumerator<T> : IEnumerator<T>
        {
            protected ZipFile zip;
            protected string pattern;
            private int count;
            private int i;

            public AbstractZipEnumerator(string filename, string pattern)
            {
                this.zip = new ZipFile(filename);
                this.pattern = pattern;
                this.count = zip.Count;
            }

            public T Current
            {
                get { return this.GetElement(this.i); }
            }

            public void Dispose()
            {
                zip.Dispose();
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

        public class ShapeVectorEnumerator : AbstractZipEnumerator<ShapeVector>
        {
            public ShapeVectorEnumerator(string filename, string pattern)
                : base(filename, pattern) {}

            public override ShapeVector GetElement(int i)
            {
                ShapeVector result;

                using (var stream = new MemoryStream())
                {
                    zip[String.Format(pattern, i)].Extract(stream);
                    stream.Position = 0;
                    result = ProtoBuf.Serializer.Deserialize<ShapeVector>(stream);
                }

                return result;
            }
        }

        public class StopDistanceVectorEnumerator : AbstractZipEnumerator<int[]>
        {
            public StopDistanceVectorEnumerator(string filename, string pattern)
                : base(filename, pattern) {}

            public override int[] GetElement(int i)
            {
                int[] result;

                using (var stream = new MemoryStream())
                {
                    zip[String.Format(pattern, i)].Extract(stream);
                    stream.Position = 0;
                    result = ProtoBuf.Serializer.Deserialize<int[]>(stream);
                }

                return result;
            }
        }
        #endregion
    }
}
