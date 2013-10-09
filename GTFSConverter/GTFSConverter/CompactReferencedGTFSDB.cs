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
        [ProtoMember(1, AsReference = true)]
        List<Stop> stops;

        [ProtoMember(2, AsReference = true)]
        List<Route> routes;

        [ProtoMember(3, AsReference = true)]
        List<Trip> trips;
    }

    [ProtoContract]
    public struct LatLng
    {
        [ProtoMember(1)]
        float latitude;

        [ProtoMember(2)]
        float longitude;
    }

    [ProtoContract]
    public struct RGB
    {
        [ProtoMember(1)]
        byte r;

        [ProtoMember(2)]
        byte g;

        [ProtoMember(3)]
        byte b;
    }

    [ProtoContract]
    public struct Transfer
    {
        [ProtoMember(1, AsReference = true)]
        Stop toStop;

        [ProtoMember(2)]
        float distance;
    }

    [ProtoContract]
    public struct StopTime
    {
        [ProtoMember(1, AsReference = true)]
        Stop stop;

        [ProtoMember(2)]
        ushort arrivalTime;

        [ProtoMember(3)]
        byte waitingTime;

        [ProtoMember(4, AsReference = false)]
        List<LatLng> shapeSegmentsBefore;
    }

    [ProtoContract]
    public class Stop
    {
        [ProtoMember(1)]
        string name;

        [ProtoMember(2, AsReference = false)]
        LatLng position;

        [ProtoMember(3, AsReference = false)]
        List<Transfer> transfers;

        [ProtoMember(4, AsReference = true)]
        List<Route> knownRoutes;
    }

    [ProtoContract]
    public class Route
    {
        [ProtoMember(1, AsReference = true)]
        List<Trip> mondayTrips;

        [ProtoMember(2, AsReference = true)]
        List<Trip> tuesdayTrips;

        [ProtoMember(3, AsReference = true)]
        List<Trip> wednesdayTrips;

        [ProtoMember(4, AsReference = true)]
        List<Trip> thursdayTrips;

        [ProtoMember(5, AsReference = true)]
        List<Trip> fridayTrips;

        [ProtoMember(6, AsReference = true)]
        List<Trip> saturdayTrips;

        [ProtoMember(7, AsReference = true)]
        List<Trip> sundayTrips;

        [ProtoMember(8, AsReference = true)]
        List<Trip> exceptionTrips;

        [ProtoMember(9)]
        string name;

        [ProtoMember(10)]
        string description;

        [ProtoMember(11)]
        byte type;

        [ProtoMember(12, AsReference = false)]
        RGB colour;

        [ProtoMember(13, AsReference = false)]
        RGB textColour;
    }

    [ProtoContract]
    public class Trip
    {
        [ProtoMember(1)]
        string headsign;

        [ProtoMember(2)]
        ulong startDate;

        [ProtoMember(3)]
        ulong endDate;

        [ProtoMember(4)]
        ushort dailyStartTime;

        [ProtoMember(5)]
        ushort dailyEndTime;

        [ProtoMember(6)]
        bool wheelchairSupport;

        [ProtoMember(7, IsPacked=true)]
        List<ulong> noService;

        [ProtoMember(8, AsReference=false)]
        List<StopTime> stopTimes;
    }
}
