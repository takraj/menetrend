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
            foreach (var tripdate in route.dates)
            {
                if (tripdate.date < (currentDate - 1))
                {
                    continue;
                }

                var trip = tdb.trips.ElementAt(tripdate.tripIndex);
                var tdate = ConvertBackToDate(tripdate.date);

                if (tdate.AddMinutes(trip.endTime) < currentDateTime)
                {
                    continue;
                }

                for (int i = 1; i < trip.stopTimes.Count; i++)
                {
                    var currentStopTime = trip.stopTimes.ElementAt(i-1);
                    var nextStopTime = trip.stopTimes.ElementAt(i);

                    var currentStopDeparts = tdate.AddMinutes(currentStopTime.arrivalTime + currentStopTime.waitingTime);

                    if (currentStopDeparts >= currentDateTime)
                    {
                        return new Edge
                        {
                            byWalking = false,
                            fromStop = currentStop,
                            toStop = tdb.stops.ElementAt(nextStopTime.stopIndex),
                            fromStopTime = currentStopTime,
                            viaStopTime = nextStopTime,
                            viaRoute = route,
                            viaTrip = trip,
                            cost = (ushort)(tdate.AddMinutes(nextStopTime.arrivalTime) - currentDateTime).TotalMinutes
                        };
                    }
                }
            }

            return null;
        }

        public List<Edge> GetTransferEdges(Stop stop)
        {
            return (from transfer in stop.transfers
                    select new Edge
                    {
                        fromStop = stop,
                        toStop = tdb.stops.ElementAt(transfer.toStopIndex),
                        byWalking = true,
                        cost = (ushort)(GetWalkingCostInMinutes(transfer.distance) + 1)
                    }).ToList();
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
