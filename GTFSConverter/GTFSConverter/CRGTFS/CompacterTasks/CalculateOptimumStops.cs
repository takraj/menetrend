using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTFSConverter.CRGTFS
{
    public partial class RGTFSCompacter
    {
        void CalculateOptimumStops(TransitDB tdb)
        {
            var routeSequenceInfo = new Dictionary<int, Dictionary<int, HashSet<int>>>();

            foreach (var trip in tdb.trips)
            {
                var route = tdb.routes[trip.routeIndex];

                if (!routeSequenceInfo.ContainsKey(trip.routeIndex))
                {
                    routeSequenceInfo[trip.routeIndex] = new Dictionary<int, HashSet<int>>();
                }

                if (!routeSequenceInfo[trip.routeIndex].ContainsKey(trip.stopSequenceHint))
                {
                    routeSequenceInfo[trip.routeIndex][trip.stopSequenceHint] = new HashSet<int>(trip.stopTimes.Select(st => st.StopIndex));

                    // A sequence összes megállóját megnézzük
                    foreach (var stopIndex in routeSequenceInfo[trip.routeIndex][trip.stopSequenceHint])
                    {
                        var stop = tdb.stops[stopIndex];

                        // Ha ebben a megállóban nincsenek gyalogos átszállási lehetőségek...
                        if (stop.nearbyStops == null)
                        {
                            continue; // Nincsenek közeli megállók. Fura, de létezik ilyen.
                        }

                        // Ebből a megállóból ezek a megállók érhetőek el gyalog
                        for (int i = 0; i < (stop.nearbyStops.Length / 2); i++)
                        {
                            int nearbyStopIdx = stop.nearbyStops[i * 2];

                            if (routeSequenceInfo[trip.routeIndex][trip.stopSequenceHint].Contains(nearbyStopIdx))
                            {
                                continue; // A sequence tartalmazza ezt a lehetőséget, tehát nem kell gyalogolni az eléréséhez
                            }

                            // Állítsuk be legközelebbinek ezt a megállót, hacsak nincs közelebbi beállítva
                            string key = trip.stopSequenceHint.ToString() + "-" + nearbyStopIdx.ToString();
                            if (route.optimumStop.ContainsKey(key))
                            {
                                // Már van valami beállítva... vizsgáljuk meg
                                var currentOptimumStop = tdb.stops[route.optimumStop[key]];

                                for (int j = 0; j < (currentOptimumStop.nearbyStops.Length / 2); j++)
                                {
                                    if (currentOptimumStop.nearbyStops[j * 2] == nearbyStopIdx)
                                    {
                                        // Hacsak nincs közelebbi beállítva...
                                        if (stop.nearbyStops[(i * 2) + 1] < currentOptimumStop.nearbyStops[(j * 2) + 1])
                                        {
                                            route.optimumStop[key] = stopIndex;
                                        }
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                // Ha még nem volt hozzá legközelebbi beállítva, akkor nyilván ez a legközelebbi.
                                route.optimumStop[key] = stopIndex;
                            }
                        }
                    }
                }
            }
        }
    }
}
