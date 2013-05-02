using MTR.BusinessLogic.Common.POCO;
using MTR.DataAccess.EFDataManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTR.BusinessLogic.Pathfinder.Dijkstra
{
    public class TransferEdge : Edge
    {
        private int _cost = 10; // Egyelőre az átszállás költsége FIX 10 perc!
        private int _toStopId;
        private TimeSpan _time;

        public TransferEdge(int toStopId, TimeSpan time, Stop srcStop = null, Stop dstStop = null)
        {
            _toStopId = toStopId;
            _time = time;

            if (srcStop != null && dstStop != null)
            {
                var distanceInMetres = MTR.Common.Utility.measureDistance
                    (
                        srcStop.StopLatitude, srcStop.StopLongitude,
                        dstStop.StopLatitude, dstStop.StopLongitude
                    );

                _cost = 1 + (int)(distanceInMetres * 0.015);        // 1 km kb 15 perc (4kmph)
            }
        }

        public string GetTimeString()
        {
            return _time.Add(TimeSpan.FromMinutes(_cost)).ToString(@"hh\:mm");
        }

        public int GetDestinationStopId()
        {
            return _toStopId;
        }

        public int? GetCost()
        {
            return _cost;
        }

        public override string ToString()
        {
            return "(fel-/leszállás) --> '" + DbManager.GetStopById(_toStopId).StopName + "'";
        }

        public Edge Clone()
        {
            return new TransferEdge(_toStopId, new TimeSpan(_time.Ticks));
        }
    }
}
