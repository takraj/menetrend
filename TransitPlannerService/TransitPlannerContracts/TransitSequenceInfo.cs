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
        public int id { get; set; } // sequence id

        [DataMember]
        public int count_of_stops { get; set; }

        [DataMember]
        public string headsign { get; set; }

        [DataMember]
        public TransitStop first_stop { get; set; }

        [DataMember]
        public TransitStop last_stop { get; set; }
    }
}
