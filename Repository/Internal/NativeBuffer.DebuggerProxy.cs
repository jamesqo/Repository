using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Repository.Internal
{
    internal partial class NativeBuffer
    {
        private class DebuggerProxy
        {
            private readonly NativeBuffer _buffer;

            public DebuggerProxy(NativeBuffer buffer) => _buffer = buffer;

            [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
            public byte[] Contents => _buffer.ToByteArray();
        }
    }
}