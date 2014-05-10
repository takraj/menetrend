using FlowerDataModel;
using FlowerGraphModel;
using PathfinderCore;
using PriorityQueues;
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
        static void Main(string[] args)
        {
            Console.WriteLine("Initializing...");

            var dataSource = new CsvDataSource(@"C:\flower_data");
            var repo = new MemoryRepository(dataSource);

            var graph = new FlowerGraph(repo, new HashSet<int>(), new Dictionary<int, TimeSpan>(), new TimeSpan(0, 1, 0), new TimeSpan(0, 30, 0), 5.3, false);
            
            var source = new WalkingNode(graph, 817);
            var destination = new WalkingNode(graph, 1740);

            var state = new AStarPathfinderState(graph, source, destination, new DateTime(2014, 2, 20, 14, 0, 0));

            var pathfinder = new GenericPathfinder<FlowerNode, DateTime, DijkstraPathfinderState, BinaryHeapPriorityQueue<FlowerNode>>();

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
        }
    }
}
