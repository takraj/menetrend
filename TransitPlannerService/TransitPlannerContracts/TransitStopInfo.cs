using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace TransitPlannerContracts
{
    [DataContract]
    public class TransitStopInfo
    {
        [DataMember]
        public TransitStop stop { get; set; }

        [DataMember]
        public IList<TransitRoute> available_routes { get; set; }
    }
}
