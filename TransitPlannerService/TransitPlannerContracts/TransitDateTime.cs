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
        public int year, month, day, hour, minute;
    }
}
