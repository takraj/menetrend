using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace TransitPlannerContracts
{
    [DataContract]
    public class TransitPlan
    {
        [DataMember]
        public TransitDateTime base_time { get; set; }
        
        [DataMember]
        public TransitDateTime end_time { get; set; }

        [DataMember]
        public int route_duration { get; set; } // minutes

        [DataMember]
        public double route_length { get; set; } // km

        [DataMember]
        public string algorithm { get; set; }

        [DataMember]
        public double plan_computation_time { get; set; } // seconds

        [DataMember]
        public List<TransitPlanInstruction> instructions { get; set; }

        [DataMember]
        public List<TransitRoute> used_routes { get; set; }

        [DataMember]
        public List<TransitTrip> used_trips { get; set; }
    }
}
