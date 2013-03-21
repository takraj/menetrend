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
    public class EF_CalendarDate
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Int32 Id { get; set; }

        public DateTime ExceptionDate { get; set; }
        public E_CalendarExceptionType ExceptionType { get; set; }

        public virtual EF_Calendar CalendarId { get; set; }

        public EF_CalendarDate() {}

        public EF_CalendarDate(GTFS_CalendarDate cd, EF_Calendar forWhat)
        {
            CalendarId = forWhat;
            ExceptionDate = DateTime.ParseExact(cd.Date, "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);
            ExceptionType = cd.ExceptionType;
        }

        #region Bulk Insert

        public static void BulkInsertEntities(List<EF_CalendarDate> entities)
        {
            var insertManager = new BulkInsertManager(MethodBase.GetCurrentMethod().DeclaringType.Name);
            insertManager.AddColumn("ExceptionDate");
            insertManager.AddColumn("ExceptionType");
            insertManager.AddColumn("CalendarId_Id");

            foreach (EF_CalendarDate e in entities)
            {
                insertManager.AddRow(e.ExceptionDate, (int)e.ExceptionType, e.CalendarId.Id);
            }

            insertManager.DoBulkInsert();
        }

        #endregion
    }
}
