using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Repository.Internal.Diagnostics
{
    internal class DebugAssertException : Exception
    {
        public DebugAssertException(string message)
            : base(message)
        {
        }
    }
}