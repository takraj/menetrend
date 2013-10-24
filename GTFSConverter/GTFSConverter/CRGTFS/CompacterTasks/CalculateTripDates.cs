﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTFSConverter.CRGTFS
{
    public partial class RGTFSCompacter
    {
        void CalculateTripDates(ref TransitDB tdb, ref OriginalMaps originalMaps)
        {
            var serviceDictionary = db.trips.GroupBy(t => t.service_id).ToDictionary(t => t.Key, t => t.ToList());

            var tripIndexes = new Dictionary<Trip, int>();
            for (int i = 0; i < tdb.trips.Count(); i++)
            {
                tripIndexes[tdb.trips.ElementAt(i)] = i;
            }

            tdb.routeDatesMap = new Dictionary<Route, List<TripDate>>();
            foreach (var r in tdb.routes)
            {
                tdb.routeDatesMap[r] = new List<TripDate>();
            }

            foreach (var calendar in db.calendars)
            {
                var startDate = calendar.start_date;
                var endDate = calendar.end_date;

                while (startDate < endDate)
                {
                    if (db.calendar_dates.Exists(cd => ((cd.date == startDate) && cd.is_removed && (cd.service_id == calendar.service_id))))
                    {
                        startDate++;
                        continue;
                    }

                    var actualDate = ConvertBackToDate(startDate);

                    bool monday = actualDate.DayOfWeek == DayOfWeek.Monday && calendar.monday;
                    bool tuesday = actualDate.DayOfWeek == DayOfWeek.Tuesday && calendar.tuesday;
                    bool wednesday = actualDate.DayOfWeek == DayOfWeek.Wednesday && calendar.wednesday;
                    bool thursday = actualDate.DayOfWeek == DayOfWeek.Thursday && calendar.thursday;
                    bool friday = actualDate.DayOfWeek == DayOfWeek.Friday && calendar.friday;
                    bool saturday = actualDate.DayOfWeek == DayOfWeek.Saturday && calendar.saturday;
                    bool sunday = actualDate.DayOfWeek == DayOfWeek.Sunday && calendar.sunday;

                    if (monday || tuesday || wednesday || thursday || friday || saturday || sunday)
                    {
                        foreach (var ctrip in serviceDictionary[calendar.service_id])
                        {
                            var rroute = originalMaps.originalRouteMap[ctrip.route_id];
                            var rtrip = originalMaps.originalTripMap[ctrip.trip_id];
                            var dateToInsert = new TripDate
                            {
                                date = startDate,
                                tripIndex = tripIndexes[rtrip]
                            };
                            tdb.routeDatesMap[rroute].Add(dateToInsert);
                        }
                    }

                    startDate++;
                }
            }

            foreach (var calendar_addition in db.calendar_dates.Where(cd => cd.is_removed == false))
            {
                foreach (var ctrip in serviceDictionary[calendar_addition.service_id])
                {
                    var rroute = originalMaps.originalRouteMap[ctrip.route_id];
                    var rtrip = originalMaps.originalTripMap[ctrip.trip_id];
                    var dateToInsert = new TripDate
                    {
                        date = calendar_addition.date,
                        tripIndex = tripIndexes[rtrip]
                    };
                    tdb.routeDatesMap[rroute].Add(dateToInsert);
                }
            }

            foreach (var r in tdb.routes)
            {
                var minimumDate = tdb.routeDatesMap[r].Min(td => td.date);
                var maximumDate = tdb.routeDatesMap[r].Max(td => td.date);

                var dates = new List<ushort>();
                dates.Add(minimumDate);
                dates.Add(maximumDate);

                foreach (var date in Enumerable.Range(minimumDate, maximumDate - minimumDate + 1))
                {
                    if (!tdb.routeDatesMap[r].Exists(td => td.date == date))
                    {
                        dates.Add((ushort)date);
                    }
                }

                r.dates = dates.ToArray();
                var trips = tdb.trips;
            }
        }

        private DateTime ConvertBackToDate(ushort daysFrom2000)
        {
            var date = new DateTime(2000, 1, 1);
            return date.AddDays(daysFrom2000);
        }
    }
}