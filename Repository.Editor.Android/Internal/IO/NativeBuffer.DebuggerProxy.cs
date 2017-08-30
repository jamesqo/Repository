using System.Diagnostics;

namespace Repository.Editor.Android.Internal.IO
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