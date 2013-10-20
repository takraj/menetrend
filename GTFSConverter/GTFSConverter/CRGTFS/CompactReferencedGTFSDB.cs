using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTFSConverter.CRGTFS
{
    public class TransitDB
    {
        public Stop[] stops;
        public Route[] routes;
        public Trip[] trips;
        public int[] stopDistanceMatrix;
        public List<ShapeVector> shapeMatrix;
        public string[] headsigns;
        public Dictionary<Route, List<TripDate>> routeDatesMap;
    }

    public class OriginalMaps
    {
        public Dictionary<object, Stop> originalStopMap = new Dictionary<object, Stop>();
        public Dictionary<object, Route> originalRouteMap = new Dictionary<object, Route>();
        public Dictionary<object, Trip> originalTripMap = new Dictionary<object, Trip>();
        public Dictionary<object, int> originalShapeIndexMap = new Dictionary<object, int>();
        public Dictionary<string, int> headsignMap = new Dictionary<string, int>();
    }

    [ProtoContract]
    public struct ShapeVector
    {
        /// <summary>
        /// {lat, lng, lat, lng, lat, lng, ...}
        /// 
        /// shapeData[i].lat = verticesVector[(i * 2) + 0]
        /// shapeData[i].lng = verticesVector[(i * 2) + 1]
        /// </summary>
        [ProtoMember(1)]
        public float[] verticesVector;
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
        /// <summary>
        /// {stopIdx, shapeIdx, shapeDistanceTravelled}
        /// </summary>
        [ProtoMember(1)]
        public int[] refIndices;

        [ProtoMember(2)]
        public ushort arrivalTime;

        [ProtoMember(3)]
        public byte waitingTime;
    }

    [ProtoContract]
    public class Stop
    {
        [ProtoMember(1)]
        public string name;

        [ProtoMember(2)]
        public LatLng position;

        [ProtoMember(3)]
        public HashSet<int> knownRoutes;

        [ProtoMember(4)]
        public int idx;

        /// <summary>
        /// list of stop indices
        /// </summary>
        [ProtoMember(5)]
        public int[] nearbyStops;
    }

    [ProtoContract]
    public class Route
    {
        /// <summary>
        /// {minDate, maxDate, noService1, noService2, noService3, ...}
        /// </summary>
        [ProtoMember(1)]
        public ushort[] dates;

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

        [ProtoMember(7)]
        public int idx;
    }

    [ProtoContract]
    public class Trip
    {
        [ProtoMember(1)]
        public int headsignIdx;

        [ProtoMember(3)]
        public ushort endTime;

        [ProtoMember(4)]
        public bool wheelchairSupport;

        [ProtoMember(5)]
        public StopTime[] stopTimes;
    }
}
