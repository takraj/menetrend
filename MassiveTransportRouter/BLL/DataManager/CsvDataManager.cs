using MTR.BusinessLogic.Common.POCO;
using MTR.DataAccess.CsvDataManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTR.BusinessLogic.DataManager
{
    public class CsvDataManager
    {
        public static void importCsv(string BasePath)
        {
            GtfsDatabase.importCsv(BasePath);
        }

        public static List<MTR.WebApp.Common.ViewModels.VMDL_Stop> GetAllStops()
        {
            var result = new List<MTR.WebApp.Common.ViewModels.VMDL_Stop>();

            foreach (Stop s in DataAccess.CsvDataManager.GtfsDatabase.GetAllStops())
            {
                result.Add(new WebApp.Common.ViewModels.VMDL_Stop(s.StopId, s.StopName, s.StopLatitude, s.StopLongitude, s.LocationType, s.ParentStation, s.WheelchairBoarding));
            }

            return result;
        }
    }
}
