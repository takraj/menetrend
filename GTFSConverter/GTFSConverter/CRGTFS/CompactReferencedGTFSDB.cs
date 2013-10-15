using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTFSConverter.CRGTFS
{
    [ProtoContract]
    public class TransitDB
    {
        [ProtoMember(1)]
        public List<Stop> stops;

        [ProtoMember(2)]
        public List<Route> routes;

        [ProtoMember(3)]
        public List<Trip> trips;
    }

    public class OriginalMaps
    {
        public Dictionary<object, Stop> originalStopMap = new Dictionary<object, Stop>();
        public Dictionary<object, Route> originalRouteMap = new Dictionary<object, Route>();
        public Dictionary<object, Trip> originalTripMap = new Dictionary<object, Trip>();
    }

    [ProtoContract]
    public struct LatLng
    {
        [ProtoMember(1)]
        public float latitude;

        [ProtoMember(2)]
        public float longitude;
    }

    [ProtoContract]
    public struct RGB
    {
        [ProtoMember(1)]
        public byte r;

        [ProtoMember(2)]
        public byte g;

        [ProtoMember(3)]
        public byte b;
    }

    [ProtoContract]
    public struct Transfer
    {
        [ProtoMember(1)]
        public int toStopIndex;

        [ProtoMember(2)]
        public float distance;
    }

    [ProtoContract]
    public struct TripDate
    {
        [ProtoMember(1)]
        public ushort date;

        [ProtoMember(2)]
        public int tripIndex;
    }

    [ProtoContract]
    public struct StopTime
    {
        [ProtoMember(1)]
        public int stopIndex;

        [ProtoMember(2)]
        public ushort arrivalTime;

        [ProtoMember(3)]
        public byte waitingTime;

        [ProtoMember(4)]
        public List<LatLng> shapeSegmentsBefore;
    }

    [ProtoContract]
    public class Stop
    {
        [ProtoMember(1)]
        public string name;

        [ProtoMember(2)]
        public LatLng position;

        [ProtoMember(3)]
        public List<Transfer> transfers;

        [ProtoMember(4)]
        public List<int> knownRoutes;

        [ProtoMember(5)]
        public int idx;
    }

    [ProtoContract]
    public class Route
    {
        [ProtoMember(1)]
        public List<TripDate> dates;

        [ProtoMember(2)]
        public string name;

        [ProtoMember(3)]
        public string description;

        [ProtoMember(4)]
        public byte type;

        [ProtoMember(5)]
        public RGB colour;

        [ProtoMember(6)]
        public RGB textColour;
    }

    [ProtoContract]
    public class Trip
    {
        [ProtoMember(1)]
        public string headsign;

        [ProtoMember(3)]
        public ushort endTime;

        [ProtoMember(4)]
        public bool wheelchairSupport;

        [ProtoMember(5)]
        public List<StopTime> stopTimes;
    }
}
