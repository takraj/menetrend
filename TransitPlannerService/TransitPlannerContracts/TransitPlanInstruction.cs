using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace TransitPlannerContracts
{
    [DataContract]
    public class TransitPlanInstruction
    {
        [DataMember]
        public TransitStop stop { get; set; }

        [DataMember]
        public int plan_minute { get; set; }

        [DataMember]
        public bool is_walking { get; set; }

        [DataMember]
        public int route_id { get; set; }

        [DataMember]
        public int trip_id { get; set; }
    }
}
