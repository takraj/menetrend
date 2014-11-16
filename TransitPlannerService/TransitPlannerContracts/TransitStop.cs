using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace TransitPlannerContracts
{
    [DataContract]
    public class TransitStop
    {
        [DataMember]
        public int id { get; set; }

        [DataMember]
        public string name { get; set; }

        [DataMember]
        public double latitude { get; set; }

        [DataMember]
        public double longitude { get; set; }

        [DataMember]
        public bool has_wheelchair_support { get; set; }

        [DataMember]
        public string postal_code { get; set; }

        [DataMember]
        public string city { get; set; }

        [DataMember]
        public string street { get; set; }
    }
}
