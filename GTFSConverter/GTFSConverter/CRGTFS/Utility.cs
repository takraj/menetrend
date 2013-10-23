using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTFSConverter.CRGTFS
{
    public abstract class Utility
    {
        public static DateTime ConvertBackToDate(ushort daysFrom2000)
        {
            var date = new DateTime(2000, 1, 1);
            return date.AddDays(daysFrom2000);
        }

        public static ushort GetDaysFrom2000(DateTime date)
        {
            var d2000 = new DateTime(2000, 1, 1);
            return (ushort)(date - d2000).TotalDays;
        }
    }
}
