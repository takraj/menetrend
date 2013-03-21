using MTR.Common.Gtfs;
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
    public class EF_Route
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Int32 Id { get; set; }

        public String OriginalId { get; set; }
        public String RouteShortName { get; set; }
        public String RouteDescription { get; set; }
        public E_RouteType RouteType { get; set; }
        public String RouteColor { get; set; }
        public String RouteTextColor { get; set; }

        public virtual EF_Agency AgencyId { get; set; }

        public EF_Route() { }

        public EF_Route(GTFS_Route route, EF_Agency agency)
        {
            OriginalId = route.RouteId;
            RouteShortName = route.RouteShortName;
            RouteDescription = route.RouteDescription;
            RouteType = route.RouteType;
            RouteColor = route.RouteColor;
            RouteTextColor = route.RouteTextColor;
            AgencyId = agency;
        }

        #region Bulk Insert

        public static void BulkInsertEntities(List<EF_Route> entities)
        {
            var insertManager = new BulkInsertManager(MethodBase.GetCurrentMethod().DeclaringType.Name);
            insertManager.AddColumn("OriginalId");
            insertManager.AddColumn("RouteShortName");
            insertManager.AddColumn("RouteDescription");
            insertManager.AddColumn("RouteType");
            insertManager.AddColumn("RouteColor");
            insertManager.AddColumn("RouteTextColor");
            insertManager.AddColumn("AgencyId_Id");

            foreach (EF_Route e in entities)
            {
                insertManager.AddRow(e.OriginalId, e.RouteShortName, e.RouteDescription, (int)e.RouteType, e.RouteColor, e.RouteTextColor, e.AgencyId.Id);
            }

            insertManager.DoBulkInsert();
        }

        #endregion
    }
}
