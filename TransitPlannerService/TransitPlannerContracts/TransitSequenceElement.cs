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
        public TransitStop stop;

        [DataMember]
        public int order;

        [DataMember]
        public int arrival; // minutes relative to basetime

        [DataMember]
        public int departure; // minutes relative to basetime
    }
}
