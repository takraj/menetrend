using TransitPlannerLibrary.FlowerDataModel;
using TransitPlannerLibrary.FlowerGraphModel;
using TransitPlannerLibrary.PathfinderCore;
using TransitPlannerLibrary.PriorityQueues;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransitPlannerHost
{
    class Program
    {
        private static int GetServiceDay(MemoryRepository repo, DateTime when)
        {
            var meta = repo.MetaInfo;
            return (int)(when.Date - meta.MinDate).TotalDays;
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Initializing...");

            var dataSource = new CsvDataSource(@"C:\flower_data");
            var repo = new MemoryRepository(dataSource);

            var when1 = new DateTime(2014, 4, 7);
            var when2 = new DateTime(2014, 4, 8);
            var when3 = new DateTime(2014, 4, 9);
            var when4 = new DateTime(2014, 4, 10);
            var when5 = new DateTime(2014, 4, 11);
            var when6 = new DateTime(2014, 4, 12);
            var when7 = new DateTime(2014, 4, 13);

            string fs = "when={0}, service_day={1}, trip_id={2}, in_service={3}";
            Trip trip = repo.GetTripById(837);

            {
                int serviceDay = GetServiceDay(repo, when1);
                Console.WriteLine(String.Format(fs, when1, serviceDay, 837, repo.IsServiceAvailableOnDay(trip.ServiceIdx, serviceDay)));
            }
            {
                int serviceDay = GetServiceDay(repo, when2);
                Console.WriteLine(String.Format(fs, when2, serviceDay, 837, repo.IsServiceAvailableOnDay(trip.ServiceIdx, serviceDay)));
            }
            {
                int serviceDay = GetServiceDay(repo, when3);
                Console.WriteLine(String.Format(fs, when3, serviceDay, 837, repo.IsServiceAvailableOnDay(trip.ServiceIdx, serviceDay)));
            }
            {
                int serviceDay = GetServiceDay(repo, when4);
                Console.WriteLine(String.Format(fs, when4, serviceDay, 837, repo.IsServiceAvailableOnDay(trip.ServiceIdx, serviceDay)));
            }
            {
                int serviceDay = GetServiceDay(repo, when5);
                Console.WriteLine(String.Format(fs, when5, serviceDay, 837, repo.IsServiceAvailableOnDay(trip.ServiceIdx, serviceDay)));
            }
            {
                int serviceDay = GetServiceDay(repo, when6);
                Console.WriteLine(String.Format(fs, when6, serviceDay, 837, repo.IsServiceAvailableOnDay(trip.ServiceIdx, serviceDay)));
            }
            {
                int serviceDay = GetServiceDay(repo, when7);
                Console.WriteLine(String.Format(fs, when7, serviceDay, 837, repo.IsServiceAvailableOnDay(trip.ServiceIdx, serviceDay)));
            }

            /*

            var graph = new FlowerGraph(repo, new HashSet<int>(), new Dictionary<int, TimeSpan>(), new TimeSpan(0, 1, 0), new TimeSpan(1, 30, 0), 3.3, false);
            
            var source = new WalkingNode(graph, 1146);
            var destination = new WalkingNode(graph, 1424);

            var state = new AStarPathfinderState(graph, source, destination, new DateTime(2014, 3, 24, 2, 0, 0));

            var pathfinder = new GenericPathfinder<FlowerNode, DateTime, DijkstraPathfinderState, BinaryHeapPriorityQueue<FlowerNode>>();

            //Console.WriteLine("READY");
            //Console.ReadKey();

            Console.WriteLine("Starting pathfinder...");
            var sw = new Stopwatch();
            sw.Start();

            var path = pathfinder.GetPath(state);

            sw.Stop();
            Console.WriteLine("Elapsed time: {0} s", sw.Elapsed.TotalSeconds);

            Console.WriteLine(state.ToString());

            foreach (var item in path)
            {
                var node = item.Key;
                var time = item.Value;
                var stop = graph.Repository.GetStopById(node.StopId);

                if (node is WalkingNode)
                {
                    Console.WriteLine("WALK: {0} @ {1}", stop.Name, time.ToString());
                }
                else if (node is TravellingNode)
                {
                    var tn = node as TravellingNode;
                    var trip = graph.Repository.GetTripById(tn.TripId);
                    var route = graph.Repository.GetRouteById(trip.RouteIdx);

                    Console.WriteLine("{2}: {0} @ {1}", stop.Name, time.ToString(), route.ShortName);
                }
            }
             * */
        }
    }
}
