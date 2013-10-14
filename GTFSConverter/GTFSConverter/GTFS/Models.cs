using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTFSConverter
{
    [ProtoContract]
    public class GTFS_Agency
    {
        public const string SourceFilename = "agency.txt";

        [ProtoMember(1)]
        public string agency_id { get; set; }

        [ProtoMember(2)]
        public string agency_name { get; set; }

        [ProtoMember(3)]
        public string agency_url { get; set; }

        [ProtoMember(4)]
        public string agency_timezone { get; set; }

        [ProtoMember(5)]
        public string agency_lang { get; set; }

        [ProtoMember(6)]
        public string agency_phone { get; set; }
    }

    [ProtoContract]
    public class GTFS_Route
    {
        public const string SourceFilename = "routes.txt";

        [ProtoMember(1)]
        public string route_id { get; set; }

        [ProtoMember(2)]
        public string agency_id { get; set; }

        [ProtoMember(3)]
        public string route_short_name { get; set; }

        [ProtoMember(4)]
        public string route_desc { get; set; }

        [ProtoMember(5)]
        public int route_type { get; set; }

        [ProtoMember(6)]
        public string route_color { get; set; }

        [ProtoMember(7)]
        public string route_text_color { get; set; }
    }

    [ProtoContract]
    public class GTFS_Trip
    {
        public const string SourceFilename = "trips.txt";

        [ProtoMember(1)]
        public string trip_id { get; set; }

        [ProtoMember(2)]
        public string route_id { get; set; }

        [ProtoMember(3)]
        public string service_id { get; set; }

        [ProtoMember(4)]
        public string trip_headsign { get; set; }

        [ProtoMember(5)]
        public int direction_id { get; set; }

        [ProtoMember(6)]
        public string block_id { get; set; }

        [ProtoMember(7)]
        public string shape_id { get; set; }

        [ProtoMember(8)]
        public int wheelchair_accessible { get; set; }
    }

    [ProtoContract]
    public class GTFS_StopTime
    {
        public const string SourceFilename = "stop_times.txt";

        [ProtoMember(1)]
        public string trip_id { get; set; }

        [ProtoMember(2)]
        public string arrival_time { get; set; }

        [ProtoMember(3)]
        public string departure_time { get; set; }

        [ProtoMember(4)]
        public string stop_id { get; set; }

        [ProtoMember(5)]
        public int stop_sequence { get; set; }

        [ProtoMember(6)]
        public double shape_dist_traveled { get; set; }
    }

    [ProtoContract]
    public class GTFS_Stop
    {
        public const string SourceFilename = "stops.txt";

        [ProtoMember(1)]
        public string stop_id { get; set; }

        [ProtoMember(2)]
        public string stop_name { get; set; }

        [ProtoMember(3)]
        public double stop_lat { get; set; }

        [ProtoMember(4)]
        public double stop_lon { get; set; }

        [ProtoMember(5)]
        public int? location_type { get; set; }

        [ProtoMember(6)]
        public string parent_station { get; set; }

        [ProtoMember(7)]
        public int wheelchair_boarding { get; set; }
    }

    [ProtoContract]
    public class GTFS_CalendarDate
    {
        public const string SourceFilename = "calendar_dates.txt";

        [ProtoMember(1)]
        public string service_id { get; set; }

        [ProtoMember(2)]
        public string date { get; set; }

        [ProtoMember(3)]
        public int exception_type { get; set; }
    }

    [ProtoContract]
    public class GTFS_Calendar
    {
        public const string SourceFilename = "calendar.txt";

        [ProtoMember(1)]
        public string service_id { get; set; }

        [ProtoMember(2)]
        public bool monday { get; set; }

        [ProtoMember(3)]
        public bool tuesday { get; set; }

        [ProtoMember(4)]
        public bool wednesday { get; set; }

        [ProtoMember(5)]
        public bool thursday { get; set; }

        [ProtoMember(6)]
        public bool friday { get; set; }

        [ProtoMember(7)]
        public bool saturday { get; set; }

        [ProtoMember(8)]
        public bool sunday { get; set; }

        [ProtoMember(9)]
        public string start_date { get; set; }

        [ProtoMember(10)]
        public string end_date { get; set; }
    }

    [ProtoContract]
    public class GTFS_Shape
    {
        public const string SourceFilename = "shapes.txt";

        [ProtoMember(1)]
        public string shape_id { get; set; }

        [ProtoMember(2)]
        public double shape_pt_lat { get; set; }

        [ProtoMember(3)]
        public double shape_pt_lon { get; set; }

        [ProtoMember(4)]
        public int shape_pt_sequence { get; set; }

        [ProtoMember(5)]
        public double shape_dist_traveled { get; set; }
    }
}
