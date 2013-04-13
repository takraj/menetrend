using MTR.BusinessLogic.DataManager;
using MTR.BusinessLogic.DataTransformer;
using MTR.BusinessLogic.Pathfinder.Dijkstra;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTR.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Configuring database...");
            DbDataManager.initDatabase(Environment.CurrentDirectory + "budapest_gtfs/");

            //CreateStopGroups();
            //CreateGraph();
            //CreateTimetableCache();

            //GetTimetable();
            //GetNextDeparture();
            //GetGraphMap();
            RunDijkstra();

            Console.WriteLine("-- DONE --");
            Console.ReadKey();
        }

        static void RunDijkstra()
        {
            Console.WriteLine("Press any key to get a random route. Terminate with '.' key.");
            char key;
            while ((key = Console.ReadKey().KeyChar).CompareTo('.') != 0)
            {
                Console.WriteLine();
                Console.WriteLine("Running Dijkstra...");
                var stopwatch = new Stopwatch();

                stopwatch.Start();
                var rnd = new Random();
                var src = rnd.Next(100, 5000);
                var dst = rnd.Next(100, 5000);
                new FastDijkstraPathfinder().GetShortestRoute(src, dst, new DateTime(2013, 03, 01, 19, 0, 0));
                //new FastDijkstraPathfinder().GetShortestRoute(3240, 2841, new DateTime(2013, 03, 01, 19, 0, 0)); // test
                stopwatch.Stop();

                Console.WriteLine("\t Idő: " + (stopwatch.ElapsedMilliseconds / 1000.0) + "s");
            }
        }

        static void GetGraphMap()
        {
            Console.WriteLine("Getting Graph Map...");
            var stopwatch = new Stopwatch();

            stopwatch.Start();
            Console.WriteLine("Count of StopGroups  :\t" + DbDataManager.GetStopGroupsFromDb().Keys.Count);
            Console.WriteLine("CountOfStopsWithEdges:\t" + GraphTransformer.GetGraphMap().Keys.Count);
            stopwatch.Stop();

            Console.WriteLine("\t Idő: " + (stopwatch.ElapsedMilliseconds / 1000.0) + "s");
        }

        static void GetNextDeparture()
        {
            Console.WriteLine("Press a key to get next departure. Terminate with '.' key.");
            Console.WriteLine("Press 'c' for cache or press 'd' for DB retreival.");
            char key;
            while ((key = Console.ReadKey().KeyChar).CompareTo('.') != 0)
            {
                if (key.CompareTo('d') == 0)
                {
                    Console.Write("Getting next departure from DB...");
                    var stopwatch = new Stopwatch();

                    stopwatch.Start();
                    CostCalculator.GetNextDeparture();
                    stopwatch.Stop();

                    Console.WriteLine("\t Idő: " + (stopwatch.ElapsedMilliseconds / 1000.0) + "s");
                }
                else if (key.CompareTo('c') == 0)
                {
                    Console.Write("Getting next departure from cache...");
                    var stopwatch = new Stopwatch();

                    stopwatch.Start();
                    CostCalculator.GetNextDepartureFromCache();
                    stopwatch.Stop();

                    Console.WriteLine("\t Idő: " + (stopwatch.ElapsedMilliseconds / 1000.0) + "s");
                }
            }
        }

        static void GetTimetable()
        {
            Console.WriteLine("Getting timetable...");
            var stopwatch = new Stopwatch();

            stopwatch.Start();
            CostCalculator.GetTimetable();
            stopwatch.Stop();

            Console.WriteLine("\t Idő: " + (stopwatch.ElapsedMilliseconds / 1000.0) + "s");
        }

        static void CreateTimetableCache()
        {
            Console.WriteLine("Creating timetable cache...");
            var stopwatch = new Stopwatch();

            stopwatch.Start();
            CostCalculator.CreateTimetableCache();
            stopwatch.Stop();

            Console.WriteLine("\t Idő: " + (stopwatch.ElapsedMilliseconds / 1000.0) + "s");
        }

        static void CreateGraph()
        {
            Console.WriteLine("Creating Graph...");
            var stopwatch = new Stopwatch();

            stopwatch.Start();
            GraphBuilder.BuildGraph();
            stopwatch.Stop();

            Console.WriteLine("\t Idő: " + (stopwatch.ElapsedMilliseconds / 1000.0) + "s");
        }

        static void FindStopGroups()
        {
            Console.Write("Finding Groups...");
            var stopwatch = new Stopwatch();
            
            stopwatch.Start();
            var allStops = DbDataManager.GetAllStops();
            var stopGroups = DbDataManager.GetStopGroups(100);
            stopwatch.Stop();

            Console.WriteLine("\t" + stopGroups.Count + " / " + allStops.Count + "\t" + (stopwatch.ElapsedMilliseconds / 1000.0) + "s");
        }

        static void CreateStopGroups()
        {
            Console.WriteLine("Creating Groups...");
            var stopwatch = new Stopwatch();

            stopwatch.Start();
            Console.WriteLine("Before: " + DbDataManager.GetStopGroupsFromDb().Keys.Count);
            GraphTransformer.CreateStopGroups(100);
            Console.WriteLine("After: " + DbDataManager.GetStopGroupsFromDb().Keys.Count);
            stopwatch.Stop();

            Console.WriteLine("\t Idő: " + (stopwatch.ElapsedMilliseconds / 1000.0) + "s");
        }
    }
}
