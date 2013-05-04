using MTR.BusinessLogic.Common.POCO;
using MTR.BusinessLogic.DataTransformer;
using MTR.DataAccess.EFDataManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTR.BusinessLogic.Pathfinder.Dijkstra
{
    public class RouteEdge : Edge
    {
        private int _toStopId;
        private int _routeId;
        private DateTime _date;
        private TimeSpan _time;
        private int? _cost;

        public int RouteId { get { return _routeId; } }

        public RouteEdge(int toStopId, int routeId, DateTime date, TimeSpan time, int? cost = null)
        {
            _toStopId = toStopId;
            _routeId = routeId;
            _date = date;
            _time = time;

            if (cost != null)
            {
                _cost = cost;
            }
            else
            {
                var when = new DateTime(_date.Year, _date.Month, _date.Day, _time.Hours, _time.Minutes, _time.Seconds);
                var dep = CostCalculator.GetNextDeparture(when, _routeId, _toStopId);

                if (dep == null)
                {
                    _cost = null;
                }
                else
                {
                    _cost = (int)(((TimeSpan)(dep - when.TimeOfDay)).TotalMinutes);
                }
            }
        }

        public Edge Clone()
        {
            return new RouteEdge(_toStopId, _routeId, new DateTime(_date.Ticks), new TimeSpan(_time.Ticks), _cost);
        }

        public int GetDestinationStopId()
        {
            return _toStopId;
        }

        public string GetTimeString()
        {
            if (_cost != null)
            {
                var nextDeparture = _time + TimeSpan.FromMinutes((double)_cost);
                return (nextDeparture != null) ? ((TimeSpan)nextDeparture).ToString(@"hh\:mm") : "";
            }

            return "";
        }

        public int? GetCost() {
            return _cost;
        }

        public override string ToString()
        {
            return "( " + DbManager.GetRouteById(_routeId).RouteShortName + " | " + _time.ToString(@"hh\:mm") + " ) --> '" + DbManager.GetStopById(_toStopId).StopName + "'";
        }
    }
}
