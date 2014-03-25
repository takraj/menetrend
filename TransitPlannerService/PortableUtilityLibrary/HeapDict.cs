using System;
using System.Collections.Generic;
using System.Linq;

// Ported from Python: https://github.com/DanielStutzbach/heapdict

namespace PortableUtilityLibrary
{
    public class HeapDict<K, V> : IEnumerable<K> where V : IComparable
    {
        private class Wrapper
        {
            public V value;      // idx = 0
            public K key;        // idx = 1
            public int position; // idx = 2

            public Wrapper(V value, K key, int cookie)
            {
                this.value = value;
                this.key = key;
                this.position = cookie;
            }
        }

        private List<Wrapper> heap;
        private Dictionary<K, Wrapper> d;

        public HeapDict()
        {
            this.heap = new List<Wrapper>();
            this.d = new Dictionary<K, Wrapper>();
        }

        private static int parent(int i)
        {
            return ((i - 1) >> 1);
        }

        private static int left(int i)
        {
            return ((i << 1) + 1);
        }

        private static int right(int i)
        {
            return ((i + 1) << 1);
        }

        public void Clear()
        {
            this.heap.Clear();
            this.d.Clear();
        }

        public void SetItem(K key, V value)
        {
            var wrapper = new Wrapper(value, key, this.Length);

            this.d[key] = wrapper;
            this.heap.Add(wrapper);
            this.decrease_key(this.heap.Count - 1);
        }

        private void min_heapify(int i)
        {
            int l = left(i);
            int r = right(i);
            int n = this.heap.Count;
            int low;

            if ((l < n) && (this.heap[l].value.CompareTo(this.heap[i].value) < 0))
            {
                low = l;
            }
            else
            {
                low = i;
            }

            if ((r < n) && (this.heap[r].value.CompareTo(this.heap[low].value) < 0))
            {
                low = r;
            }

            if (low != i)
            {
                this.swap(i, low);
                this.min_heapify(low);
            }
        }

        private void decrease_key(int i)
        {
            while (i > 0)
            {
                int par = parent(i);
                if (this.heap[par].value.CompareTo(this.heap[i].value) < 0)
                {
                    break;
                }
                this.swap(i, par);
                i = par;
            }
        }

        private void swap(int i, int j)
        {
            Wrapper tmp = this.heap[i];
            this.heap[i] = this.heap[j];
            this.heap[j] = tmp;

            this.heap[i].position = i;
            this.heap[j].position = j;
        }

        public void DeleteItem(K key)
        {
            var wrapper = d[key];

            while (wrapper.position > 0)
            {
                var parentpos = parent(wrapper.position);
                var par = this.heap[parentpos];
                this.swap(wrapper.position, par.position);
            }
            this.PopItem();
        }

        public V GetItem(K key)
        {
            return this.d[key].value;
        }

        public IEnumerator<K> GetEnumerator()
        {
            return this.heap.Select(i => i.key).GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public KeyValuePair<K, V> PopItem()
        {
            var wrapper = this.heap[0];

            if (this.heap.Count == 1)
            {
                this.heap.Clear();
            }
            else
            {
                this.heap[0] = this.heap.Last();
                this.heap.RemoveAt(this.heap.Count - 1);
                this.heap[0].position = 0;
                this.min_heapify(0);
            }
            this.d.Remove(wrapper.key);
            return new KeyValuePair<K, V>(wrapper.key, wrapper.value);
        }

        public int Length
        {
            get
            {
                return this.d.Keys.Count;
            }
        }

        public KeyValuePair<K, V> PeekItem()
        {
            return new KeyValuePair<K, V>(this.heap[0].key, this.heap[0].value);
        }

        public V this[K key]
        {
            get
            {
                return this.GetItem(key);
            }
            set
            {
                this.SetItem(key, value);
            }
        }
    }
}
