using ProtoBuf;
using System;
using System.Collections.Concurrent;
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
        public IDictionary<ushort, Stop> originalStopMap = new ConcurrentDictionary<ushort, Stop>();
        public IDictionary<ushort, Route> originalRouteMap = new ConcurrentDictionary<ushort, Route>();
        public IDictionary<uint, Trip> originalTripMap = new ConcurrentDictionary<uint, Trip>();
        public IDictionary<ushort, int> originalShapeIndexMap = new ConcurrentDictionary<ushort, int>();
        public IDictionary<string, int> headsignMap = new ConcurrentDictionary<string, int>();
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

        public override string ToString()
        {
            return String.Format("ShapeVector: len/2 = {0}", verticesVector.Length / 2);
        }
    }

    [ProtoContract]
    public struct LatLng
    {
        [ProtoMember(1)]
        public float latitude;

        [ProtoMember(2)]
        public float longitude;

        public override string ToString()
        {
            return String.Format("LatLng: {0}, {1}", latitude, longitude);
        }
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

        public override string ToString()
        {
            return String.Format("RGB: {0}, {1}, {2}", r, g, b);
        }
    }

    [ProtoContract]
    public struct TripDate
    {
        [ProtoMember(1)]
        public ushort date;

        [ProtoMember(2)]
        public int tripIndex;

        public override string ToString()
        {
            return String.Format("TripDate: {0} for trip {1}", Utility.ConvertBackToDate(date), tripIndex);
        }
    }

    [ProtoContract]
    public struct StopTime
    {
        /// <summary>
        /// {stopIdx, tripIdx, shapeIdx, shapeDistanceTravelled}
        /// </summary>
        [ProtoMember(1)]
        public int[] refIndices;

        /// <summary>
        /// Érkezés: Éjfél óta eltelt percek.
        /// </summary>
        [ProtoMember(2)]
        public ushort arrivalTime;

        /// <summary>
        /// Percek
        /// </summary>
        [ProtoMember(3)]
        public byte waitingTime;

        public override string ToString()
        {
            return String.Format("StopTime: arrives {0} waits {1} for trip {2}", arrivalTime, waitingTime, refIndices[1]);
        }
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
        /// {stopIdx, stopDst, stopIdx, stopDst, stopIdx, stopDst, ...}
        /// </summary>
        [ProtoMember(5)]
        public int[] nearbyStops;

        [ProtoMember(6)]
        public ushort firstTripArrives;

        [ProtoMember(7)]
        public ushort lastTripArrives;

        public override bool Equals(object obj)
        {
            if (obj is Stop)
            {
                var other = (Stop)obj;
                return (this.idx == other.idx);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return this.idx.GetHashCode();
        }

        public override string ToString()
        {
            return String.Format("Stop: {0} (idx={1})", name, idx);
        }
    }

    [ProtoContract]
    public class Route
    {
        /// <summary>
        /// Kivételek (napok 2000 óta)
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

        public override bool Equals(object obj)
        {
            if (obj is Route)
            {
                var other = (Route)obj;
                return (this.idx == other.idx);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return this.idx.GetHashCode();
        }

        public override string ToString()
        {
            return String.Format("Route: {0} (idx={1}, type={2})", name, idx, type);
        }
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

        [ProtoMember(6)]
        public int idx;

        [ProtoMember(7)]
        public int routeIndex;

        [ProtoMember(8)]
        public int stopSequenceHint;

        public override bool Equals(object obj)
        {
            if (obj is Trip)
            {
                var other = (Trip)obj;
                return (this.idx == other.idx);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return this.idx.GetHashCode();
        }

        public override string ToString()
        {
            return String.Format("Trip: ends at {0}:{1} (headsign={2}, idx={3})", endTime/60, endTime%60, headsignIdx, idx);
        }
    }
}
