﻿using TransitPlannerLibrary.FlowerDataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PortableUtilityLibrary;

namespace TransitPlannerLibrary.FlowerGraphModel
{
    public class FlowerGraph
    {
        private static readonly TimeSpan NO_DELAY = new TimeSpan(0, 0, 0);

        private readonly IRepository _repository;
        private readonly TimeSpan _getOnOffTime;
        private readonly TimeSpan _maxWaitingTime;
        private readonly double _walkingSpeed;
        private readonly bool _wheelchair;

        protected readonly bool[] _disabledRouteIds;
        protected readonly IDictionary<int, TimeSpan> _tripDelays;

        /// <summary>
        /// Configures a FlowerGraph for trip planning.
        /// </summary>
        /// <param name="repository">Repository that provides the underlying data model.</param>
        /// <param name="disabledVehicleTypes">Enumerable list of unusable route IDs.</param>
        /// <param name="tripDelays">TripID -> AmountOfDelay.</param>
        /// <param name="getOnOffTime">The amount of time that is needed to get on or off a vehicle.</param>
        /// <param name="maxWaitingTime">The maximum willingness to wait at a stop.</param>
        /// <param name="walkingSpeed">Speed of walking in km/h units.</param>
        /// <param name="walkingSpeed">Only enable that subgraph where wheelchair is supported.</param>
        public FlowerGraph(IRepository repository, IEnumerable<int> disabledRouteIds, IDictionary<int, TimeSpan> tripDelays, TimeSpan getOnOffTime, TimeSpan maxWaitingTime, double walkingSpeed, bool needsWheelchairSupport)
        {
            _repository = repository;
            _getOnOffTime = getOnOffTime;
            _maxWaitingTime = maxWaitingTime;
            _walkingSpeed = walkingSpeed;
            _wheelchair = needsWheelchairSupport;

            _disabledRouteIds = new bool[_repository.MetaInfo.CountOfRoutes];
            foreach (var i in disabledRouteIds)
            {
                _disabledRouteIds[i] = true;
            }

            _tripDelays = tripDelays;
        }

        /// <summary>
        /// Provides a connection to the underlying data model.
        /// </summary>
        public IRepository Repository { get { return _repository; } }

        /// <summary>
        /// Stores the amount of time that is needed to get on or off a vehicle.
        /// </summary>
        public TimeSpan GetOnOffTimePerTransfer { get { return _getOnOffTime; } }

        /// <summary>
        /// Stores the maximum willingness to wait at a stop.
        /// </summary>
        public TimeSpan MaxWaitingTimePerTransfer { get { return _maxWaitingTime; } }

        /// <summary>
        /// Stores the walking speed in km/h units.
        /// </summary>
        public double WalkingSpeed { get { return _walkingSpeed; } }

        /// <summary>
        /// Indicated whether the user requeres wheelchair support or not.
        /// </summary>
        public bool NeedsWheelchairSupport { get { return _wheelchair; } }

        /// <summary>
        /// Returns the trip delay.
        /// </summary>
        /// <param name="tripId">ID of the trip.</param>
        /// <returns></returns>
        public TimeSpan GetTripDelay(int tripId)
        {
            if (_tripDelays.ContainsKey(tripId))
            {
                return _tripDelays[tripId];
            }
            else
            {
                return NO_DELAY;
            }
        }

        /// <summary>
        /// Returns the distance between the two Stops (represented by their IDs). The unit is 'km'.
        /// </summary>
        /// <param name="stopId1">ID of the first Stop.</param>
        /// <param name="stopId2">ID of the second Stop.</param>
        /// <returns>Distance inbetween in km.</returns>
        public double GetDistanceBetween(int stopId1, int stopId2)
        {
            var stop1 = _repository.GetStopById(stopId1);
            var stop2 = _repository.GetStopById(stopId2);

            return Haversine.GetDistanceBetween(stop1.Latitude, stop1.Longitude, stop2.Latitude, stop2.Longitude); // km
        }

        /// <summary>
        /// Returns true if the specified Route ID is marked as unusable.
        /// </summary>
        /// <param name="routeId">ID of the route.</param>
        /// <returns>Boolean value.</returns>
        public bool IsRouteDisabled(int routeId)
        {
            return _disabledRouteIds[routeId];
        }
    }
}
