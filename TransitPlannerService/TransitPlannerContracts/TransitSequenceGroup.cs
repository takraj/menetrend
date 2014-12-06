using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace TransitPlannerContracts
{
    [DataContract]
    public class TransitSequenceGroup
    {
        [DataMember]
        public IList<TransitDateTime> sequence_base_times { get; set; }

        [DataMember]
        public IList<int> sequence_trip_ids { get; set; }

        [DataMember]
        public IList<TransitSequenceElement> sequence_elements { get; set; }

        [DataMember]
        public TransitSequenceInfo sequence_info { get; set; }
    }
}
