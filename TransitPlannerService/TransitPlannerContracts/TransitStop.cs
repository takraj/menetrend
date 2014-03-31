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
        public int id;

        [DataMember]
        public string name;

        [DataMember]
        public double latitude;

        [DataMember]
        public double longitude;

        [DataMember]
        public bool has_wheelchair_support;

        [DataMember]
        public string postal_code;

        [DataMember]
        public string city;

        [DataMember]
        public string street;
    }
}
