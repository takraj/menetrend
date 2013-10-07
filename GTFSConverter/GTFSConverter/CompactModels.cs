using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTFSConverter
{
    [ProtoContract]
    public class CompactGTFS_Agency
    {
        public const string SourceFilename = "agency.txt";

        [ProtoMember(1)]
        public byte agency_id { get; set; }             // STRING -> BYTE

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
    public class CompactGTFS_Route
    {
        [ProtoMember(1)]
        public ushort route_id { get; set; }            // STRING -> USHORT

        [ProtoMember(2)]
        public byte agency_id { get; set; }             // STRING -> BYTE

        [ProtoMember(3)]
        public string route_short_name { get; set; }

        [ProtoMember(4)]
        public string route_desc { get; set; }

        [ProtoMember(5)]
        public byte route_type { get; set; }            // INT -> BYTE

        [ProtoMember(6)]
        public SimpleRGB route_color { get; set; }      // STRING -> SimpleRGB (new type)

        [ProtoMember(7)]
        public SimpleRGB route_text_color { get; set; } // STRING -> SimpleRGB (new type)
    }

    [ProtoContract]
    public class CompactGTFS_Trip
    {
        [ProtoMember(1)]
        public uint trip_id { get; set; }             // STRING -> UINT

        [ProtoMember(2)]
        public ushort route_id { get; set; }            // STRING -> USHORT

        [ProtoMember(3)]
        public ushort service_id { get; set; }          // STRING -> USHORT

        [ProtoMember(4)]
        public string trip_headsign { get; set; }

        [ProtoMember(5)]
        public bool is_forward_trip { get; set; }       // INT -> BOOL

        [ProtoMember(6)]
        public uint block_id { get; set; }            // STRING -> UINT

        [ProtoMember(7)]
        public ushort shape_id { get; set; }            // STRING -> USHORT

        [ProtoMember(8)]
        public bool wheelchair_accessible { get; set; } // INT -> BOOL
    }

    [ProtoContract]
    public class CompactGTFS_StopTime
    {
        [ProtoMember(1)]
        public uint trip_id { get; set; }             // STRING -> UINT

        [ProtoMember(2)]
        public ushort arrival_time { get; set; }        // STRING -> USHORT

        [ProtoMember(3)]
        public ushort departure_time { get; set; }      // STRING -> USHORT

        [ProtoMember(4)]
        public ushort stop_id { get; set; }             // STRING -> USHORT

        [ProtoMember(5)]
        public uint shape_dist_traveled { get; set; } // DOUBLE -> UINT
    }

    [ProtoContract]
    public class CompactGTFS_Stop
    {
        [ProtoMember(1)]
        public ushort stop_id { get; set; }             // STRING -> USHORT

        [ProtoMember(2)]
        public string stop_name { get; set; }

        [ProtoMember(3)]
        public double stop_lat { get; set; }

        [ProtoMember(4)]
        public double stop_lon { get; set; }

        [ProtoMember(5)]
        public bool is_station { get; set; }            // INT -> BOOL

        [ProtoMember(6)]
        public ushort? parent_station { get; set; }      // STRING -> USHORT

        [ProtoMember(7)]
        public bool wheelchair_boarding { get; set; }   // INT -> BOOL
    }

    [ProtoContract]
    public class CompactGTFS_CalendarDate
    {
        [ProtoMember(1)]
        public byte service_id { get; set; }            // STRING -> BYTE

        [ProtoMember(2)]
        public ushort date { get; set; }                // STRING -> DAYS (ushort)

        [ProtoMember(3)]
        public bool is_removed { get; set; }            // INT -> BOOL
    }

    [ProtoContract]
    public class CompactGTFS_Calendar
    {
        [ProtoMember(1)]
        public byte service_id { get; set; }            // STRING -> BYTE

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
        public ushort start_date { get; set; }           // STRING -> DAYS (USHORT)

        [ProtoMember(10)]
        public ushort end_date { get; set; }             // STRING -> DAYS (USHORT)
    }

    [ProtoContract]
    public class CompactGTFS_Shape
    {
        [ProtoMember(1)]
        public ushort shape_id { get; set; }            // STRING -> USHORT

        [ProtoMember(2)]
        public double shape_pt_lat { get; set; }

        [ProtoMember(3)]
        public double shape_pt_lon { get; set; }

        [ProtoMember(4)]
        public uint shape_dist_traveled { get; set; } // DOUBLE -> UINT
    }

    [ProtoContract]
    public class SimpleRGB
    {
        [ProtoMember(1)]
        public byte red { get; set; }

        [ProtoMember(2)]
        public byte green { get; set; }

        [ProtoMember(3)]
        public byte blue { get; set; }
    }
}
