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
    }
}
