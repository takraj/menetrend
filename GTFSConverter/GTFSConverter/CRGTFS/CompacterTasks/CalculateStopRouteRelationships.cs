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

            foreach (var croute in db.routes)
            {
                if (!ctripDictionary.ContainsKey(croute.route_id))
                {
                    continue;
                }

                // Végigmegy minden Trip összes StopTime-ján, hogy az egymásra hivatkozásokat feltérképezze
                foreach (var ctrip in ctripDictionary[croute.route_id])
                {
                    foreach (var cstoptime in cstoptimeDictionary[ctrip.trip_id])
                    {
                        //if (cstoptime == cstoptimeDictionary[ctrip.trip_id].Last())
                        //{
                        //    continue;
                        //}

                        var relatedRRoute = originalMaps.originalRouteMap[croute.route_id];
                        var relatedRStop = originalMaps.originalStopMap[cstoptime.stop_id];

                        relatedRStop.knownRoutes.Add(relatedRRoute.idx);
                        relatedRRoute.knownStops.Add(relatedRStop.idx);
                    }
                }
            }
        }
    }
}
