using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTFSConverter.CRGTFS
{
    public partial class RGTFSCompacter
    {
        void CalculateTransferMap(ref TransitDB tdb)
        {
            tdb.transferMap = new List<TransferMap>();

            for (int i = 0; i < tdb.routes.Length; i++)
            {
                var entry = GetEntryForRoute(tdb.routes[i], tdb);
                tdb.transferMap.Add(entry);
            }
        }

        TransferMap GetEntryForRoute(Route subjectRoute, TransitDB tdb)
        {
            TransferMap entry = new TransferMap
            {
                reachableRouteIdxs = new HashSet<int>(),
                reachableStopIdxs = new HashSet<int>()
            };

            foreach (var stopIdx in subjectRoute.knownStops)
            {
                // Ahol megállhat az elérhető
                entry.reachableStopIdxs.Add(stopIdx);

                var stop = tdb.stops[stopIdx];

                // Az átszállási távolságra lévő megállók is elérhetőek
                if (stop.nearbyStops != null)
                {
                    for (int i = 0; i < (stop.nearbyStops.Length / 2); i++)
                    {
                        if (stop.nearbyStops[i * 2 + 1] <= 500)
                        {
                            entry.reachableStopIdxs.Add(stop.nearbyStops[i * 2]);
                        }
                    }
                }
            }

            // Minden elérhető megálló minden elérhető Route-ja...
            foreach (var stopIdx in entry.reachableStopIdxs)
            {
                var stop = tdb.stops[stopIdx];

                if (stop.knownRoutes != null)
                {
                    foreach (var routeIdx in stop.knownRoutes)
                    {
                        entry.reachableRouteIdxs.Add(routeIdx);
                    }
                }
            }

            return entry;
        }
    }
}
