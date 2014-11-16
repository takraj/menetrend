using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace TransitPlannerContracts
{
    [DataContract]
    public class TransitDateTime
    {
        [DataMember]
        public int year { get; set; }

        [DataMember]
        public int month { get; set; }

        [DataMember]
        public int day { get; set; }

        [DataMember]
        public int hour { get; set; }

        [DataMember]
        public int minute { get; set; }
    }
}
