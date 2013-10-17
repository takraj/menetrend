using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace GTFSConverter
{
    [ProtoBuf.ProtoContract]
    struct Proba
    {
        [ProtoBuf.ProtoMember(1)]
        public bool thisIsAFlag;

        [ProtoBuf.ProtoMember(2)]
        public bool thisIsAnotherFlag;

        [ProtoBuf.ProtoMember(3)]
        public string thisIsAString;

        [ProtoBuf.ProtoMember(4)]
        public byte thisIsAByte;

        [ProtoBuf.ProtoMember(5)]
        public ulong thisIsALong;
    }

    [ProtoBuf.ProtoContract]
    struct TwoBooleansProba
    {
        [ProtoBuf.ProtoMember(1)]
        public bool thisIsAFlag;

        [ProtoBuf.ProtoMember(2)]
        public bool thisIsAnotherFlag;
    }

    [ProtoBuf.ProtoContract]
    struct TwoBytesProba
    {
        [ProtoBuf.ProtoMember(1)]
        public byte thisIsAByte;

        [ProtoBuf.ProtoMember(2)]
        public byte thisIsAnotherByte;
    }

    [ProtoBuf.ProtoContract]
    struct ThreeBooleansProba
    {
        [ProtoBuf.ProtoMember(1)]
        public bool thisIsAFlag;

        [ProtoBuf.ProtoMember(2)]
        public bool thisIsAnotherFlag;

        [ProtoBuf.ProtoMember(3)]
        public bool thisIsTheThirdFlag;
    }

    [ProtoBuf.ProtoContract]
    struct ThreeBytesProba
    {
        [ProtoBuf.ProtoMember(1)]
        public byte thisIsAByte;

        [ProtoBuf.ProtoMember(2)]
        public byte thisIsAnotherByte;

        [ProtoBuf.ProtoMember(3)]
        public byte thisIsTheThirdByte;
    }

    class Program
    {
        private static string basedir = "C:\\budapest_gtfs\\";

        static void testDataStructures()
        {
            using (var file = System.IO.File.Create(basedir + "test-complex-struct.proto"))
            {
                ProtoBuf.Serializer.Serialize(file, new Proba
                {
                    thisIsAFlag = false,
                    thisIsAnotherFlag = true,
                    thisIsAString = "Apple",
                    thisIsAByte = 0xA1,
                    thisIsALong = 0xA2
                });
            }

            using (var file = System.IO.File.Create(basedir + "test-two-booleans-struct.proto"))
            {
                ProtoBuf.Serializer.Serialize(file, new TwoBooleansProba
                {
                    thisIsAFlag = false,
                    thisIsAnotherFlag = true
                });
            }

            using (var file = System.IO.File.Create(basedir + "test-byte.proto"))
            {
                byte toSerialize = 0xA1;
                ProtoBuf.Serializer.Serialize(file, toSerialize);
            }

            using (var file = System.IO.File.Create(basedir + "test-array-two-bytes.proto"))
            {
                byte[] toSerialize = { 0xA1, 0xA2 };
                ProtoBuf.Serializer.Serialize(file, toSerialize);
            }

            using (var file = System.IO.File.Create(basedir + "test-list-two-bytes.proto"))
            {
                byte[] toSerialize = { 0xA1, 0xA2 };
                ProtoBuf.Serializer.Serialize(file, toSerialize.ToList());
            }

            using (var file = System.IO.File.Create(basedir + "test-struct-two-bytes.proto"))
            {
                ProtoBuf.Serializer.Serialize(file, new TwoBytesProba
                {
                    thisIsAByte = 0xA1,
                    thisIsAnotherByte = 0xA2
                });
            }

            using (var file = System.IO.File.Create(basedir + "test-array-three-bytes.proto"))
            {
                byte[] toSerialize = { 0xA1, 0xA2, 0xA3 };
                ProtoBuf.Serializer.Serialize(file, toSerialize);
            }

            using (var file = System.IO.File.Create(basedir + "test-list-three-bytes.proto"))
            {
                byte[] toSerialize = { 0xA1, 0xA2, 0xA3 };
                ProtoBuf.Serializer.Serialize(file, toSerialize.ToList());
            }

            using (var file = System.IO.File.Create(basedir + "test-struct-three-bytes.proto"))
            {
                ProtoBuf.Serializer.Serialize(file, new ThreeBytesProba
                {
                    thisIsAByte = 0xA1,
                    thisIsAnotherByte = 0xA2,
                    thisIsTheThirdByte = 0xA3
                });
            }

            using (var file = System.IO.File.Create(basedir + "test-array-500-bytes.proto"))
            {
                byte[] toSerialize = new byte[500];

                for (var i = 0; i < toSerialize.Count(); i++)
                {
                    toSerialize[i] = 0xFA;
                }

                ProtoBuf.Serializer.Serialize(file, toSerialize);
            }

            using (var file = System.IO.File.Create(basedir + "test-array-5M-bytes.proto"))
            {
                byte[] toSerialize = new byte[5000000];

                for (var i = 0; i < toSerialize.Count(); i++)
                {
                    toSerialize[i] = 0xFA;
                }

                ProtoBuf.Serializer.Serialize(file, toSerialize);
            }
        }

        static void Main(string[] args)
        {
            var totalTime = new Stopwatch();
            totalTime.Start();

            //testDataStructures();

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

        private static GTFSConverter.CRGTFS.TransitDB DeserializeTransitGTFS()
        {
            GTFSConverter.CRGTFS.TransitDB tdb = null;
            var partialTime = new Stopwatch();
            partialTime.Start();
            Console.Write("Deserializing transit database...");
            using (var file = System.IO.File.OpenRead(basedir + "gtfs_transit.dat"))
            {
                tdb = ProtoBuf.Serializer.Deserialize<GTFSConverter.CRGTFS.TransitDB>(file);
            }
            Console.WriteLine(" " + (partialTime.ElapsedMilliseconds / 1000.0) + "s");
            return tdb;
        }

        private static void SerializeTransitGTFS(GTFSConverter.CRGTFS.TransitDB tdb)
        {
            var partialTime = new Stopwatch();
            partialTime.Start();
            Console.Write("Serializing transit database...");
            using (var file = System.IO.File.Create(basedir + "gtfs_transit.dat"))
            {
                ProtoBuf.Serializer.Serialize(file, tdb);
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
