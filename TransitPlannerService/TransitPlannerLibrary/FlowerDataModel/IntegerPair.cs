using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowerDataModel
{
    public struct IntegerPair
    {
        private int first, second;

        public IntegerPair(int first, int second)
        {
            this.first = first;
            this.second = second;
        }

        public override bool Equals(object obj)
        {
            if (obj is IntegerPair)
            {
                var other = (IntegerPair)obj;
                return (this.first == other.first) && (this.second == other.second);
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
            return string.Format("({0}, {1})", this.first, this.second);
        }
    }
}
