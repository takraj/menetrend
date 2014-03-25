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

        #region Vehicle Type Constants

        public const int TRAM = 0;
        public const int SUBWAY = 1;
        public const int RAIL = 2;
        public const int BUS = 3;
        public const int FERRY = 4;
        public const int CABLECAR = 5;
        public const int GONDOLA = 6;
        public const int FUNICULAR = 7;

        #endregion

        public string RouteTypeName
        {
            get
            {
                switch (this.RouteType)
                {
                    case TRAM:
                        return "Tram";
                    case SUBWAY:
                        return "Subway";
                    case RAIL:
                        return "Rail";
                    case BUS:
                        return "Bus";
                    case FERRY:
                        return "Ferry";
                    case CABLECAR:
                        return "CableCar";
                    case GONDOLA:
                        return "Gondola";
                    case FUNICULAR:
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
