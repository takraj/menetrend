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
    }
}
