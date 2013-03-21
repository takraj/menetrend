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
    public class EF_Trip
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Int32 Id { get; set; }

        public String OriginalId { get; set; }

        public virtual EF_Route RouteId { get; set; }
        public virtual EF_Calendar ServiceId { get; set; }
        public virtual EF_Shape ShapeId { get; set; }

        public String TripHeadsign { get; set; }
        public E_TripDirection DirectionId { get; set; }
        public String BlockId { get; set; }
        public E_WheelchairSupport WheelchairAccessible { get; set; }

        public EF_Trip() {}

        public EF_Trip(GTFS_Trip trip, EF_Route route, EF_Calendar calendar, EF_Shape shape)
        {
            OriginalId = trip.TripId;
            RouteId = route;
            ServiceId = calendar;
            ShapeId = shape;
            TripHeadsign = trip.TripHeadsign;
            DirectionId = trip.DirectionId;
            BlockId = trip.BlockId;
            WheelchairAccessible = trip.WheelchairAccessible;
        }

        #region Bulk Insert

        public static void BulkInsertEntities(List<EF_Trip> entities)
        {
            var insertManager = new BulkInsertManager(MethodBase.GetCurrentMethod().DeclaringType.Name);
            insertManager.AddColumn("OriginalId");
            insertManager.AddColumn("TripHeadsign");
            insertManager.AddColumn("BlockId");
            insertManager.AddColumn("DirectionId");
            insertManager.AddColumn("WheelchairAccessible");
            insertManager.AddColumn("RouteId_Id");
            insertManager.AddColumn("ServiceId_Id");
            insertManager.AddColumn("ShapeId_Id");

            foreach (EF_Trip e in entities)
            {
                insertManager.AddRow(e.OriginalId, e.TripHeadsign, e.BlockId, (int)e.DirectionId, (int)e.WheelchairAccessible, e.RouteId.Id, e.ServiceId.Id, e.ShapeId.Id);
            }

            insertManager.DoBulkInsert();
        }

        #endregion
    }
}
