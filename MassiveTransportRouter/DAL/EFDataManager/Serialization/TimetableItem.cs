using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTR.DataAccess.EFDataManager.Serialization
{
    [ProtoContract]
    public class TimetableItem
    {
        [ProtoMember(1)]
        public long ValidFromTick { get; set; }

        [ProtoMember(2)]
        public long ValidToTick { get; set; }

        [ProtoMember(3)]
        public bool ValidOnMonday { get; set; }

        [ProtoMember(4)]
        public bool ValidOnTuesday { get; set; }

        [ProtoMember(5)]
        public bool ValidOnWednesday { get; set; }

        [ProtoMember(6)]
        public bool ValidOnThursday { get; set; }

        [ProtoMember(7)]
        public bool ValidOnFriday { get; set; }

        [ProtoMember(8)]
        public bool ValidOnSaturday { get; set; }

        [ProtoMember(9)]
        public bool ValidOnSunday { get; set; }

        [ProtoMember(10)]
        public long DepartureTick { get; set; }

        // Db related
        public int RouteDbId { get; set; }
        public int StopDbId { get; set; }
        public DateTime ValidFrom { set { this.ValidFromTick = value.Ticks; } }
        public DateTime ValidTo { set { this.ValidToTick = value.Ticks; } }
        public TimeSpan Departure { set { this.DepartureTick = value.Ticks; } }
    }
}
