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
    public class EF_StopTime
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Int32 Id { get; set; }

        public virtual EF_Trip TripId { get; set; }
        public virtual EF_Stop StopId { get; set; }

        public TimeSpan ArrivalTime { get; set; }
        public TimeSpan DepartureTime { get; set; }

        public Int32 StopSequence { get; set; }
        public Double ShapeDistanceTraveled { get; set; }

        public EF_StopTime() {}

        public EF_StopTime(GTFS_StopTime stoptime, EF_Trip trip, EF_Stop stop)
        {
            TripId = trip;
            StopId = stop;
            try
            {
                ArrivalTime = TimeSpan.ParseExact(stoptime.ArrivalTime, @"hh\:mm\:ss", System.Globalization.CultureInfo.InvariantCulture);
            }
            catch (System.OverflowException)
            {
                var values = stoptime.ArrivalTime.Split(':');
                values[0] = (Int32.Parse(values[0]) % 24).ToString();
                ArrivalTime = TimeSpan.ParseExact(values[0] + ':' + values[1] + ':' + values[2], @"h\:mm\:ss", System.Globalization.CultureInfo.InvariantCulture);
            }
            try
            {
                DepartureTime = TimeSpan.ParseExact(stoptime.DepartureTime, @"hh\:mm\:ss", System.Globalization.CultureInfo.InvariantCulture);
            }
            catch (System.OverflowException)
            {
                var values = stoptime.DepartureTime.Split(':');
                values[0] = (Int32.Parse(values[0]) % 24).ToString();
                DepartureTime = TimeSpan.ParseExact(values[0] + ':' + values[1] + ':' + values[2], @"h\:mm\:ss", System.Globalization.CultureInfo.InvariantCulture);
            }
            StopSequence = stoptime.StopSequence;
            ShapeDistanceTraveled = stoptime.ShapeDistanceTraveled;
        }

        #region Bulk Insert

        public static void BulkInsertEntities(List<EF_StopTime> entities)
        {
            var insertManager = new BulkInsertManager(MethodBase.GetCurrentMethod().DeclaringType.Name);
            insertManager.AddColumn("ArrivalTime");
            insertManager.AddColumn("DepartureTime");
            insertManager.AddColumn("StopSequence");
            insertManager.AddColumn("ShapeDistanceTraveled");
            insertManager.AddColumn("TripId_Id");
            insertManager.AddColumn("StopId_Id");

            foreach (EF_StopTime e in entities)
            {
                insertManager.AddRow(e.ArrivalTime, e.DepartureTime, e.StopSequence, e.ShapeDistanceTraveled, e.TripId.Id, e.StopId.Id);
            }

            insertManager.DoBulkInsert();
        }

        #endregion
    }
}
