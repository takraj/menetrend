using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTFSConverter
{
    class Program
    {
        static void Main(string[] args)
        {
            const string basedir = "G:\\budapest_gtfs\\";

            var totalTime = new Stopwatch();
            totalTime.Start();

            var db = new GTFSDB();

            var partialTime = new Stopwatch();
            /*partialTime.Start();
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

            partialTime.Restart();
            Console.Write("Serializing database...");
            using (var file = System.IO.File.Create(basedir + "gtfs.dat"))
            {
                ProtoBuf.Serializer.Serialize(file, db);
            }
            Console.WriteLine(" " + (partialTime.ElapsedMilliseconds / 1000.0) + "s");

            partialTime.Restart();
            Console.Write("Deserializing database...");
            using (var file = System.IO.File.OpenRead(basedir + "gtfs.dat"))
            {
                db = ProtoBuf.Serializer.Deserialize<GTFSDB>(file);
            }
            Console.WriteLine(" " + (partialTime.ElapsedMilliseconds / 1000.0) + "s");

            partialTime.Restart();
            Console.Write("Compacting database...");
            var cdb = new GTFSCompacter(db).MakeCompactModels();
            Console.WriteLine(" " + (partialTime.ElapsedMilliseconds / 1000.0) + "s");

            partialTime.Restart();
            Console.Write("Serializing compact database...");
            using (var file = System.IO.File.Create(basedir + "gtfs_compact.dat"))
            {
                ProtoBuf.Serializer.Serialize(file, cdb);
            }
            Console.WriteLine(" " + (partialTime.ElapsedMilliseconds / 1000.0) + "s"); */
            CompactGTFSDB cdb = null;

            partialTime.Restart();
            Console.Write("Deserializing compact database...");
            using (var file = System.IO.File.OpenRead(basedir + "gtfs_compact.dat"))
            {
                cdb = ProtoBuf.Serializer.Deserialize<CompactGTFSDB>(file);
            }
            Console.WriteLine(" " + (partialTime.ElapsedMilliseconds / 1000.0) + "s");

            partialTime.Stop();
            totalTime.Stop();

            Console.WriteLine();
            Console.WriteLine("Idő: " + (totalTime.ElapsedMilliseconds / 1000.0) + "s");
            Console.WriteLine("Nyomj meg egy billentyűt...");
            Console.ReadKey();
        }
    }
}
