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
    public class EF_Calendar
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Int32 Id { get; set; }

        public String OriginalId { get; set; }
        public Boolean Monday { get; set; }
        public Boolean Tuesday { get; set; }
        public Boolean Wednesday { get; set; }
        public Boolean Thursday { get; set; }
        public Boolean Friday { get; set; }
        public Boolean Saturday { get; set; }
        public Boolean Sunday { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public EF_Calendar() { }

        public EF_Calendar(GTFS_Calendar calendar)
        {
            OriginalId = calendar.ServiceId;
            Monday = calendar.Monday;
            Tuesday = calendar.Tuesday;
            Wednesday = calendar.Wednesday;
            Thursday = calendar.Thursday;
            Friday = calendar.Friday;
            Saturday = calendar.Saturday;
            Sunday = calendar.Sunday;
            StartDate = DateTime.ParseExact(calendar.StartDate, "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);
            EndDate = DateTime.ParseExact(calendar.EndDate, "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);
        }
    }
}
