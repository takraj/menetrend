using MTR.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTR.BusinessLogic.Common.POCO
{
    public class StopGroup
    {
        private List<Stop> stops = new List<Stop>();
        private HashSet<Stop> nextStops = new HashSet<Stop>();
        private bool hasDifferentNames = false;

        public double avgLatitude = 0.0;
        public double avgLongitude = 0.0;
        public string name = "";

        public bool HasDifferentNames
        {
            get
            {
                return hasDifferentNames;
            }
        }

        /// <summary>
        /// Megadja, hogy a csoporton belül lévő megállók és a megadott koordináta pár
        /// között mekkora a legnagyobb távolság.
        /// </summary>
        /// <param name="lat">Szélesség</param>
        /// <param name="lon">Hosszúság</param>
        /// <returns>Távolság, méterben</returns>
        public double GetMaxDistanceTo(double lat, double lon)
        {
            var lck = new Object();
            double result = 0.0;

            stops.AsParallel().ForAll(stop =>
            {
                var dst = Utility.measureDistance(lat, lon, stop.StopLatitude, stop.StopLongitude);

                lock (lck)
                {
                    if (result < dst)
                    {
                        result = dst;
                    }
                }
            });

            return result;
        }

        /// <summary>
        /// Simán hozzáad egy megállót a csoporthoz
        /// </summary>
        /// <param name="stop"></param>
        public void AddStop(Stop stop)
        {
            stops.Add(stop);
            avgLatitude = stops.Average(s => s.StopLatitude);
            avgLongitude = stops.Average(s => s.StopLongitude);

            if (this.name.Equals(""))
            {
                this.name = stop.StopName;
            }

            if (!stop.HasSimilarNameTo(this.name))
            {
                hasDifferentNames = true;
            }
        }

        /// <summary>
        /// Hozzáad egy megállót a csoporthoz, majd megkeresi a paraméterek alapján,
        /// hogy milyen következő megállók lehetségesek innen.
        /// </summary>
        /// <param name="stop"></param>
        /// <param name="stopsTripsDict">Adott stop-ban (int id) milyen trip-ek értelmezettek (list int id). Szótár.</param>
        /// <param name="tripsTimesDict">Adott trip (int id) megállói (list StopTime id). Szótár.</param>
        /// <param name="allStops">Az összes lehetséges megálló. Ezek már konkrét Stop példányok.</param>
        public void AddStop(Stop stop, Dictionary<int, List<int>> stopsTripsDict, Dictionary<int, List<StopTime>> tripsTimesDict, List<Stop> allStops)
        {
            this.AddStop(stop);

            // Az aktuális megálló alapján lekéri a tripeket
            List<int> listOfTrips;
            if (stopsTripsDict.TryGetValue(stop.DbId, out listOfTrips))
            {
                // Minden trip-ben megkeresi a következő megállót, ha van
                foreach (int tripId in listOfTrips)
                {
                    List<StopTime> listOfStopTimes;
                    if (tripsTimesDict.TryGetValue(tripId, out listOfStopTimes))
                    {
                        // Ezen még csiszolni kell, mert szűk keresztmetszetet jelenthet az Exception kezelés
                        try
                        {
                            // Következő megálló keresése a rendezett megálló listában
                            var mySequence = listOfStopTimes.First(lost => lost.stopId == stop.DbId).StopSequence;
                            var nextStopId = listOfStopTimes.First(lost => lost.StopSequence > mySequence).stopId;
                            AddNextStop(allStops.First(s => s.DbId == nextStopId));
                        }
                        catch
                        {
                            // dummy
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Pointert hoz létre egy lehetséges továbblépési pontra.
        /// </summary>
        /// <param name="stop"></param>
        public void AddNextStop(Stop stop)
        {
            nextStops.Add(stop);
        }

        /// <summary>
        /// Lekéri a csoport összes megállóját
        /// </summary>
        /// <returns></returns>
        public List<Stop> GetStops()
        {
            return new List<Stop>(stops);
        }

        /// <summary>
        /// Lekéri a csoport összes lehetséges továbblépési pontját
        /// </summary>
        /// <returns></returns>
        public List<Stop> GetNextStops()
        {
            return new List<Stop>(nextStops);
        }
    }
}
