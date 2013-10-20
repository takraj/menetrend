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
        int GetDistance(Stop stop1, Stop stop2)
        {
            var coordA = new GeoCoordinate(stop1.position.latitude, stop1.position.longitude);
            var coordB = new GeoCoordinate(stop2.position.latitude, stop2.position.longitude);

            return (int) coordA.GetDistanceTo(coordB);
        }

        void CalculateTransferDistances(ref TransitDB tdb, ref OriginalMaps originalMaps)
        {
            var countOfStops = tdb.stops.Count();
            var stopIndices = Enumerable.Range(0, countOfStops - 1).ToArray();

            var allStops = tdb.stops;
            var stopDistanceMatrix = new int[countOfStops * countOfStops];

            stopIndices.AsParallel().ForAll(i => {
                stopDistanceMatrix[i * i] = 0;

                for (int j = (i + 1); j < countOfStops; j++)
                {
                    var distance = GetDistance(allStops[i], allStops[j]);
                    stopDistanceMatrix[(i * countOfStops) + j] = distance;
                    stopDistanceMatrix[(j * countOfStops) + i] = distance;
                }
            });

            tdb.stopDistanceMatrix = stopDistanceMatrix;

            SetupNearbyStops(ref tdb);
        }

        struct StopDistanceElement
        {
            public Stop stop;
            public int dst;
        }

        void SetupNearbyStops(ref TransitDB tdb)
        {
            var countOfStops = tdb.stops.Count();

            foreach (var stop in tdb.stops)
            {
                var stopDstVector = new List<StopDistanceElement>();

                for (int i = 0; i < countOfStops; i++ )
                {
                    if (i == stop.idx)
                    {
                        continue;
                    }

                    stopDstVector.Add(
                        new StopDistanceElement {
                            stop = stop,
                            dst = tdb.stopDistanceMatrix[(stop.idx * countOfStops) + i]
                        });
                }

                var orderedStopDstVector = stopDstVector.OrderBy(sdv => sdv.dst);
                var nearbyStops = new List<int>();

                for (int i = 0; (i < 50) && (i < stopDstVector.Count); i++)
                {
                    nearbyStops.Add(stopDstVector[i].stop.idx);
                    nearbyStops.Add(stopDstVector[i].dst);
                }

                stop.nearbyStops = nearbyStops.ToArray();
            }
        }
    }
}
