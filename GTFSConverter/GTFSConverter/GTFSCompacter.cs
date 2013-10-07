using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTFSConverter
{
    class GTFSCompacter
    {
        private GTFSDB db;

        public GTFSCompacter(GTFSDB db)
        {
            this.db = db;
        }

        public ushort GetDaysFrom2000(string date)
        {
            var dprovided = DateTime.ParseExact(date, "yyyyMMdd", CultureInfo.InvariantCulture);
            var d2000 = new DateTime(2000, 1, 1);
            return (ushort) (dprovided - d2000).TotalDays;
        }

        public ushort MinutesFromLastMidnight(string time)
        {
            var time_components = time.Split(':');
            if (time_components.Length >= 2)
            {
                var hours = ushort.Parse(time_components[0]);
                var minutes = ushort.Parse(time_components[1]);

                return (ushort) ((hours * 60) + minutes);
            }
            else
            {
                return 0;
            }
        }

        public SimpleRGB ExtractColor(string htmlColor)
        {
            var color = ColorTranslator.FromHtml("#" + htmlColor);
            return new SimpleRGB()
            {
                red = color.R,
                green = color.G,
                blue = color.B
            };
        }

        public CompactGTFSDB MakeCompactModels()
        {
            var result = new CompactGTFSDB();

            #region Agency
            var agency_ids = new Dictionary<string, byte>();
            {
                byte nextId = 0;
                foreach (var agency in db.agencies)
                {
                    if (!agency_ids.ContainsKey(agency.agency_id))
                    {
                        agency_ids.Add(agency.agency_id, nextId++);
                    }
                }
            }

            result.agencies = new List<CompactGTFS_Agency>();
            foreach (var agency in db.agencies)
            {
                var compact_agency = new CompactGTFS_Agency()
                {
                    agency_id = agency_ids[agency.agency_id],
                    agency_lang = agency.agency_lang,
                    agency_name = agency.agency_name,
                    agency_phone = agency.agency_phone,
                    agency_timezone = agency.agency_timezone,
                    agency_url = agency.agency_url
                };
                result.agencies.Add(compact_agency);
            }
            #endregion

            #region Calendar
            var service_ids = new Dictionary<string, byte>();
            {
                byte nextId = 0;
                foreach (var calendar in db.calendars)
                {
                    if (!service_ids.ContainsKey(calendar.service_id))
                    {
                        service_ids.Add(calendar.service_id, nextId++);
                    }
                }

                result.calendars = new List<CompactGTFS_Calendar>();
                foreach (var calendar in db.calendars)
                {
                    var compact_calendar = new CompactGTFS_Calendar()
                    {
                        service_id = service_ids[calendar.service_id],
                        end_date = GetDaysFrom2000(calendar.end_date),
                        start_date = GetDaysFrom2000(calendar.start_date),
                        monday = calendar.monday,
                        tuesday = calendar.tuesday,
                        wednesday = calendar.wednesday,
                        thursday = calendar.thursday,
                        friday = calendar.friday,
                        saturday = calendar.saturday,
                        sunday = calendar.sunday
                    };
                    result.calendars.Add(compact_calendar);
                }
            }
            #endregion

            #region Calendar Dates
            {
                byte nextId = (byte) service_ids.Values.Count;
                foreach (var calendar_date in db.calendar_dates)
                {
                    if (!service_ids.ContainsKey(calendar_date.service_id))
                    {
                        service_ids.Add(calendar_date.service_id, nextId++);
                    }
                }

                result.calendar_dates = new List<CompactGTFS_CalendarDate>();
                foreach (var calendar_date in db.calendar_dates)
                {
                    var compact_calendar_date = new CompactGTFS_CalendarDate()
                    {
                        service_id = service_ids[calendar_date.service_id],
                        date = GetDaysFrom2000(calendar_date.date),
                        is_removed = (calendar_date.exception_type == 2)
                    };
                    result.calendar_dates.Add(compact_calendar_date);
                }
            }
            #endregion

            #region Routes
            var route_ids = new Dictionary<string, ushort>();
            {
                ushort nextId = 0;
                foreach (var route in db.routes)
                {
                    if (!route_ids.ContainsKey(route.route_id))
                    {
                        route_ids.Add(route.route_id, nextId++);
                    }
                }

                result.routes = new List<CompactGTFS_Route>();
                foreach (var route in db.routes)
                {
                    var compact_route = new CompactGTFS_Route()
                    {
                        agency_id = agency_ids[route.agency_id],
                        route_desc = route.route_desc,
                        route_id = route_ids[route.route_id],
                        route_short_name = route.route_short_name,
                        route_type = (byte)route.route_type,
                        route_text_color = ExtractColor(route.route_text_color),
                        route_color = ExtractColor(route.route_color)
                    };
                    result.routes.Add(compact_route);
                }
            }
            #endregion

            #region Stops
            var stop_ids = new Dictionary<string, ushort>();
            {
                ushort nextId = 0;
                foreach (var stop in db.stops)
                {
                    if (!stop_ids.ContainsKey(stop.stop_id))
                    {
                        stop_ids.Add(stop.stop_id, nextId++);
                    }
                }

                result.stops = new List<CompactGTFS_Stop>();
                foreach (var stop in db.stops)
                {
                    var compact_stop = new CompactGTFS_Stop()
                    {
                        stop_id = stop_ids[stop.stop_id],
                        stop_name = stop.stop_name,
                        stop_lat = stop.stop_lat,
                        stop_lon = stop.stop_lon,
                        is_station = stop.location_type == 1,
                        parent_station = null,
                        wheelchair_boarding = stop.wheelchair_boarding == 1
                    };

                    if (stop.parent_station != null && stop_ids.ContainsKey(stop.parent_station))
                    {
                        compact_stop.parent_station = stop_ids[stop.parent_station];
                    }

                    result.stops.Add(compact_stop);
                }
            }
            #endregion

            #region Shapes
            var shape_ids = new Dictionary<string, ushort>();
            {
                ushort nextId = 0;
                foreach (var shape in db.shapes)
                {
                    if (!shape_ids.ContainsKey(shape.shape_id))
                    {
                        shape_ids.Add(shape.shape_id, nextId++);
                    }
                }

                result.shapes = new List<CompactGTFS_Shape>();
                foreach (var shape in db.shapes)
                {
                    var compact_shape = new CompactGTFS_Shape()
                    {
                        shape_id = shape_ids[shape.shape_id],
                        shape_dist_traveled = (uint)shape.shape_dist_traveled,
                        shape_pt_lat = (float)shape.shape_pt_lat,
                        shape_pt_lon = (float)shape.shape_pt_lon
                    };

                    result.shapes.Add(compact_shape);
                }

                // Kicsit át kellene alakítani, mert ez nagyon ineffektíven van most tárolva. EZ EGY VONAL!
            }
            #endregion

            #region Trips
            var trip_ids = new Dictionary<string, uint>();
            var block_ids = new Dictionary<string, uint>();
            {
                uint nextId = 0;
                uint nextBlockId = 0;
                foreach (var trip in db.trips)
                {
                    if (!trip_ids.ContainsKey(trip.trip_id))
                    {
                        trip_ids.Add(trip.trip_id, nextId++);
                    }

                    if (!block_ids.ContainsKey(trip.block_id))
                    {
                        block_ids.Add(trip.block_id, nextBlockId++);
                    }
                }

                result.trips = new List<CompactGTFS_Trip>();
                foreach (var trip in db.trips)
                {
                    var compact_trip = new CompactGTFS_Trip()
                    {
                        trip_id = trip_ids[trip.trip_id],
                        route_id = route_ids[trip.route_id],
                        service_id = service_ids[trip.service_id],
                        trip_headsign = trip.trip_headsign,
                        shape_id = shape_ids[trip.shape_id],
                        wheelchair_accessible = trip.wheelchair_accessible == 1,
                        is_forward_trip = trip.direction_id == 0,
                        block_id = block_ids[trip.block_id]
                    };

                    result.trips.Add(compact_trip);
                }
            }
            #endregion

            #region Stop Times
            {
                result.stop_times = new List<CompactGTFS_StopTime>();
                foreach (var stop_time in db.stop_times)
                {
                    var compact_stop_time = new CompactGTFS_StopTime()
                    {
                        stop_id = stop_ids[stop_time.stop_id],
                        trip_id = trip_ids[stop_time.trip_id],
                        shape_dist_traveled = (uint) stop_time.shape_dist_traveled,
                        arrival_time = MinutesFromLastMidnight(stop_time.arrival_time),
                        departure_time = MinutesFromLastMidnight(stop_time.departure_time)
                    };

                    result.stop_times.Add(compact_stop_time);
                }
            }
            #endregion

            return result;
        }
    }
}
