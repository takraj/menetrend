using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FlowerDataModel
{
    public class StopTime
    {
        public int ArrivalTime, DepartureTime;
        public int StopIdx;

        public override string ToString()
        {
            return string.Format("StopTime({0}, {1}, {2})",
                this.ArrivalTime.ToString(), this.DepartureTime.ToString(), this.StopIdx.ToString());
        }
    }
}
