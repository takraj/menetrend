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
        public TransitStop stop;

        [DataMember]
        public int plan_minute;

        [DataMember]
        public bool is_walking;

        [DataMember]
        public int route_id;

        [DataMember]
        public int trip_id;
    }
}
