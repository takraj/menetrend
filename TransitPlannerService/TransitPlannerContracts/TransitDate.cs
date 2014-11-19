using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace TransitPlannerContracts
{
    [DataContract]
    public class TransitDate
    {
        [DataMember]
        public int year { get; set; }

        [DataMember]
        public int month { get; set; }

        [DataMember]
        public int day { get; set; }

        public DateTime AsDate
        {
            get
            {
                return new DateTime(year, month, day);
            }
        }
    }
}
