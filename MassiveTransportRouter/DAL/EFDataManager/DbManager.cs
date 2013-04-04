using MTR.DataAccess.EFDataManager.Entities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTR.DataAccess.EFDataManager
{
    public class DbManager
    {
        /// <summary>
        /// Returns all stop records from the database
        /// </summary>
        /// <returns></returns>
        public static List<MTR.BusinessLogic.Common.POCO.Stop> GetAllStops()
        {
            var result = new List<MTR.BusinessLogic.Common.POCO.Stop>();

            using (var db = new EF_GtfsDbContext())
            {
                foreach (EF_Stop s in db.Stops.ToList())
                {
                    result.Add(new BusinessLogic.Common.POCO.Stop(s.Id, s.OriginalId, s.StopName, s.StopLatitude, s.StopLongitude, s.LocationType, (s.ParentStation != null) ? s.ParentStation.OriginalId : null, s.WheelchairBoarding));
                }
            }

            return result;
        }

        /// <summary>
        /// Returns all stop times (stop sequences) that are valid for the specified date
        /// </summary>
        /// <param name="date"></param>
        /// <returns>Ordered list (StopSequence ASC)</returns>
        public static List<BusinessLogic.Common.POCO.StopTime> GetAllStopTimesForDate(DateTime date)
        {
            // Majd refaktorálni kellene, mert ez csak a megállók lehetséges sorrendjeit
            // adja meg (nincs benne az összes trip)
            var result = new List<BusinessLogic.Common.POCO.StopTime>();

            using (var db = new EF_GtfsDbContext())
            {
                var trips = db.Trips.Where(t => ((t.ServiceId.StartDate <= date) && (t.ServiceId.EndDate >= date)));

                switch (date.DayOfWeek)
                {
                    case DayOfWeek.Monday:
                        trips = trips.Where(t => t.ServiceId.Monday);
                        break;
                    case DayOfWeek.Tuesday:
                        trips = trips.Where(t => t.ServiceId.Tuesday);
                        break;
                    case DayOfWeek.Wednesday:
                        trips = trips.Where(t => t.ServiceId.Wednesday);
                        break;
                    case DayOfWeek.Thursday:
                        trips = trips.Where(t => t.ServiceId.Thursday);
                        break;
                    case DayOfWeek.Friday:
                        trips = trips.Where(t => t.ServiceId.Friday);
                        break;
                    case DayOfWeek.Saturday:
                        trips = trips.Where(t => t.ServiceId.Saturday);
                        break;
                    case DayOfWeek.Sunday:
                        trips = trips.Where(t => t.ServiceId.Sunday);
                        break;
                }

                // eliminate redundancy
                var rtrips = new List<EF_Trip>();
                foreach (EF_Trip t in trips.ToArray())
                {
                    if (!rtrips.Exists(rt => (rt.DirectionId.CompareTo(t.DirectionId) == 0) && (rt.TripHeadsign.Equals(t.TripHeadsign)))) {
                        rtrips.Add(t);
                    }
                }
                trips = null;

                // for easy querying
                var rtripids = new List<int>();
                rtrips.ForEach(rt => rtripids.Add(rt.Id));
                rtrips = null;

                // query
                var times = db.StopTimes.Where(st => rtripids.Contains(st.TripId.Id)).OrderBy(st => st.StopSequence);

                // create result
                foreach (EF_StopTime st in times.ToArray())
                {
                    result.Add(new BusinessLogic.Common.POCO.StopTime(st.Id, st.TripId.Id, st.StopId.Id, st.ArrivalTime, st.DepartureTime, st.StopSequence, st.ShapeDistanceTraveled));
                }
            }

            return result;
        }
    }
}
