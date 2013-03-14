using MTR.DataAccess.CsvDataManager;
using MTR.DataAccess.EFDataManager;
using MTR.WebApp.Common.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTR.BusinessLogic.DataManager
{
    public class DbDataManager
    {
        public static void initDatabase(string BasePath)
        {
            GtfsDatabase.initDatabase(BasePath);
        }

        /// <summary>
        /// Returns all stops from the database
        /// </summary>
        /// <returns></returns>
        public static List<VMDL_Stop> GetAllStops()
        {
            var result = new List<MTR.WebApp.Common.ViewModels.VMDL_Stop>();
            DbManager.GetAllStops().ForEach(s => result.Add(new VMDL_Stop(s.StopId, s.StopName, s.StopLatitude, s.StopLongitude, s.LocationType, s.ParentStation, s.WheelchairBoarding)));
            return result;
        }
    }
}
