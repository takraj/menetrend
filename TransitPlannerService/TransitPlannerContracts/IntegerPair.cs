using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace TransitPlannerContracts
{
    [DataContract]
    public class IntegerPair
    {
        [DataMember]
        public int key { get; set; }

        [DataMember]
        public int value { get; set; }
    }
}
