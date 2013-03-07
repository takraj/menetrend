using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using MTR.WebApp.Classes;

namespace MTR.WebApp.Models
{
    /// <summary>
    /// Temporary in-memory database
    /// </summary>
    public class GtfsDatabase
    {
        public static List<Agency> agencies = new List<Agency>();
        public static List<Route> routes = new List<Route>();
        public static List<Trip> trips = new List<Trip>();
        public static List<StopTime> stop_times = new List<StopTime>();
        public static List<Stop> stops = new List<Stop>();
        public static List<CalendarDate> calendar_dates = new List<CalendarDate>();
        public static List<Calendar> calendar = new List<Calendar>();
        public static List<Shape> shapes = new List<Shape>();
    }

    public class GtfsReader
    {
        /// <summary>
        /// Reads in a CSV file
        /// </summary>
        /// <typeparam name="T">Type of instances to return</typeparam>
        /// <param name="fileName">Name of the file</param>
        /// <returns>List of instances with data from csv</returns>
        public static IEnumerable<T> Read<T>(string fileName) where T : class
        {
            var fileReader = System.IO.File.OpenText(fileName);
            var config = new CsvConfiguration();
            config.UseInvariantCulture = true;
            var csvReader = new CsvHelper.CsvReader(fileReader, config);
            return csvReader.GetRecords<T>();
        }
    }

    public enum E_WheelchairSupport
    {
        NO_INFO = 0,
        YES = 1,
        NO = 2
    }

    public enum E_RouteType
    {
        TRAM = 0,
        SUBWAY = 1,
        RAIL = 2,
        BUS = 3,
        FERRY = 4,
        STREET_LEVEL_CABLE_CAR = 5,
        GONDOLA_LIFT = 6,
        FUNICULAR_CLIFF_RAIL = 7
    }

    public enum E_TripDirection
    {
        FORWARD = 0,
        BACKWARD = 1
    }

    public enum E_LocationType
    {
        STOP = 0,
        STATION = 1
    }

    public enum E_CalendarExceptionType
    {
        ADDED = 1,
        REMOVED = 2
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
        [TypeConverter(typeof(AdvancedEnumConverter<E_RouteType>))]
        public E_RouteType RouteType { get; set; }

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
        [TypeConverter(typeof(AdvancedEnumConverter<E_TripDirection>))]
        public E_TripDirection DirectionId { get; set; }

        [CsvField(Name = "block_id")]
        public String BlockId { get; set; }      // refer

        [CsvField(Name = "shape_id")]
        public String ShapeId { get; set; }      // refer

        [CsvField(Name = "wheelchair_accessible")]
        [TypeConverter(typeof(AdvancedEnumConverter<E_WheelchairSupport>))]
        public E_WheelchairSupport WheelchairAccessible { get; set; }
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
        [TypeConverter(typeof(AdvancedEnumConverter<E_LocationType>))]
        public E_LocationType LocationType { get; set; }

        [CsvField(Name = "parent_station")]
        public String ParentStation { get; set; }

        [CsvField(Name = "wheelchair_boarding")]
        [TypeConverter(typeof(AdvancedEnumConverter<E_WheelchairSupport>))]
        public E_WheelchairSupport WheelchairBoarding { get; set; }
    }

    public class CalendarDate
    {
        public const string SourceFilename = "calendar_dates.txt";

        [CsvField(Name = "service_id")]
        public String ServiceId { get; set; }        // refer

        [CsvField(Name = "date")]
        public String Date { get; set; }             // to DateTime?

        [CsvField(Name = "exception_type")]
        [TypeConverter(typeof(AdvancedEnumConverter<E_CalendarExceptionType>))]
        public E_CalendarExceptionType ExceptionType { get; set; }
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