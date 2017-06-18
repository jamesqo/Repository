using System.Diagnostics;
using Java.Nio;

namespace Repository.Internal.EditorServices.Highlighting
{
    internal partial class FragmentedByteBuffer
    {
        private class DebuggerProxy
        {
            private readonly FragmentedByteBuffer _buffer;

            internal DebuggerProxy(FragmentedByteBuffer buffer) => _buffer = buffer;

            [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
            public ByteBuffer[] Fragments => _buffer.GetFragments();
        }
    }
}