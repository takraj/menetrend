﻿using System;
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
        public int from { get; set; }

        [DataMember]
        public int to { get; set; }

        [DataMember]
        public TransitDateTime when { get; set; }

        [DataMember]
        public int get_on_off_time { get; set; } // minutes

        [DataMember]
        public int max_waiting_time { get; set; } // minutes

        [DataMember]
        public double walking_speed { get; set; } // kmph

        [DataMember]
        public bool needs_wheelchair_support { get; set; }

        [DataMember]
        public string use_algorithm { get; set; }

        [DataMember]
        public List<int> disabled_route_ids { get; set; }

        [DataMember]
        public List<IntegerPair> trip_delays { get; set; }
    }
}
