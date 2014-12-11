using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

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

        public string CalculationTimeAsString
        {
            get
            {
                return String.Format("{0} s", CalculationTime.ToString("F3"));
            }
        }

        public string PlannedStartTimeAsString
        {
            get
            {
                return PlannedStartTime.ToString("yyyy-MM-dd HH:mm");
            }
        }

        public string RouteLengthAsString
        {
            get
            {
                return String.Format("({0} megálló, kb. {1} km)", RouteLengthStops, RouteLengthKm.ToString("F1"));
            }
        }

        public string RouteLengthInMinutesAsString
        {
            get
            {
                return String.Format("{0} perc", RouteLengthTime);
            }
        }

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

            public string RouteNameAsString
            {
                get
                {
                    if (IsWalking)
                    {
                        return "Gyalog";
                    }
                    else
                    {
                        return RouteInfo.ShortName;
                    }
                }
            }

            private Color ConvertFromHtml(string htmlColor)
            {
                byte r = byte.Parse(htmlColor.Substring(0, 2), NumberStyles.HexNumber);
                byte g = byte.Parse(htmlColor.Substring(2, 2), NumberStyles.HexNumber);
                byte b = byte.Parse(htmlColor.Substring(4, 2), NumberStyles.HexNumber);

                return Color.FromArgb(byte.MaxValue, r, g, b);
            }

            public Brush BackgroundColorAsBrush
            {
                get
                {
                    if (IsWalking)
                    {
                        return new SolidColorBrush(Colors.Transparent);
                    }
                    else
                    {
                        var c = ConvertFromHtml(SectionBadge.BadgeBackgroundColor);
                        return new SolidColorBrush(c);
                    }
                }
            }

            public Brush ForegroundColorAsBrush
            {
                get
                {
                    if (IsWalking)
                    {
                        return new SolidColorBrush(Colors.LightGray);
                    }
                    else
                    {
                        var c = ConvertFromHtml(SectionBadge.BadgeLabelColor);
                        return new SolidColorBrush(c);
                    }
                }
            }
        }
    }
}
