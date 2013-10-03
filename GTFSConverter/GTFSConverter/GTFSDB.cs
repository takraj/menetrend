using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTFSConverter
{
    [ProtoContract]
    class GTFSDB
    {
        [ProtoMember(1)]
        public List<GTFS_Agency> agencies;

        [ProtoMember(2)]
        public List<GTFS_Calendar> calendars;

        [ProtoMember(3)]
        public List<GTFS_CalendarDate> calendar_dates;

        [ProtoMember(4)]
        public List<GTFS_Route> routes;

        [ProtoMember(5)]
        public List<GTFS_Shape> shapes;

        [ProtoMember(6)]
        public List<GTFS_Stop> stops;

        [ProtoMember(7)]
        public List<GTFS_StopTime> stop_times;

        [ProtoMember(8)]
        public List<GTFS_Trip> trips;
    }
}
