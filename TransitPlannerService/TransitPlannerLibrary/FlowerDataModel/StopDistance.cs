using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TransitPlannerLibrary.FlowerDataModel
{
    public class StopDistance
    {
        public int StopIdx;
        public double Distance;

        public override string ToString()
        {
            return string.Format("StopDst({0} @ {1} m)", this.StopIdx.ToString(), this.Distance.ToString());
        }
    }
}
