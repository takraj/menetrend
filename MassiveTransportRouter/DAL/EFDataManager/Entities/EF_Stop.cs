using MTR.Common.Gtfs;
using MTR.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MTR.DataAccess.EFDataManager.Entities
{
    public class EF_Stop
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Int32 Id { get; set; }

        public String OriginalId { get; set; }
        public String StopName { get; set; }
        public Double StopLatitude { get; set; }
        public Double StopLongitude { get; set; }

        public virtual EF_Stop ParentStation { get; set; }

        public E_LocationType LocationType { get; set; }
        public E_WheelchairSupport WheelchairBoarding { get; set; }

        public EF_Stop()
        {
        }

        public EF_Stop(GTFS_Stop stop)
        {
            OriginalId = stop.StopId;
            StopName = stop.StopName;
            StopLatitude = stop.StopLatitude;
            StopLongitude = stop.StopLongitude;
            LocationType = stop.LocationType;
            WheelchairBoarding = stop.WheelchairBoarding;
        }

        #region Bulk Insert

        public static void BulkInsertEntities(List<EF_Stop> entities)
        {
            var insertManager = new BulkInsertManager(MethodBase.GetCurrentMethod().DeclaringType.Name);
            insertManager.AddColumn("OriginalId");
            insertManager.AddColumn("StopName");
            insertManager.AddColumn("StopLatitude");
            insertManager.AddColumn("StopLongitude");
            insertManager.AddColumn("LocationType");
            insertManager.AddColumn("WheelchairBoarding");

            foreach (EF_Stop e in entities)
            {
                insertManager.AddRow(e.OriginalId, e.StopName, e.StopLatitude, e.StopLongitude, (int)e.LocationType, (int)e.WheelchairBoarding);
            }

            insertManager.DoBulkInsert();
        }

        #endregion
    }
}
