using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTR.DataAccess.EFDataManager.Entities
{
    public class EF_StopRouteStopEdge
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Int32 Id { get; set; }

        public virtual EF_Stop SourceStop { get; set; }
        public virtual EF_Route ViaRoute { get; set; }
        public virtual EF_Stop DestinationStop { get; set; }
        public virtual EF_Shape AssignedShape { get; set; }

        public Int32 SourceDistance { get; set; }
        public Int32 DestinationDistance { get; set; }
    }
}
