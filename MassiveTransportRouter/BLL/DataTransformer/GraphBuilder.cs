using MTR.BusinessLogic.Common.POCO;
using MTR.DataAccess.EFDataManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTR.BusinessLogic.DataTransformer
{
    public class GraphBuilder
    {
        private static bool isDebug = true;

        public static void BuildGraph()
        {
            var allStops = DbManager.GetAllStops();
            var routesOfStops = new Dictionary<Stop, HashSet<int>>();
            var pathsOfRoutes = new Dictionary<int, HashSet<List<Stop>>>();
            var sequenceComparer = new SequenceComparer<Stop>();

            int counter = 0;

            foreach (Trip trip in DbManager.GetTripsWithDistinctPaths())
            {
                if (!pathsOfRoutes.ContainsKey(trip.RouteId))
                {
                    // Ha nincs még benne ez a kulcs, akkor csinálunk egy listát
                    pathsOfRoutes.Add(trip.RouteId, new HashSet<List<Stop>>(sequenceComparer));
                }

                // Lekérem a Trip-hez tartozó összes megállót + referenciát szerzek rájuk
                var stopsOrder = new List<Stop>();
                DbManager.GetStopsOrderByTrip(trip).ForEach(s => stopsOrder.Add(allStops.First(sr => sr.DbId == s.DbId)));

                // Megállósorrend lista hozzáadása a halmazhoz
                HashSet<List<Stop>> paths;
                pathsOfRoutes.TryGetValue(trip.RouteId, out paths);
                paths.Add(stopsOrder);

                // Stop-okat érintő Route-ok regisztrálása
                stopsOrder.ForEach(s => {
                    if (!routesOfStops.ContainsKey(s))
                    {
                        // Ha ez a Stop új, akkor hozzáadjuk a kulcsot (RouteId) egy üres listával
                        routesOfStops.Add(s, new HashSet<int>());
                    }

                    // Regisztrálom a Route-ot
                    HashSet<int> routes;
                    routesOfStops.TryGetValue(s, out routes);
                    routes.Add(trip.RouteId);
                });
                if (isDebug) Console.Write("#" + trip.DbId);
            }

            Console.WriteLine();
            Console.WriteLine("---------------DB ÉPÍTÉS-------------------");

            // Adattábla építése
            foreach (Stop stop in allStops)
            {
                HashSet<int> routes;
                routesOfStops.TryGetValue(stop, out routes);

                if (routes == null)
                {
                    continue;
                }

                foreach (int routeId in routes)
                {
                    var stopSet = new HashSet<Stop>();

                    HashSet<List<Stop>> paths;
                    pathsOfRoutes.TryGetValue(routeId, out paths);

                    foreach (List<Stop> listOfStops in paths)
                    {
                        // Megkeressük, hogy ki a nextStop
                        for (int i = 0; i < (listOfStops.Count - 1); i++)
                        {
                            if (listOfStops.ElementAt(i) == stop)
                            {
                                // Hozzáadjuk
                                stopSet.Add(listOfStops.ElementAt(i + 1));
                                break;
                            }
                        }
                    }

                    if (isDebug) Console.Write("" + stop.DbId + "#" + routeId + " (" + stopSet.Count + ")");
                    foreach (Stop nextStop in stopSet)
                    {
                        // [stop.DbId, routeId, nextStop.DbId] hozzáadása az adatbázishoz
                        if (isDebug) Console.Write(" | " + nextStop.DbId);
                        counter++;
                    }
                    Console.WriteLine();
                }
            }         // end of allStops foreach

            Console.WriteLine("Élek száma: " + counter);
        }
    }

    /// <summary>
    /// From Stackoverflow:
    /// http://stackoverflow.com/questions/5517932/how-to-create-a-hashsetlistint-with-distinct-elements/5518495#5518495
    /// </summary>
    /// <typeparam name="T"></typeparam>
    class SequenceComparer<T> : IEqualityComparer<IEnumerable<T>>
    {
        public bool Equals(IEnumerable<T> seq1, IEnumerable<T> seq2)
        {
            return seq1.SequenceEqual(seq2);
        }

        public int GetHashCode(IEnumerable<T> seq)
        {
            int hash = 1234567;
            foreach (T elem in seq)
                hash = hash * 37 + elem.GetHashCode();
            return hash;
        }
    }
}
