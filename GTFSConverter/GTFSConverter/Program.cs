using GTFSConverter.CRGTFS.Pathfinder;
using GTFSConverter.CRGTFS.Storage;
using System;
using System.Diagnostics;
using System.Linq;

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

            //var cdb = DeserializeCompactGTFS();
            //var referencedGTFS = CreateReferencedGTFS(cdb);

            IStorageManager storageManager = new ZipStorageManager(basedir);
            //SerializeTransitGTFS(referencedGTFS, storageManager);

            DeserializeTransitGTFS(storageManager);
            CalculateShorthestRoute(storageManager);

            totalTime.Stop();
            Console.WriteLine();
            Console.WriteLine("Idő: " + (totalTime.ElapsedMilliseconds / 1000.0) + "s");
            Console.WriteLine("Nyomj meg egy billentyűt...");
            Console.ReadKey();
        }

        private static void CalculateShorthestRoute(IStorageManager storageManager)
        {
            var partialTime = new Stopwatch();
            partialTime.Start();

            Console.WriteLine("Gráf inicializálás...");
            var source = storageManager.GetStop(430);
            var destination = storageManager.GetStop(477);
            var graph = new TransitGraph(storageManager);
            var pathfinder = new ParallelAStarPathfinder(graph, storageManager.GetStopDistanceVector(destination.idx), 500);
            //var pathfinder = new ParallelDijkstraPathfinder(graph);
            Console.WriteLine("SOURCE: " + source.name + " (" + source.idx + ")");
            Console.WriteLine("DESTINATION: " + destination.name + " (" + destination.idx + ")");
            Console.WriteLine("Keresés...");
            try
            {
                var result = pathfinder.CalculateShortestRoute(source, destination, new DateTime(2013, 2, 15, 15, 0, 0));

                for (int i = 0; i < result.Count; i++)
                {
                    Console.WriteLine(result[i]);

                    if ((result[i] is GetOffAction) && (i < (result.Count - 1)))
                    {
                        var walkTime = graph.GetWalkingCostBetween(result[i].stop, result[i + 1].stop);
                        Console.WriteLine(String.Format("WalkTime: {0}", walkTime));
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            Console.WriteLine(" " + (partialTime.ElapsedMilliseconds / 1000.0) + "s");
        }

        private static void DeserializeTransitGTFS(IStorageManager storageManager)
        {
            var partialTime = new Stopwatch();
            partialTime.Start();
            Console.Write("Deserializing transit database...");

            storageManager.LoadDatabase();

            Console.WriteLine(" " + (partialTime.ElapsedMilliseconds / 1000.0) + "s");
        }

        private static void SerializeTransitGTFS(GTFSConverter.CRGTFS.TransitDB tdb, IStorageManager storageManager)
        {
            var partialTime = new Stopwatch();
            partialTime.Start();
            Console.Write("Serializing transit database...");

            storageManager.CreateDatabase(tdb);

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
