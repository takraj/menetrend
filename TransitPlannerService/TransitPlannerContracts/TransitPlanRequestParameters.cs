using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace TransitPlannerContracts
{
    [DataContract]
    public class TransitPlanRequestParameters
    {
        [DataMember]
        public int from, to;

        [DataMember]
        public TransitDateTime when;

        [DataMember]
        public int get_on_off_time; // minutes

        [DataMember]
        public int max_waiting_time; // minutes

        [DataMember]
        public double walking_speed; // kmph

        [DataMember]
        public bool needs_wheelchair_support;

        [DataMember]
        public string use_algorithm;

        [DataMember]
        public List<int> disabled_route_ids;

        [DataMember]
        public List<IntegerPair> trip_delays;
    }
}
