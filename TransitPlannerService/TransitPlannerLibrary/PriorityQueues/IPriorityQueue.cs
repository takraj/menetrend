using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TransitPlannerLibrary.PriorityQueues
{
    public interface IPriorityQueue<K>
    {
        void AddWithPriority(K key, IComparable pri);
        K ExtractMin();
        bool IsEmpty();
        void DecreasePriority(K key, IComparable pri);
    }
}
