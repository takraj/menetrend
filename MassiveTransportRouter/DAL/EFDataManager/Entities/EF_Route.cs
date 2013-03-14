using MTR.Common.Gtfs;
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

        public EF_Route(GTFS_Route route)
        {
            OriginalId = route.RouteId;
            RouteShortName = route.RouteShortName;
            RouteDescription = route.RouteDescription;
            RouteType = route.RouteType;
            RouteColor = route.RouteColor;
            RouteTextColor = route.RouteTextColor;
        }
    }
}
