using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTR.BusinessLogic.Pathfinder.Dijkstra
{
    class TransferEdge : Edge
    {
        public int? GetCost()
        {
            return 10;  // Egyelőre az átszállás költsége FIX 10 perc!
        }

        public override string ToString()
        {
            return "(fel-/leszállás)";
        }
    }
}
