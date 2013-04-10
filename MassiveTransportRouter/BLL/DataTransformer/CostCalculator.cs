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

        public static void CreateTimetableCache()
        {
            DbManager.CreateTimetableAssociativeCache();
        }

        public static void GetNextDeparture()
        {
            TimeSpan? t;
            if ((t = DbManager.GetNextDeparture(new DateTime(2013, 03, 01), DateTime.Now.TimeOfDay, 93, 2774)) != null)
            {
                Console.Write("FOUND: " + ((TimeSpan)t).ToString());
            } else {
                Console.Write("NOT FOUND");
            }
        }

        public static void GetNextDepartureFromCache()
        {
            TimeSpan? t;
            if ((t = DbManager.GetNextDepartureFromCache(new DateTime(2013, 03, 01), DateTime.Now.TimeOfDay, 93, 2774)) != null)
            {
                Console.Write("FOUND: " + ((TimeSpan)t).ToString());
            }
            else
            {
                Console.Write("NOT FOUND");
            }
        }
    }
}
