using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace TransitPlannerContracts
{
    public class TransitSequenceElement
    {
        [DataMember]
        public TransitStop stop { get; set; }

        [DataMember]
        public int order { get; set; }

        [DataMember]
        public int arrival { get; set; } // minutes relative to basetime

        [DataMember]
        public int departure { get; set; } // minutes relative to basetime
    }
}
