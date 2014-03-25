using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeapDict;

namespace TransitPlannerLibrary.PriorityQueues
{
    public class BinaryHeapPriorityQueue<K> : IPriorityQueue<K>
    {
        private HeapDict<K, IComparable> q;

        public BinaryHeapPriorityQueue()
        {
            this.q = new HeapDict<K, IComparable>();
        }

        public void AddWithPriority(K key, IComparable pri)
        {
            this.q[key] = pri;
        }

        public K ExtractMin()
        {
            return this.q.PopItem().Key;
        }

        public bool IsEmpty()
        {
            return !this.q.Any();
        }

        public void DecreasePriority(K key, IComparable pri)
        {
            this.AddWithPriority(key, pri);
        }
    }
}
