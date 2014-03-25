using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TransitPlannerLibrary.FlowerDataModel
{
    public class Route
    {
        public string ShortName, LongName, Description;
        public string RouteColor, RouteTextColor;
        public int RouteType;
        public int StartOfDay, EndOfDay, Duration;
        public bool DayOverlap;
        public Dictionary<int, List<int>> TripsBySequence = new Dictionary<int, List<int>>();

        public string RouteTypeName
        {
            get
            {
                switch (this.RouteType)
                {
                    case 0:
                        return "Tram";
                    case 1:
                        return "Subway";
                    case 2:
                        return "Rail";
                    case 3:
                        return "Bus";
                    case 4:
                        return "Ferry";
                    case 5:
                        return "CableCar";
                    case 6:
                        return "Gondola";
                    case 7:
                        return "Funicular";
                    default:
                        return "Route";
                }
            }
        }

        public override string ToString()
        {
            return string.Format("{0}({1}, {2})", this.RouteTypeName, this.ShortName, this.Description);
        }
    }
}
