using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTFSConverter.CRGTFS
{
    public partial class RGTFSCompacter
    {
        void CalculateFirstLastArrivals(TransitDB tdb, OriginalMaps originalMaps)
        {
            var firstStopTimes = new Dictionary<int, ushort>();
            var lastStopTimes = new Dictionary<int, ushort>();

            foreach (var stopTime in db.stop_times)
            {
                int stopIndex = originalMaps.originalStopMap[stopTime.stop_id].idx;
                if (!firstStopTimes.ContainsKey(stopIndex))
                {
                    firstStopTimes[stopIndex] = stopTime.arrival_time;
                }
                else
                {
                    firstStopTimes[stopIndex] = Math.Min(stopTime.arrival_time, firstStopTimes[stopIndex]);
                }

                if (!lastStopTimes.ContainsKey(stopIndex))
                {
                    lastStopTimes[stopIndex] = stopTime.arrival_time;
                }
                else
                {
                    lastStopTimes[stopIndex] = Math.Max(stopTime.arrival_time, lastStopTimes[stopIndex]);
                }
            }

            firstStopTimes.Keys.AsParallel().ForAll(stopIndex => tdb.stops[stopIndex].firstTripArrives = firstStopTimes[stopIndex]);
            lastStopTimes.Keys.AsParallel().ForAll(stopIndex => tdb.stops[stopIndex].lastTripArrives = lastStopTimes[stopIndex]);
        }
    }
}
