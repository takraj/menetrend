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
        ushort GetDistance(Stop stop1, Stop stop2)
        {
            var coordA = new GeoCoordinate(stop1.position.latitude, stop1.position.longitude);
            var coordB = new GeoCoordinate(stop2.position.latitude, stop2.position.longitude);

            return (ushort) coordA.GetDistanceTo(coordB);
        }

        void CalculateTransferDistances(ref TransitDB tdb, ref OriginalMaps originalMaps)
        {
            var countOfStops = tdb.stops.Count();
            var stopIndices = Enumerable.Range(0, countOfStops - 1).ToArray();

            var allStops = tdb.stops;
            var stopDistanceMatrix = new uint[countOfStops * countOfStops];

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
        }
    }
}
