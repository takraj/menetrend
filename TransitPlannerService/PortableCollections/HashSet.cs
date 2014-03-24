using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PortableCollections
{
    public class HashSet<T> : ICollection<T>
    {
        private Dictionary<T, bool> dict;

        public HashSet()
        {
            this.dict = new Dictionary<T, bool>();
        }

        public HashSet(IEnumerable<T> copyFrom)
        {
            foreach (var item in copyFrom)
            {
                this.Add(item);
            }
        }

        public void Add(T item)
        {
            this.dict[item] = true;
        }

        public void Clear()
        {
            this.dict.Clear();
        }

        public bool Contains(T item)
        {
            return this.dict.ContainsKey(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            this.dict.Keys.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return dict.Keys.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(T item)
        {
            if (this.Contains(item))
            {
                dict.Remove(item);
                return true;
            }
            else
            {
                return false;
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return dict.Keys.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
