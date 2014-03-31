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
        public TransitDateTime base_time, end_time;

        [DataMember]
        public int route_duration; // minutes

        [DataMember]
        public double route_length; // km

        [DataMember]
        public string algorithm;

        [DataMember]
        public double plan_computation_time; // seconds

        [DataMember]
        public List<TransitPlanInstruction> instructions;

        [DataMember]
        public List<TransitRoute> used_routes;

        [DataMember]
        public List<TransitTrip> used_trips;
    }
}
