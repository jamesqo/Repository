using System.Diagnostics;
using System.Linq;
using System.Text;
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
            public string[] Fragments => _buffer.GetFragments().Select(GetDebugString).ToArray();

            private static string GetDebugString(ByteBuffer fragment)
            {
                Debug.Assert(fragment.Position() == 0);
                Debug.Assert(fragment.Capacity() > 0);

                var byteArray = new byte[fragment.Capacity()];
                fragment.Get(byteArray);
                fragment.Rewind();

                var sb = new StringBuilder();
                sb.Append('[');
                sb.Append(byteArray[0].ToString("X2"));
                for (int i = 1; i < byteArray.Length; i++)
                {
                    sb.Append(' ');
                    sb.Append(byteArray[i].ToString("X2"));
                }
                sb.Append(']');
                return sb.ToString();
            }
        }
    }
}