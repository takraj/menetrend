using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTFSConverter.CRGTFS.Pathfinder
{
    public class TransitGraphWrapper
    {
        private TransitDB tdb;
        private const float walkingSpeedInMetresPerSec = 1.5f;

        public TransitGraphWrapper(TransitDB tdb)
        {
            this.tdb = tdb;
        }

        public List<Edge> GetPossibleEdges(Stop stop, DateTime currentDateTime)
        {
            var result = new List<Edge>();

            result.AddRange(GetTransferEdges(stop));
            result.AddRange(GetTravelEdges(stop, currentDateTime));

            return result;
        }

        public List<Edge> GetTravelEdges(Stop stop, DateTime currentDateTime)
        {
            var currentDate = GetDaysFrom2000(currentDateTime);
            var result = new List<Edge>();

            var relevantRoutes = from rid in stop.knownRoutes select tdb.routes.ElementAt(rid);

            foreach (var route in relevantRoutes)
            {
                var edgeFound = FindTravelEdgeOnRoute(stop, currentDateTime, currentDate, route);

                if (edgeFound != null)
                {
                    result.Add((Edge) edgeFound);
                }
            }

            return result;
        }

        public Edge? FindTravelEdgeOnRoute(Stop currentStop, DateTime currentDateTime, ushort currentDate, Route route)
        {
            // TODO

            return null;
        }

        public List<Edge> GetTransferEdges(Stop stop)
        {
            var result = new List<Edge>();

            for (int i = 0; i < tdb.stops.Length; i++)
            {
                if (i == stop.idx)
                {
                    continue;
                }

                var distance = tdb.stopDistanceMatrix[(stop.idx * tdb.stops.Length) + i];

                result.Add(new Edge {
                    fromStop = stop,
                    toStop = tdb.stops.ElementAt(i),
                    byWalking = true,
                    cost = (ushort)(GetWalkingCostInMinutes(distance) + 1)
                });
            }

            return result;
        }

        public ushort GetWalkingCostInMinutes(float distanceInMetres)
        {
            return (ushort) (distanceInMetres / walkingSpeedInMetresPerSec / 60);
        }

        public ushort GetDaysFrom2000(DateTime date)
        {
            var d2000 = new DateTime(2000, 1, 1);
            return (ushort)(date - d2000).TotalDays;
        }

        public DateTime ConvertBackToDate(ushort daysFrom2000)
        {
            var date = new DateTime(2000, 1, 1);
            return date.AddDays(daysFrom2000);
        }
    }

    public struct Edge
    {
        public Stop fromStop;
        public Stop toStop;
        public StopTime fromStopTime;
        public StopTime viaStopTime;
        public Route viaRoute;
        public Trip viaTrip;
        public bool byWalking;
        public ushort cost;
    }
}
