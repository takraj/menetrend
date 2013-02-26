using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CsvHelper.Configuration;

namespace Menetrend_WebApp.Models
{
    public enum RouteType
    {
        TRAM = 0,
        SUBWAY = 1,
        RAIL = 2,
        BUS = 3,
        FERRY = 4,
        CABLECAR = 5,
        GONDOLA = 6,
        FUNICULAR = 7
    }

    public class Agency
    {
        public const string SourceFilename = "agency.txt";

        [CsvField(Name = "agency_id")]
        public String AgencyId { get; set; }

        [CsvField(Name = "agency_name")]
        public String AgencyName { get; set; }

        [CsvField(Name = "agency_url")]
        public String AgencyUrl { get; set; }

        [CsvField(Name = "agency_timezone")]
        public String AgencyTimeZone { get; set; }

        [CsvField(Name = "agency_lang")]
        public String AgencyLanguage { get; set; }

        [CsvField(Name = "agency_phone")]
        public String AgencyPhoneNumber { get; set; }
    }

    public class Route
    {
        public const string SourceFilename = "routes.txt";

        [CsvField(Name = "route_id")]
        public String RouteId { get; set; }

        [CsvField(Name = "agency_id")]
        public String AgencyId { get; set; }    // refer to that concrete object !!

        [CsvField(Name = "route_short_name")]
        public String RouteShortName { get; set; }

        [CsvField(Name = "route_desc")]
        public String RouteDescription { get; set; }

        [CsvField(Name = "route_type")]
        public Int32 RouteType { get; set; }    // convert to enum !!

        [CsvField(Name = "route_color")]
        public String RouteColor { get; set; }

        [CsvField(Name = "route_text_color")]
        public String RouteTextColor { get; set; }
    }

    public class Trip
    {
        public const string SourceFilename = "trips.txt";

        [CsvField(Name = "trip_id")]
        public String TripId { get; set; }

        [CsvField(Name = "route_id")]
        public String RouteId { get; set; }     // refer

        [CsvField(Name = "service_id")]
        public String ServiceId { get; set; }   // refer

        [CsvField(Name = "trip_headsign")]
        public String TripHeadsign { get; set; }

        [CsvField(Name = "direction_id")]
        public Int32 DirectionId { get; set; }    // to Enum

        [CsvField(Name = "block_id")]
        public String BlockId { get; set; }      // refer

        [CsvField(Name = "shape_id")]
        public String ShapeId { get; set; }      // refer

        [CsvField(Name = "wheelchair_accessible")]
        public Int32 WheelchairAccessible { get; set; }    // to Enum
    }

    public class StopTime
    {
        public const string SourceFilename = "stop_times.txt";

        [CsvField(Name = "trip_id")]
        public String TripId { get; set; }

        [CsvField(Name = "arrival_time")]
        public String ArrivalTime { get; set; }      // to DateTime?

        [CsvField(Name = "departure_time")]
        public String DepartureTime { get; set; }    // to DateTime?

        [CsvField(Name = "stop_id")]
        public String StopId { get; set; }           // refer

        [CsvField(Name = "stop_sequence")]
        public Int32 StopSequence { get; set; }

        [CsvField(Name = "shape_dist_traveled")]
        public Double ShapeDistanceTraveled { get; set; }
    }

    public class Stop
    {
        public const string SourceFilename = "stops.txt";

        [CsvField(Name = "stop_id")]
        public String StopId { get; set; }

        [CsvField(Name = "stop_name")]
        public String StopName { get; set; }

        [CsvField(Name = "stop_lat")]
        public Double StopLatitude { get; set; }

        [CsvField(Name = "stop_lon")]
        public Double StopLongitude { get; set; }

        [CsvField(Name = "location_type")]
        public Int32 LocationType { get; set; }          // to Enum

        [CsvField(Name = "parent_station")]
        public String ParentStation { get; set; }

        [CsvField(Name = "wheelchair_boarding")]
        public Int32 WheelchairBoarding { get; set; }    // to Enum
    }

    public class CalendarDate
    {
        public const string SourceFilename = "calendar_dates.txt";

        [CsvField(Name = "service_id")]
        public String ServiceId { get; set; }        // refer

        [CsvField(Name = "date")]
        public String Date { get; set; }             // to DateTime?

        [CsvField(Name = "exception_type")]
        public Int32 ExceptionType { get; set; }     // to Enum
    }

    public class Calendar
    {
        public const string SourceFilename = "calendar.txt";

        [CsvField(Name = "service_id")]
        public String ServiceId { get; set; }

        [CsvField(Name = "monday")]
        public Boolean Monday { get; set; }

        [CsvField(Name = "tuesday")]
        public Boolean Tuesday { get; set; }

        [CsvField(Name = "wednesday")]
        public Boolean Wednesday { get; set; }

        [CsvField(Name = "thursday")]
        public Boolean Thursday { get; set; }

        [CsvField(Name = "friday")]
        public Boolean Friday { get; set; }

        [CsvField(Name = "saturday")]
        public Boolean Saturday { get; set; }

        [CsvField(Name = "sunday")]
        public Boolean Sunday { get; set; }

        [CsvField(Name = "start_date")]
        public String StartDate { get; set; }        // to DateTime?

        [CsvField(Name = "end_date")]
        public String EndDate { get; set; }          // to DateTime?
    }

    public class Shape
    {
        public const string SourceFilename = "shapes.txt";

        [CsvField(Name = "shape_id")]
        public String ShapeId { get; set; }

        [CsvField(Name = "shape_pt_lat")]
        public Double ShapePointLatitude { get; set; }

        [CsvField(Name = "shape_pt_lon")]
        public Double ShapePointLongitude { get; set; }

        [CsvField(Name = "shape_pt_sequence")]
        public Int32 ShapePointSequence { get; set; }

        [CsvField(Name = "shape_dist_traveled")]
        public Double ShapeDistanceTravelled { get; set; }
    }
}