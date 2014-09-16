using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace TransitPlannerContracts
{
    [DataContract]
    public class TransitMetadata
    {
        [DataMember]
        public TransitDate valid_from;

        [DataMember]
        public TransitDate valid_to;

        [DataMember]
        public int valid_duration; // days
    }
}
