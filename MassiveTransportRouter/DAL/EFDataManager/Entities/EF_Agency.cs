using MTR.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MTR.DataAccess.EFDataManager.Entities
{
    public class EF_Agency
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Int32 Id { get; set; }

        public String OriginalId { get; set; }
        public String AgencyName { get; set; }
        public String AgencyUrl { get; set; }
        public String AgencyTimeZone { get; set; }
        public String AgencyLanguage { get; set; }
        public String AgencyPhoneNumber { get; set; }

        public EF_Agency()
        {
        }

        public EF_Agency(GTFS_Agency agency)
        {
            OriginalId = agency.AgencyId;
            AgencyName = agency.AgencyName;
            AgencyUrl = agency.AgencyUrl;
            AgencyTimeZone = agency.AgencyTimeZone;
            AgencyLanguage = agency.AgencyLanguage;
            AgencyPhoneNumber = agency.AgencyPhoneNumber;
        }

        #region Bulk Insert

        public static void BulkInsertEntities(List<EF_Agency> entities) {
            var insertManager = new BulkInsertManager(MethodBase.GetCurrentMethod().DeclaringType.Name);
            insertManager.AddColumn("OriginalId");
            insertManager.AddColumn("AgencyName");
            insertManager.AddColumn("AgencyUrl");
            insertManager.AddColumn("AgencyTimeZone");
            insertManager.AddColumn("AgencyLanguage");
            insertManager.AddColumn("AgencyPhoneNumber");

            foreach (EF_Agency e in entities)
            {
                insertManager.AddRow(e.OriginalId, e.AgencyName, e.AgencyUrl, e.AgencyTimeZone, e.AgencyLanguage, e.AgencyPhoneNumber);
            }

            insertManager.DoBulkInsert();
        }

        #endregion
    }
}
