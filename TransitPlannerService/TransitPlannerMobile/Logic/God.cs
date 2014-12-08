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
            FromStopName = "",
            ToStopName = "",
            TypeOfReport = "Baleset",
            WalkingSpeed = "Átlagos",
            WheelchairSupportNeeded = false,
            WhenDate = DateTime.Now,
            WhenTime = DateTime.Now,
            ReportText = "",
            PositionIncluded = false,
            MaxWaitingTime = "30 perc"
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
