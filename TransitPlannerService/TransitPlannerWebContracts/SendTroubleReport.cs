using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using TransitPlannerContracts;

namespace TransitPlannerWebContracts
{
    [DataContract]
    public class SendTroubleReport
    {
        [DataMember]
        public int category { get; set; }

        [DataMember]
        public string description { get; set; }

        [DataMember]
        public bool has_location_data { get; set; }

        [DataMember]
        public double latitude { get; set; }
        
        [DataMember]
        public double longitude { get; set; }

        [DataMember]
        public TransitDateTime timestamp { get; set; }
    }
}
