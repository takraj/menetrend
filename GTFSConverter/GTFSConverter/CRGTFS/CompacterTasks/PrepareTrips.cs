using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTFSConverter.CRGTFS
{
    public partial class RGTFSCompacter
    {
        void PrepareTrips(ref TransitDB tdb, ref OriginalMaps originalMaps)
        {
            var trips = new List<Trip>();
            var headsigns = new List<string>();

            var cstoptimes = db.stop_times.GroupBy(st => st.trip_id).ToDictionary(st => st.Key, st => st.ToList());
            var cshapeseg = db.shapes.GroupBy(sh => sh.shape_id).ToDictionary(sh => sh.Key, sh => sh.ToList());

            foreach (var ctrip in db.trips)
            {
                var rstoptimes = new List<StopTime>();
                var relatedShape = cshapeseg[ctrip.shape_id].OrderBy(s => s.shape_dist_traveled).ToArray();
                var relatedCStoptimes = cstoptimes[ctrip.trip_id].OrderBy(st => st.shape_dist_traveled).ToArray();

                for (int i = 0; i < relatedCStoptimes.Count(); i++)
                {
                    var cstoptime = relatedCStoptimes[i];

                    uint fromDistance = (i > 0) ? relatedCStoptimes[i - 1].shape_dist_traveled : 0;
                    
                    int stopIndex = originalMaps.originalStopMap[cstoptime.stop_id].idx;
                    int shapeIndex = originalMaps.originalShapeIndexMap[relatedShape.First().shape_id];

                    var rstoptime = new StopTime
                    {
                        arrivalTime = cstoptime.arrival_time,
                        waitingTime = (byte)(cstoptime.departure_time - cstoptime.arrival_time),
                        refIndices = new int[]
                        {
                            stopIndex,
                            shapeIndex,
                            (int)fromDistance,
                            (int)cstoptime.shape_dist_traveled
                        },
                        tripIndex = trips.Count
                    };

                    rstoptimes.Add(rstoptime);
                }

                if (!originalMaps.headsignMap.ContainsKey(ctrip.trip_headsign))
                {
                    headsigns.Add(ctrip.trip_headsign);
                    originalMaps.headsignMap[ctrip.trip_headsign] = headsigns.Count;
                }
                var headsignIdx = originalMaps.headsignMap[ctrip.trip_headsign];

                var rtrip = new Trip
                {
                    headsignIdx = headsignIdx,
                    wheelchairSupport = ctrip.wheelchair_accessible,
                    stopTimes = rstoptimes.ToArray(),
                    endTime = rstoptimes.Last().arrivalTime,
                    idx = trips.Count,
                    routeIndex = originalMaps.originalRouteMap[ctrip.route_id].idx
                };

                trips.Add(rtrip);
                originalMaps.originalTripMap[ctrip.trip_id] = rtrip;
            }

            tdb.trips = trips.ToArray();
            tdb.headsigns = headsigns.ToArray();
        }
    }
}
