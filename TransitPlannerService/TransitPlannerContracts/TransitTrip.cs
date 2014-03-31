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
        public int id;

        [DataMember]
        public string headsign;

        [DataMember]
        public int route_id;

        [DataMember]
        public int sequence_id;
    }
}
