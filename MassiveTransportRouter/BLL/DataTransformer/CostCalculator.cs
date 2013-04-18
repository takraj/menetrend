using MTR.DataAccess.EFDataManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTR.BusinessLogic.DataTransformer
{
    public class CostCalculator
    {
        public static void GetTimetable()
        {
            DbManager.GetTimetable(new DateTime(2013, 03, 01));
        }

        public static void CreateTimetableCache(string appHomeDir = "")
        {
            DbManager.CreateTimetableAssociativeCache(appHomeDir);
        }

        public static void GetNextDeparture()
        {
            TimeSpan? t;
            if ((t = DbManager.GetNextDeparture(new DateTime(2013, 03, 01), DateTime.Now.TimeOfDay, 125, 3247)) != null)
            {
                Console.Write("FOUND: " + ((TimeSpan)t).ToString());
            } else {
                Console.Write("NOT FOUND");
            }
        }

        public static TimeSpan? GetNextDeparture(DateTime when, int routeId, int stopId)
        {
            TimeSpan? t;
            if ((t = DbManager.GetNextDepartureFromCache(when, when.TimeOfDay, routeId, stopId)) != null)
            {
                return t;
            }
            else
            {
                return null;
            }
        }

        public static void GetNextDepartureFromCache()
        {
            TimeSpan? t;
            if ((t = DbManager.GetNextDepartureFromCache(new DateTime(2013, 03, 01), DateTime.Now.TimeOfDay, 125, 3247)) != null)
            {
                Console.Write("FOUND: " + ((TimeSpan)t).ToString());
                if (GetNextDeparture(new DateTime(2013, 03, 01, 19, 10, 0), 125, 3247) == null)
                {
                    Console.Write("#null");
                }
                else
                {
                    Console.Write("#ok");
                }
            }
            else
            {
                Console.Write("NOT FOUND");
            }
        }
    }
}
