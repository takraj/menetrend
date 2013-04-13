using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTR.BusinessLogic.Pathfinder.Dijkstra
{
    public interface PriorityQueue
    {
        CompleteNode Pop();
        CompleteNode Peek();
        CompleteNode GetNode(int stopId);
        bool Contains(int stopId);
        void Push(CompleteNode node);
        void UpdateNode(CompleteNode node);
    }
}
