using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TransitPlannerLibrary.FlowerDataModel
{
    public class Metadata
    {
        public DateTime MinDate, MaxDate;
        public int MaxSpeed;
        public int CountOfCalendars, CountOfServiceDays, CountOfSequences;
        public int CountOfStops, CountOfTrips, CountOfRoutes;
    }
}
