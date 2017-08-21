using System.Diagnostics;

namespace Repository.Internal.IO
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