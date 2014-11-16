using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using TransitPlannerContracts;

namespace TransitPlannerWebContracts
{
    [DataContract]
    public class WebTransitPlanRequestParameters
    {
        [DataMember]
        public int from { get; set; }

        [DataMember]
        public int to { get; set; }

        [DataMember]
        public TransitDateTime when { get; set; }

        [DataMember]
        public int max_waiting_time { get; set; } // minutes

        [DataMember]
        public int walking_speed_category { get; set; }

        [DataMember]
        public bool needs_wheelchair_support { get; set; }

        [DataMember]
        public List<int> disabled_route_types { get; set; }
    }
}
