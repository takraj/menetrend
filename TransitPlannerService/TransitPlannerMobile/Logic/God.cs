using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransitPlannerMobile.Logic.ViewModels;

namespace TransitPlannerMobile.Logic
{
    public class God
    {
        public static MainPageViewModel mainPageViewModel = new MainPageViewModel
        {
            CountOfDisabledRoutes = "3 járat",
            FromStopName = "Rajmund háza #39",
            ToStopName = "Munkahely #23",
            TypeOfReport = "Baleset",
            WalkingSpeed = "Normál",
            WheelchairSupportNeeded = false,
            WhenDate = DateTime.Now,
            WhenTime = DateTime.Now,
            ReportText = "",
            PositionIncluded = false,
            MaxWaitingTime = "33 perc"
        };

        public static SelectDisabledRoutesViewModel disabledRoutes = new SelectDisabledRoutesViewModel
        {
            Bus = false,
            Tram = false,
            Rail = false,
            Ferry = false,
            Funicular = false,
            Subway = false,
            CableCar = true,
            Gondola = false
        };
    }
}
