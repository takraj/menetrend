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
        public TransitDate valid_from { get; set; }

        [DataMember]
        public TransitDate valid_to { get; set; }

        [DataMember]
        public int valid_duration { get; set; } // days
    }
}
