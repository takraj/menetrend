using GTFSConverter.CRGTFS.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTFSConverter.CRGTFS.Pathfinder
{
    public class ChangeOption
    {
        public DateTime arrivalTime;
        public StopTime stopTime;
    }

    public class TransitGraph
    {
        public double walkingSpeed = 70;
        public double maxTotalWalkingMinutes = 30;
        public double maxWalkingDistancePerChange = 500;
        public double maxWaitingMinutesForNextTrip = 60;
        public double costOfGettingOff = 2;

        private IStorageManager storageManager;

        public TransitGraph(IStorageManager storageManager)
        {
            this.storageManager = storageManager;
        }

        public Stop GetStopByIndex(int stopIndex)
        {
            return storageManager.GetStop(stopIndex);
        }

        public Trip GetTripByIndex(int tripIndex)
        {
            return storageManager.GetTrip(tripIndex);
        }

        public Route GetRouteByIndex(int routeIndex)
        {
            return storageManager.GetRoute(routeIndex);
        }

        public List<ChangeOption> GetChangeOptions(Stop stop, DateTime date, HashSet<Route> unusableRoutes)
        {
            if (stop.knownRoutes == null || stop.knownRoutes.Count == 0)
            {
                return new List<ChangeOption>();
            }

            var result = new List<ChangeOption>();

            foreach (var route in stop.knownRoutes.Select(kridx => GetRouteByIndex(kridx)))
            {
                if (unusableRoutes.Contains(route))
                {
                    continue;
                }

                var option = FindNextStopTimeOnRoute(stop, date, route);
                if (option != null)
                {
                    result.Add(option);
                }
            }
            
            return result;
        }

        public ChangeOption FindNextStopTimeOnRoute(Stop stop, DateTime currentDate, Route route)
        {
            var exceptons = new HashSet<ushort>(route.NoServiceDates);
            DateTime minDate = Utility.ConvertBackToDate(route.MinDate);
            DateTime maxDate = Utility.ConvertBackToDate(route.MaxDate);

            if (currentDate > maxDate)
            {
                // Úgyse fog már találni megfelelő opciót.
                return null;
            }

            /*
             * Élek azzal a feltételezéssel, hogy egy trip max 24 óráig tart.
             * Ez a valóságban még országos vonatoknál is csak néhány óra...
             */

            var iteratorDate = currentDate.AddDays(-1).Date;

            if (iteratorDate < minDate)
            {
                iteratorDate = minDate;
            }

            // Az előző naptól elkezdjük keresni...
            while (iteratorDate <= maxDate)
            {
                // Ha ezen a napon nem közlekedik, akkor skip...
                if (exceptons.Contains(Utility.GetDaysFrom2000(iteratorDate)))
                {
                    iteratorDate = iteratorDate.AddDays(1);
                    continue;
                }

                // Az adott napon közlekedő tripeken végigmegyünk...
                var tripDates = storageManager.GetTripsForDate(route.idx, Utility.GetDaysFrom2000(iteratorDate));
                foreach (var trip in tripDates.Select(tripIndex => storageManager.GetTrip(tripIndex)))
                {
                    /*
                     * Kiszámoljuk, hogy mikor érne véget a trip.
                     * Már itt is fontos, hogy az előző naptól kezdjük, mert lehet, hogy
                     * a trip másnap ér véget és még van benne értelmes StopTime.
                     * Ha esetleg még így is lekéstük, akkor skip...
                     */
                    var endOfTrip = iteratorDate.AddMinutes(trip.endTime);
                    if (endOfTrip <= currentDate)
                    {
                        continue;
                    }

                    // Az utolsót nem akarom megtalálni
                    for (int i = 0; i < (trip.stopTimes.Length - 1); i++)
                    {
                        var arrivalTime = iteratorDate.AddMinutes(trip.stopTimes[i].arrivalTime);

                        /*
                         * (a megfelelő megállóra vonatkozik) && (currentDate < arrivalTime)
                         */
                        if ((trip.stopTimes[i].StopIndex == stop.idx) && (arrivalTime > currentDate))
                        {
                            return new ChangeOption
                            {
                                arrivalTime = arrivalTime,
                                stopTime = trip.stopTimes[i]
                            };
                        }
                    }
                }

                iteratorDate = iteratorDate.AddDays(1);
            }

            // Semmit sem találtunk... :(
            return null;
        }

        /// <summary>
        /// Az átlagos gyaloglási sebességet 83 m/min értékűnek véve számítja ki két
        /// megálló között a gyaloglási időt percekben, légvonalban.
        /// </summary>
        /// <param name="stop1"></param>
        /// <param name="stop2"></param>
        /// <returns>Gyaloglási idő, percekben.</returns>
        public double GetWalkingCostBetween(Stop stop1, Stop stop2)
        {
            return Math.Ceiling(GetDistanceBetween(stop1, stop2) / this.walkingSpeed);
        }

        public int GetDistanceBetween(Stop stop1, Stop stop2)
        {
            for (int i = 0; i < stop1.nearbyStops.Length; i += 2)
            {
                if (stop1.nearbyStops[i] == stop2.idx)
                {
                    return stop1.nearbyStops[i + 1];
                }
            }

            for (int i = 0; i < stop2.nearbyStops.Length; i += 2)
            {
                if (stop2.nearbyStops[i] == stop1.idx)
                {
                    return stop2.nearbyStops[i + 1];
                }
            }

            return storageManager.GetStopDistanceVector(stop1.idx)[stop2.idx];
        }
    }
}
