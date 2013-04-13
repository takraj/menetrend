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

        public int RouteId { get { return _routeId; } }

        public RouteEdge(int toStopId, int routeId, DateTime date, TimeSpan time)
        {
            _toStopId = toStopId;
            _routeId = routeId;
            _date = date;
            _time = time;
        }

        public Edge Clone()
        {
            return new RouteEdge(_toStopId, _routeId, new DateTime(_date.Ticks), new TimeSpan(_time.Ticks));
        }

        public int? GetCost() {
            var when = new DateTime(_date.Year, _date.Month, _date.Day, _time.Hours, _time.Minutes, _time.Seconds);

            var dep = CostCalculator.GetNextDeparture(when, _routeId, _toStopId);

            if (dep == null)
            {
                return null;
            }
            else
            {
                return (int)(((TimeSpan)(dep - when.TimeOfDay)).TotalMinutes);
            }
        }

        public override string ToString()
        {
            return "( " + DbManager.GetRouteById(_routeId).RouteShortName + " | " + _time.ToString() + " ) --> '" + DbManager.GetStopById(_toStopId).StopName + "'";
        }
    }
}
