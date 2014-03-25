namespace PortableUtilityLibrary
{
    /// <summary>
    /// Represents a Pair which can consist two different types.
    /// </summary>
    /// <typeparam name="T1">Type 1</typeparam>
    /// <typeparam name="T2">Type 2</typeparam>
    public struct Pair<T1, T2>
    {
        public T1 first;
        public T2 second;

        public Pair(T1 first, T2 second)
        {
            this.first = first;
            this.second = second;
        }

        public T1 GetFirst()
        {
            return this.first;
        }

        public override bool Equals(object obj)
        {
            if (obj is Pair<T1, T2>)
            {
                var other = (Pair<T1, T2>)obj;
                return this.first.Equals(other.first) && this.second.Equals(other.second);
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return this.first.GetHashCode() ^ this.second.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("({0}, {1})", this.first.ToString(), this.second.ToString());
        }
    }
}