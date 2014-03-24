using FlowerDataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowerGraphModel
{
    public class FlowerGraph
    {
        protected IRepository _repository;
        protected TimeSpan _getOnOffTime;
        protected TimeSpan _maxWaitingTime;
        protected double _walkingSpeed;
        protected bool _wheelchair;

        protected HashSet<int> _disabledVehicleTypes;
        protected Dictionary<int, TimeSpan> _tripDelays;

        /// <summary>
        /// Configures a FlowerGraph for trip planning.
        /// </summary>
        /// <param name="repository">Repository that provides the underlying data model.</param>
        /// <param name="disabledVehicleTypes">HashSet containing the unusable vehicle type IDs.</param>
        /// <param name="tripDelays">TripID -> AmountOfDelay.</param>
        /// <param name="getOnOffTime">The amount of time that is needed to get on or off a vehicle.</param>
        /// <param name="maxWaitingTime">The maximum willingness to wait at a stop.</param>
        /// <param name="walkingSpeed">Speed of walking in km/h units.</param>
        /// <param name="walkingSpeed">Only enable that subgraph where wheelchair is supported.</param>
        public FlowerGraph(IRepository repository, HashSet<int> disabledVehicleTypes, Dictionary<int, TimeSpan> tripDelays, TimeSpan getOnOffTime, TimeSpan maxWaitingTime, double walkingSpeed, bool needsWheelchairSupport)
        {
            _repository = repository;
            _getOnOffTime = getOnOffTime;
            _maxWaitingTime = maxWaitingTime;
            _walkingSpeed = walkingSpeed;
            _wheelchair = needsWheelchairSupport;

            _disabledVehicleTypes = disabledVehicleTypes;
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
        public TimeSpan GetDelay(int tripId)
        {
            if (_tripDelays.ContainsKey(tripId))
            {
                return _tripDelays[tripId];
            }
            else
            {
                return new TimeSpan(0, 0, 0);
            }
        }
    }
}
