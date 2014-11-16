using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace TransitPlannerContracts
{
    [DataContract]
    public class TransitTrip
    {
        [DataMember]
        public int id { get; set; }

        [DataMember]
        public string headsign { get; set; }

        [DataMember]
        public int route_id { get; set; }

        [DataMember]
        public int sequence_id { get; set; }
    }
}
