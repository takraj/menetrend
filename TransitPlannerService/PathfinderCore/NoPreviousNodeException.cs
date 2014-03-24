using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PathfinderCore
{
    public class NoPreviousNodeException : Exception
    {
        public NoPreviousNodeException() : base() { }
    }
}
