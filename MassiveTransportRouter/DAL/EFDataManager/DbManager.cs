using MTR.DataAccess.EFDataManager.Entities;
using System;
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
                    result.Add(new BusinessLogic.Common.POCO.Stop(s.OriginalId, s.StopName, s.StopLatitude, s.StopLongitude, s.LocationType, (s.ParentStation != null) ? s.ParentStation.OriginalId : null, s.WheelchairBoarding));
                }
            }

            return result;
        }
    }
}
