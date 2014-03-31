using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace TransitPlannerContracts
{
    [DataContract]
    public class TransitRoute
    {
        [DataMember]
        public int id;

        [DataMember]
        public string ShortName, LongName, Description;

        [DataMember]
        public string RouteColor, RouteTextColor;

        [DataMember]
        public int RouteType;
    }
}
