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
        public int id { get; set; }

        [DataMember]
        public string ShortName { get; set; }

        [DataMember]
        public string LongName { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public string RouteColor { get; set; }
        
        [DataMember]
        public string RouteTextColor { get; set; }

        [DataMember]
        public int RouteType { get; set; }
    }
}
