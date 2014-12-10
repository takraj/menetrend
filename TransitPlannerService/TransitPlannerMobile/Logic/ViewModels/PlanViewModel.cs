using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransitPlannerMobile.Logic.ViewModels
{
    public class VM_Stop : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string PropertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(PropertyName));
            }
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string PostalCode { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }

    public class VM_Route : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string PropertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(PropertyName));
            }
        }

        public string ShortName { get; set; }
        public string Direction { get; set; }
        public string Type { get; set; }
    }

    public class RouteBadgeModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string PropertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(PropertyName));
            }
        }

        public int FontSize { get; set; }
        public int BadgeSize { get; set; }
        public string BadgeLabel { get; set; }
        public string BadgeBackgroundColor { get; set; }
        public string BadgeLabelColor { get; set; }
    }

    public class PlanViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string PropertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(PropertyName));
            }
        }

        public DateTime PlannedStartTime { get; set; }

        public string FirstActionTime { get; set; }
        public string LastActionTime { get; set; }

        public string UsedAlgorithm { get; set; }
        public double CalculationTime { get; set; }

        public int RouteLengthTime { get; set; }
        public double RouteLengthKm { get; set; }
        public int RouteLengthStops { get; set; }

        public List<Section> Sections { get; set; }

        public class Step
        {
            public string When { get; set; }
            public VM_Stop Stop { get; set; }
        }

        public class Section : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;

            public void OnPropertyChanged(string PropertyName)
            {
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs(PropertyName));
                }
            }

            public bool IsWalking { get; set; }
            public VM_Route RouteInfo { get; set; }
            public List<Step> Steps { get; set; }
            public RouteBadgeModel SectionBadge { get; set; }
        }
    }
}
