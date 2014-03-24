﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FlowerDataModel
{
    public interface IRepository
    {
        Metadata MetaInfo { get; }

        IEnumerable<Stop> Stops { get; }
        IEnumerable<Route> Routes { get; }
        IEnumerable<Trip> Trips { get; }

        Stop GetStopById(int id);
        Route GetRouteById(int id);
        Trip GetTripById(int id);

        bool IsServiceAvailableOnDay(int serviceId, int day);
        IEnumerable<StopTime> GetSequenceById(int id);

        SequenceLookupData LookupNextStop(int sequenceId, int stopId);
    }
}
