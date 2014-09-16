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
        public IList<TransitDateTime> sequence_base_times;

        [DataMember]
        public IList<TransitSequenceElement> sequence_elements;

        [DataMember]
        public TransitSequenceInfo sequence_info;
    }
}
