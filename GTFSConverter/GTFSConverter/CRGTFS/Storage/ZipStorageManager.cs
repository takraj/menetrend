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
        private string rootDirectory;
        private Stop[] stops;
        private Route[] routes;
        private Trip[] trips;
        private string[] headsigns;

        public ZipStorageManager(string rootDirectory)
        {
            this.rootDirectory = rootDirectory;
        }

        public void CreateDatabase(TransitDB tdb)
        {
            #region Core
            using (ZipFile zip = new ZipFile())
            {
                using (var stream = new MemoryStream())
                {
                    ProtoBuf.Serializer.Serialize(stream, tdb.routes);
                    zip.AddEntry("routes.dat", stream.ToArray());
                }

                using (var stream = new MemoryStream())
                {
                    ProtoBuf.Serializer.Serialize(stream, tdb.trips);
                    zip.AddEntry("trips.dat", stream.ToArray());
                }

                using (var stream = new MemoryStream())
                {
                    ProtoBuf.Serializer.Serialize(stream, tdb.stops);
                    zip.AddEntry("stops.dat", stream.ToArray());
                }

                using (var stream = new MemoryStream())
                {
                    ProtoBuf.Serializer.Serialize(stream, tdb.headsigns);
                    zip.AddEntry("headsigns.dat", stream.ToArray());
                }

                zip.Save(Path.Combine(rootDirectory, "Core.zip"));
            }
            #endregion
            
            #region Shapes
            using (ZipFile zip = new ZipFile())
            {
                for (int i = 0; i < tdb.shapeMatrix.Count; i++)
                {
                    using (var stream = new MemoryStream())
                    {
                        ProtoBuf.Serializer.Serialize(stream, tdb.shapeMatrix.ElementAt(i));
                        zip.AddEntry("shape_" + i + ".dat", stream.ToArray());
                    }
                }

                zip.Save(Path.Combine(rootDirectory, "Shapes.zip"));
            }
            #endregion

            #region StopDistanceMatrix
            using (ZipFile zip = new ZipFile())
            {
                for (int i = 0; i < tdb.stops.Length; i++)
                {
                    var data = new List<float>();

                    for (int j = 0; j < tdb.stops.Length; j++)
                    {
                        data.Add(tdb.stopDistanceMatrix[(i * tdb.stops.Length) + j]);
                    }

                    using (var stream = new MemoryStream())
                    {
                        ProtoBuf.Serializer.Serialize(stream, data.ToArray());
                        zip.AddEntry("stop_" + i + ".dat", stream.ToArray());
                    }
                }

                zip.Save(Path.Combine(rootDirectory, "StopDistanceMatrix.zip"));
            }
            #endregion

            #region TripDates
            using (ZipFile zip = new ZipFile())
            {
                zip.CompressionLevel = CompressionLevel.None;

                foreach (var route in tdb.routeDatesMap.Keys)
                {
                    var routeDirName = "route_" + route.idx;

                    var dates = tdb.routeDatesMap[route].GroupBy(td => td.date).ToDictionary(td => td.Key, td => td.Select(tripdate => tripdate.tripIndex));

                    foreach (var date in dates.Keys)
                    {
                        using (var stream = new MemoryStream())
                        {
                            var data = dates[date].OrderBy(tripIndex => tdb.trips[tripIndex].stopTimes[0].arrivalTime);
                            ProtoBuf.Serializer.Serialize(stream, data.ToArray());
                            zip.AddEntry(routeDirName + "/trips_for_date_" + date + ".dat", stream.ToArray());
                        }
                    }
                }

                zip.Save(Path.Combine(rootDirectory, "TripDates.zip"));
            }
            #endregion
        }

        public void LoadDatabase()
        {
            throw new NotImplementedException();
        }

        public string GetHeadsign(int headsignIndex)
        {
            throw new NotImplementedException();
        }

        public Route GetRoute(int routeIndex)
        {
            throw new NotImplementedException();
        }

        public Stop GetStop(int stopIndex)
        {
            throw new NotImplementedException();
        }

        public Trip GetTrip(int tripIndex)
        {
            throw new NotImplementedException();
        }

        public ShapeVector GetShape(int shapeIndex)
        {
            throw new NotImplementedException();
        }

        public int[] GetStopDistanceVector(int stopIndex)
        {
            throw new NotImplementedException();
        }

        public TripDate[] GetTripsForDate(int routeIndex, ushort day)
        {
            throw new NotImplementedException();
        }

        public int CountOfStops
        {
            get { throw new NotImplementedException(); }
        }
    }
}
