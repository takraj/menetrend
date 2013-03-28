using MTR.Common.Gtfs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTR.WebApp.Common.ViewModels
{
    public class VMDL_Stop
    {
        public String StopId;
        public String StopName;
        public Double StopLatitude;
        public Double StopLongitude;
        public E_LocationType LocationType;
        public String ParentStation;
        public E_WheelchairSupport WheelchairBoarding;

        public VMDL_Stop(string id, string name, double lat, double lon, E_LocationType loctype, String parentStation, E_WheelchairSupport wheelchair)
        {
            this.StopId = id;
            this.StopName = name;
            this.StopLatitude = lat;
            this.StopLongitude = lon;
            this.LocationType = loctype;
            this.ParentStation = parentStation;
            this.WheelchairBoarding = wheelchair;
        }

        public bool HasSimilarNameTo(string subjectName)
        {
            if (this.StopName.Equals(subjectName))
            {
                return true;
            }

            var allowedDifferences = new List<char>();
            allowedDifferences.Add(' ');
            allowedDifferences.Add('+');
            allowedDifferences.Add('/');
            allowedDifferences.Add('M');
            allowedDifferences.Add('H');

            if (this.StopName.StartsWith(subjectName))
            {
                var tail = this.StopName.Substring(subjectName.Length, this.StopName.Length - subjectName.Length);
                if (tail.Length > 1)
                {
                    foreach (char c in tail.ToCharArray())
                    {
                        if (!allowedDifferences.Exists(diff => diff.CompareTo(c) == 0))
                        {
                            return false;
                        }
                    }
                }
                return true;
            }
            else if (subjectName.StartsWith(this.StopName))
            {
                var tail = subjectName.Substring(this.StopName.Length, subjectName.Length - this.StopName.Length);
                if (tail.Length > 1)
                {
                    foreach (char c in tail.ToCharArray())
                    {
                        if (!allowedDifferences.Exists(diff => diff.CompareTo(c) == 0))
                        {
                            return false;
                        }
                    }
                }
                return true;
            }

            return false;
        }
    }
}
