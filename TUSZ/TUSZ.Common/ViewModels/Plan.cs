using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace TUSZ.Common.ViewModels
{
    [DataContract]
    public class VM_PlanResult
    {
        [DataMember]
        public bool isFailed;

        [DataMember]
        public string failMessage;

        [DataMember]
        public VM_Plan plan;
    }

    [DataContract]
    public class VM_Stop
    {
        [DataMember]
        public int id;

        [DataMember]
        public string name;

        [DataMember]
        public float lat;

        [DataMember]
        public float lng;
    }

    [DataContract]
    public class VM_Route
    {
        [DataMember]
        public int id;

        [DataMember]
        public string name;

        [DataMember]
        public string html_text_color;

        [DataMember]
        public string html_base_color;
    }

    [DataContract]
    public class VM_PassedStop
    {
        [DataMember]
        public VM_Stop stop;

        [DataMember]
        public DateTime when;

        [DataMember]
        public bool getOnOff;
    }

    [DataContract]
    public class VM_TravelGroup
    {
        [DataMember]
        public VM_Route route;

        [DataMember]
        public VM_Stop from;

        [DataMember]
        public DateTime from_time;

        [DataMember]
        public VM_Stop to;

        [DataMember]
        public DateTime to_time;

        [DataMember]
        public List<VM_PassedStop> passed_stops;
    }

    [DataContract]
    public class VM_Plan
    {
        [DataMember]
        public string algorythm_name;

        [DataMember]
        public TimeSpan plan_time;

        [DataMember]
        public DateTime plan_begins;

        [DataMember]
        public DateTime plan_ends;

        [DataMember]
        public VM_Stop source;

        [DataMember]
        public VM_Stop destination;

        [DataMember]
        public List<VM_TravelGroup> travel_groups;
    }
}