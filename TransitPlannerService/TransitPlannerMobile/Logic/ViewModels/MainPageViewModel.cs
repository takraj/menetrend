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
        private DateTime _when;
        private string _maxWaitingTime, _walkSpeed;

        public void OnPropertyChanged(string PropertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(PropertyName));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        // Tervező paraméterei

        private string _fromStopName, _toStopName;
        private bool _wheelchairSupport;

        public string FromStopName
        {
            get
            {
                return _fromStopName;
            }
            set
            {
                _fromStopName = value;
                OnPropertyChanged("FromStopName");
            }
        }

        public string ToStopName
        {
            get
            {
                return _toStopName;
            }
            set
            {
                _toStopName = value;
                OnPropertyChanged("ToStopName");
            }
        }

        public bool WheelchairSupportNeeded
        {
            get
            {
                return _wheelchairSupport;
            }
            set
            {
                _wheelchairSupport = value;
                OnPropertyChanged("WheelchairSupportNeeded");
            }
        }

        public DateTime WhenDate
        {
            get
            {
                return _when;
            }
            set
            {
                _when = new DateTime(value.Year, value.Month, value.Day, _when.Hour, _when.Minute, 0);
                OnPropertyChanged("WhenDate");
                OnPropertyChanged("When");
            }
        }

        public DateTime WhenTime
        {
            get
            {
                return _when;
            }
            set
            {
                _when = new DateTime(_when.Year, _when.Month, _when.Day, value.Hour, value.Minute, 0);
                OnPropertyChanged("WhenTime");
                OnPropertyChanged("When");
            }
        }

        public string MaxWaitingTime
        {
            get
            {
                return _maxWaitingTime;
            }
            set
            {
                _maxWaitingTime = value;
                OnPropertyChanged("MaxWaitingTime");
            }
        }

        public string WalkingSpeed
        {
            get
            {
                return _walkSpeed;
            }
            set
            {
                _walkSpeed = value;
                OnPropertyChanged("WalkingSpeed");
            }
        }

        public DateTime When
        {
            get
            {
                return WhenDate.Date.AddHours(WhenTime.Hour).AddMinutes(WhenTime.Minute);
            }
            set
            {
                WhenDate = value.Date;
                WhenTime = value;
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

        private string _typeOfReport;
        private bool _positionIncluded;
        private string _reportText;

        public string TypeOfReport
        {
            get
            {
                return _typeOfReport;
            }
            set
            {
                _typeOfReport = value;
                OnPropertyChanged("TypeOfReport");
            }
        }

        public bool PositionIncluded
        {
            get
            {
                return _positionIncluded;
            }
            set
            {
                _positionIncluded = value;
                OnPropertyChanged("PositionIncluded");
            }
        }

        public string ReportText
        {
            get
            {
                return _reportText;
            }
            set
            {
                _reportText = value;
                OnPropertyChanged("ReportText");
            }
        }

        // Segédfüggvények

        public int GetWalkingSpeedCategoryNumerically()
        {
            var wsd = new Dictionary<string, int>();
            wsd["Lassú"] = 1;
            wsd["Átlagos"] = 0;
            wsd["Gyors"] = 2;

            return wsd[this.WalkingSpeed];
        }

        public int GetMaxWaitingTimeCategoryNumerically()
        {
            var wsd = new Dictionary<string, int>();
            wsd["10 perc"] = 0;
            wsd["30 perc"] = 1;
            wsd["1 óra"] = 2;
            wsd["Bármennyi"] = 3;

            return wsd[this.MaxWaitingTime];
        }

        public ICollection<int> GetDisabledRouteTypes()
        {
            var disabledRouteTypes = new HashSet<int>();

            if (God.disabledRoutes.Funicular) { disabledRouteTypes.Add(7); }
            if (God.disabledRoutes.Gondola) { disabledRouteTypes.Add(6); }
            if (God.disabledRoutes.CableCar) { disabledRouteTypes.Add(5); }
            if (God.disabledRoutes.Ferry) { disabledRouteTypes.Add(4); }
            if (God.disabledRoutes.Bus) { disabledRouteTypes.Add(3); }
            if (God.disabledRoutes.Rail) { disabledRouteTypes.Add(2); }
            if (God.disabledRoutes.Subway) { disabledRouteTypes.Add(1); }
            if (God.disabledRoutes.Tram) { disabledRouteTypes.Add(0); }

            return disabledRouteTypes;
        }
    }
}
