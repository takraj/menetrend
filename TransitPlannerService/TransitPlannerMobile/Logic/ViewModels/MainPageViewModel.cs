using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransitPlannerMobile.RoutePlannerService;

namespace TransitPlannerMobile.Logic.ViewModels
{
    public class UniqueTransitStop : TransitStop
    {
        public UniqueTransitStop(TransitStop ts)
        {
            this.city = ts.city;
            this.has_wheelchair_support = ts.has_wheelchair_support;
            this.id = ts.id;
            this.latitude = ts.latitude;
            this.longitude = ts.longitude;
            this.name = ts.name;
            this.postal_code = ts.postal_code;
            this.street = ts.street;
        }

        public string UniqueName
        {
            get
            {
                return String.Format("{0} #{1}", this.name, this.id);
            }
        }
    }

    public class MainPageViewModel : INotifyPropertyChanged
    {
        private List<UniqueTransitStop> _availableStops;

        private void OnPropertyChanged(string PropertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(PropertyName));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        // Tervező paraméterei

        public string FromStopName { get; set; }
        public string ToStopName { get; set; }
        public bool WheelchairSupportNeeded { get; set; }
        public DateTime WhenDate { get; set; }
        public DateTime WhenTime { get; set; }
        public string MaxWaitingTime { get; set; }
        public string WalkingSpeed { get; set; }

        public DateTime When
        {
            get
            {
                return WhenDate.Date.AddHours(WhenTime.Hour).AddMinutes(WhenTime.Minute);
            }
        }

        public List<UniqueTransitStop> AvailableStops
        {
            get
            {
                return _availableStops;
            }
            set
            {
                _availableStops = value;
                OnPropertyChanged("AvailableStops");
            }
        }


        public string CountOfDisabledRoutes
        {
            get
            {
                int count_of_disabled_routes = 0;

                if (God.disabledRoutes.Bus) { count_of_disabled_routes++; }
                if (God.disabledRoutes.CableCar) { count_of_disabled_routes++; }
                if (God.disabledRoutes.Ferry) { count_of_disabled_routes++; }
                if (God.disabledRoutes.Funicular) { count_of_disabled_routes++; }
                if (God.disabledRoutes.Gondola) { count_of_disabled_routes++; }
                if (God.disabledRoutes.Rail) { count_of_disabled_routes++; }
                if (God.disabledRoutes.Subway) { count_of_disabled_routes++; }
                if (God.disabledRoutes.Tram) { count_of_disabled_routes++; }

                return String.Format("{0} jármű", count_of_disabled_routes);
            }
        }

        // Bejelentések

        public string TypeOfReport { get; set; }
        public bool PositionIncluded { get; set; }
        public string ReportText { get; set; }
    }
}
