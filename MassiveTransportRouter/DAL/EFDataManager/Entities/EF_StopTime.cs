using MTR.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
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
            ArrivalTime = TimeSpan.ParseExact(stoptime.ArrivalTime, "G", System.Globalization.CultureInfo.InvariantCulture);
            DepartureTime = TimeSpan.ParseExact(stoptime.DepartureTime, "G", System.Globalization.CultureInfo.InvariantCulture);
            StopSequence = stoptime.StopSequence;
            ShapeDistanceTraveled = stoptime.ShapeDistanceTraveled;
        }
    }
}
