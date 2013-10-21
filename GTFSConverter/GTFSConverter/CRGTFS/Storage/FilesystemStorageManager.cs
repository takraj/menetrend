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
                var coredir = CreateResourceDirectory("Core");

                using (var file = System.IO.File.Create(Path.Combine(coredir, "routes.dat")))
                {
                    ProtoBuf.Serializer.Serialize(file, tdb.routes);
                }

                using (var file = System.IO.File.Create(Path.Combine(coredir, "trips.dat")))
                {
                    ProtoBuf.Serializer.Serialize(file, tdb.trips);
                }

                using (var file = System.IO.File.Create(Path.Combine(coredir, "stops.dat")))
                {
                    ProtoBuf.Serializer.Serialize(file, tdb.stops);
                }

                using (var file = System.IO.File.Create(Path.Combine(coredir, "headsigns.dat")))
                {
                    ProtoBuf.Serializer.Serialize(file, tdb.headsigns);
                }
            }
            #endregion

            #region Shapes
            {
                var shapesdir = CreateResourceDirectory("Shapes");

                for (int i = 0; i < tdb.shapeMatrix.Count; i++)
                {
                    using (var file = System.IO.File.Create(Path.Combine(shapesdir, "shape_" + i + ".dat")))
                    {
                        ProtoBuf.Serializer.Serialize(file, tdb.shapeMatrix.ElementAt(i));
                    }
                }
            }
            #endregion

            #region StopDistanceMatrix
            {
                var dmatrixdir = CreateResourceDirectory("StopDistanceMatrix");

                for (int i = 0; i < tdb.stops.Length; i++)
                {
                    var data = new List<float>();

                    for (int j = 0; j < tdb.stops.Length; j++)
                    {
                        data.Add(tdb.stopDistanceMatrix[(i * tdb.stops.Length) + j]);
                    }

                    using (var file = System.IO.File.Create(Path.Combine(dmatrixdir, "stop_" + i + ".dat")))
                    {
                        ProtoBuf.Serializer.Serialize(file, data.ToArray());
                    }
                }
            }
            #endregion

            #region TripDates
            {
                var tripDatesDir = CreateResourceDirectory("TripDates");

                foreach (var route in tdb.routeDatesMap.Keys)
                {
                    var routeDir = CreateResourceDirectory(Path.Combine(tripDatesDir, "route_" + route.idx));

                    var dates = tdb.routeDatesMap[route].GroupBy(td => td.date).ToDictionary(td => td.Key, td => td.Select(tripdate => tripdate.tripIndex));

                    foreach (var date in dates.Keys)
                    {
                        using (var file = System.IO.File.Create(Path.Combine(routeDir, "trips_for_date_" + date + ".dat")))
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
            using (var file = System.IO.File.OpenRead(Path.Combine(rootDirectory, "Core", "routes.dat")))
            {
                routes = ProtoBuf.Serializer.Deserialize<Route[]>(file);
            }

            using (var file = System.IO.File.OpenRead(Path.Combine(rootDirectory, "Core", "trips.dat")))
            {
                trips = ProtoBuf.Serializer.Deserialize<Trip[]>(file);
            }

            using (var file = System.IO.File.OpenRead(Path.Combine(rootDirectory, "Core", "stops.dat")))
            {
                stops = ProtoBuf.Serializer.Deserialize<Stop[]>(file);
            }

            using (var file = System.IO.File.OpenRead(Path.Combine(rootDirectory, "Core", "headsigns.dat")))
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
            string shapeFilename = "shape_" + shapeIndex + ".dat";
            ShapeVector shapeVector;

            using (var file = System.IO.File.OpenRead(Path.Combine(rootDirectory, "Shapes", shapeFilename)))
            {
                shapeVector = ProtoBuf.Serializer.Deserialize<ShapeVector>(file);
            }

            return shapeVector;
        }

        public int[] GetStopDistanceVector(int stopIndex)
        {
            string vectorFilename = "stop_" + stopIndex + ".dat";
            int[] dstVector;

            using (var file = System.IO.File.OpenRead(Path.Combine(rootDirectory, "StopDistanceMatrix", vectorFilename)))
            {
                dstVector = ProtoBuf.Serializer.Deserialize<int[]>(file);
            }

            return dstVector;
        }

        public TripDate[] GetTripsForDate(int routeIndex, ushort day)
        {
            string dateFilename = "trips_for_date_" + day + ".dat";
            string routeFolder = "route_" + routeIndex;
            TripDate[] dateVector;

            using (var file = System.IO.File.OpenRead(Path.Combine(rootDirectory, "TripDates", routeFolder, dateFilename)))
            {
                dateVector = ProtoBuf.Serializer.Deserialize<TripDate[]>(file);
            }

            return dateVector;
        }

        public int CountOfStops
        {
            get { return stops.Length; }
        }
    }
}
