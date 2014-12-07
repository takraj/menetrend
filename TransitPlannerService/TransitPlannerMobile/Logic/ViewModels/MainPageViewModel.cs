using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransitPlannerMobile.Logic.ViewModels
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<RoutePlannerService.TransitStop> _availableStops;

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
        public string CountOfDisabledRoutes { get; set; }
        public string MaxWaitingTime { get; set; }
        public string WalkingSpeed { get; set; }


        public ObservableCollection<RoutePlannerService.TransitStop> AvailableStops {
            get {
                return _availableStops;
            }
            set {
                _availableStops = value;
                OnPropertyChanged("AvailableStops");
            }
        }

        // Bejelentések

        public string TypeOfReport { get; set; }
        public bool PositionIncluded { get; set; }
        public string ReportText { get; set; }
    }
}
