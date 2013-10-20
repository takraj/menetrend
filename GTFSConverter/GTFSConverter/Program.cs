using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace GTFSConverter
{
    class Program
    {
        private static string basedir = @"C:\budapest_gtfs\";

        static void Main(string[] args)
        {
            var totalTime = new Stopwatch();
            totalTime.Start();

            //var db = ReadGTFS();
            //SerializeGTFS(ref db);
            //db = DeserializeBinaryGTFS();
            //var cdb = CreateCompactGTFS(db);
            //SerializeCompactGTFS(cdb);

            var cdb = DeserializeCompactGTFS();
            var tdb = CreateReferencedGTFS(cdb);
            SerializeTransitGTFS(tdb);

            tdb = DeserializeTransitGTFS();

            totalTime.Stop();
            Console.WriteLine();
            Console.WriteLine("Idő: " + (totalTime.ElapsedMilliseconds / 1000.0) + "s");
            Console.WriteLine("Nyomj meg egy billentyűt...");
            Console.ReadKey();
        }

        private static string CreateResourceDirectory(string dirname)
        {
            string path = Path.Combine(basedir, dirname);

            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }

            Directory.CreateDirectory(path);

            return path;
        }

        private static GTFSConverter.CRGTFS.TransitDB DeserializeTransitGTFS()
        {
            GTFSConverter.CRGTFS.TransitDB tdb = null;
            var partialTime = new Stopwatch();
            partialTime.Start();
            Console.Write("Deserializing transit database...");

            tdb = new CRGTFS.TransitDB();

            using (var file = System.IO.File.OpenRead(Path.Combine(basedir, "Core", "routes.dat")))
            {
                tdb.routes = ProtoBuf.Serializer.Deserialize<GTFSConverter.CRGTFS.Route[]>(file);
            }

            using (var file = System.IO.File.OpenRead(Path.Combine(basedir, "Core", "trips.dat")))
            {
                tdb.trips = ProtoBuf.Serializer.Deserialize<GTFSConverter.CRGTFS.Trip[]>(file);
            }

            using (var file = System.IO.File.OpenRead(Path.Combine(basedir, "Core", "stops.dat")))
            {
                tdb.stops = ProtoBuf.Serializer.Deserialize<GTFSConverter.CRGTFS.Stop[]>(file);
            }

            using (var file = System.IO.File.OpenRead(Path.Combine(basedir, "Core", "headsigns.dat")))
            {
                tdb.headsigns = ProtoBuf.Serializer.Deserialize<string[]>(file);
            }

            Console.WriteLine(" " + (partialTime.ElapsedMilliseconds / 1000.0) + "s");
            return tdb;
        }

        private static void SerializeTransitGTFS(GTFSConverter.CRGTFS.TransitDB tdb)
        {
            var partialTime = new Stopwatch();
            partialTime.Start();
            Console.Write("Serializing transit database...");

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

            var shapesdir = CreateResourceDirectory("Shapes");

            for (int i = 0; i < tdb.shapeMatrix.Count; i++ )
            {
                using (var file = System.IO.File.Create(Path.Combine(shapesdir, "shape_" + i + ".dat")))
                {
                    ProtoBuf.Serializer.Serialize(file, tdb.shapeMatrix.ElementAt(i));
                }
            }

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

            Console.WriteLine(" " + (partialTime.ElapsedMilliseconds / 1000.0) + "s");
        }

        private static GTFSConverter.CRGTFS.TransitDB CreateReferencedGTFS(CompactGTFSDB cdb)
        {
            var partialTime = new Stopwatch();
            partialTime.Start();
            Console.Write("Creating referenced database...");
            var tdb = new GTFSConverter.CRGTFS.RGTFSCompacter(cdb).CreateReferencedDB();
            Console.WriteLine(" " + (partialTime.ElapsedMilliseconds / 1000.0) + "s");

            return tdb;
        }

        private static CompactGTFSDB DeserializeCompactGTFS()
        {
            CompactGTFSDB cdb = null;
            var partialTime = new Stopwatch();
            partialTime.Start();
            Console.Write("Deserializing compact database...");
            using (var file = System.IO.File.OpenRead(basedir + "gtfs_compact.dat"))
            {
                cdb = ProtoBuf.Serializer.Deserialize<CompactGTFSDB>(file);
            }
            Console.WriteLine(" " + (partialTime.ElapsedMilliseconds / 1000.0) + "s");
            return cdb;
        }

        private static void SerializeCompactGTFS(CompactGTFSDB cdb)
        {
            var partialTime = new Stopwatch();
            partialTime.Start();
            Console.Write("Serializing compact database...");
            using (var file = System.IO.File.Create(basedir + "gtfs_compact.dat"))
            {
                ProtoBuf.Serializer.Serialize(file, cdb);
            }
            Console.WriteLine(" " + (partialTime.ElapsedMilliseconds / 1000.0) + "s");
        }

        private static CompactGTFSDB CreateCompactGTFS(GTFSDB db)
        {
            var partialTime = new Stopwatch();
            partialTime.Start();
            Console.Write("Compacting database...");
            var cdb = new GTFSCompacter(db).MakeCompactModels();
            Console.WriteLine(" " + (partialTime.ElapsedMilliseconds / 1000.0) + "s");

            return cdb;
        }

        private static GTFSDB DeserializeBinaryGTFS()
        {
            GTFSDB db = null;
            var partialTime = new Stopwatch();
            partialTime.Start();
            Console.Write("Deserializing database...");
            using (var file = System.IO.File.OpenRead(basedir + "gtfs.dat"))
            {
                db = ProtoBuf.Serializer.Deserialize<GTFSDB>(file);
            }
            Console.WriteLine(" " + (partialTime.ElapsedMilliseconds / 1000.0) + "s");
            return db;
        }

        private static void SerializeGTFS(ref GTFSDB db)
        {
            var partialTime = new Stopwatch();
            partialTime.Start();
            Console.Write("Serializing database...");
            using (var file = System.IO.File.Create(basedir + "gtfs.dat"))
            {
                ProtoBuf.Serializer.Serialize(file, db);
            }
            Console.WriteLine(" " + (partialTime.ElapsedMilliseconds / 1000.0) + "s");
        }

        private static GTFSDB ReadGTFS()
        {
            var db = new GTFSDB();

            var partialTime = new Stopwatch();

            partialTime.Start();
            Console.Write("Reading agencies...");
            db.agencies = GTFSReader.Read<GTFS_Agency>(basedir + GTFS_Agency.SourceFilename).ToList();
            Console.WriteLine(" " + (partialTime.ElapsedMilliseconds / 1000.0) + "s");

            partialTime.Restart();
            Console.Write("Reading calendars...");
            db.calendars = GTFSReader.Read<GTFS_Calendar>(basedir + GTFS_Calendar.SourceFilename).ToList();
            Console.WriteLine(" " + (partialTime.ElapsedMilliseconds / 1000.0) + "s");

            partialTime.Restart();
            Console.Write("Reading calendar dates...");
            db.calendar_dates = GTFSReader.Read<GTFS_CalendarDate>(basedir + GTFS_CalendarDate.SourceFilename).ToList();
            Console.WriteLine(" " + (partialTime.ElapsedMilliseconds / 1000.0) + "s");

            partialTime.Restart();
            Console.Write("Reading routes...");
            db.routes = GTFSReader.Read<GTFS_Route>(basedir + GTFS_Route.SourceFilename).ToList();
            Console.WriteLine(" " + (partialTime.ElapsedMilliseconds / 1000.0) + "s");

            partialTime.Restart();
            Console.Write("Reading shapes...");
            db.shapes = GTFSReader.Read<GTFS_Shape>(basedir + GTFS_Shape.SourceFilename).ToList();
            Console.WriteLine(" " + (partialTime.ElapsedMilliseconds / 1000.0) + "s");

            partialTime.Restart();
            Console.Write("Reading stops...");
            db.stops = GTFSReader.Read<GTFS_Stop>(basedir + GTFS_Stop.SourceFilename).ToList();
            Console.WriteLine(" " + (partialTime.ElapsedMilliseconds / 1000.0) + "s");

            partialTime.Restart();
            Console.Write("Reading stop times...");
            db.stop_times = GTFSReader.Read<GTFS_StopTime>(basedir + GTFS_StopTime.SourceFilename).ToList();
            Console.WriteLine(" " + (partialTime.ElapsedMilliseconds / 1000.0) + "s");

            partialTime.Restart();
            Console.Write("Reading trips...");
            db.trips = GTFSReader.Read<GTFS_Trip>(basedir + GTFS_Trip.SourceFilename).ToList();
            Console.WriteLine(" " + (partialTime.ElapsedMilliseconds / 1000.0) + "s");

            return db;
        }
    }
}
