using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace TransitPlannerContracts
{
    [DataContract]
    public class TransitSequenceInfo
    {
        [DataMember]
        public int id; // sequence id

        [DataMember]
        public int count_of_stops;

        [DataMember]
        public string headsign;

        [DataMember]
        public TransitStop first_stop, last_stop;
    }
}
