using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTFSConverter.CRGTFS
{
    public partial class RGTFSCompacter
    {
        void CalculateStopRouteRelationships(ref TransitDB tdb, ref OriginalMaps originalMaps)
        {
            var ctripDictionary = db.trips.GroupBy(t => t.route_id).ToDictionary(t => t.Key, t => t.ToList());
            var cstoptimeDictionary = db.stop_times.GroupBy(st => st.trip_id).ToDictionary(st => st.Key, st => st.ToList());

            var routeIndexes = new Dictionary<Route, int>();
            for (int i = 0; i < tdb.routes.Count; i++)
            {
                routeIndexes[tdb.routes.ElementAt(i)] = i;
            }

            foreach (var croute in db.routes)
            {
                foreach (var ctrip in ctripDictionary[croute.route_id])
                {
                    foreach (var cstoptime in cstoptimeDictionary[ctrip.trip_id])
                    {
                        if (cstoptime == cstoptimeDictionary[ctrip.trip_id].Last())
                        {
                            continue;
                        }

                        var relatedRRoute = originalMaps.originalRouteMap[croute.route_id];
                        var relatedRStop = originalMaps.originalStopMap[cstoptime.stop_id];

                        if (relatedRStop.knownRoutes.Contains(routeIndexes[relatedRRoute]))
                        {
                            continue;
                        }

                        relatedRStop.knownRoutes.Add(routeIndexes[relatedRRoute]);
                    }
                }
            }
        }
    }
}
