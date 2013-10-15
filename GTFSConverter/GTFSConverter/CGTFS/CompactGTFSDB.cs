using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTFSConverter
{
    [ProtoContract]
    public class CompactGTFSDB
    {
        [ProtoMember(1)]
        public List<CompactGTFS_Agency> agencies;

        [ProtoMember(2)]
        public List<CompactGTFS_Calendar> calendars;

        [ProtoMember(3)]
        public List<CompactGTFS_CalendarDate> calendar_dates;

        [ProtoMember(4)]
        public List<CompactGTFS_Route> routes;

        [ProtoMember(5)]
        public List<CompactGTFS_Shape> shapes;

        [ProtoMember(6)]
        public List<CompactGTFS_Stop> stops;

        [ProtoMember(7)]
        public List<CompactGTFS_StopTime> stop_times;

        [ProtoMember(8)]
        public List<CompactGTFS_Trip> trips;
    }
}
