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
                var processed = new byte[fragment.Position()];
                var remaining = new byte[fragment.Remaining()];
                fragment.Get(remaining);
                fragment.Rewind();
                fragment.Get(processed);
                return $"Processed: {GetDebugString(processed)}\nRemaining: {GetDebugString(remaining)}";
            }

            private static string GetDebugString(byte[] byteArray)
            {
                var sb = new StringBuilder();
                sb.Append('[');
                sb.Append(byteArray[0].ToString("X"));
                for (int i = 1; i < byteArray.Length; i++)
                {
                    sb.Append(' ');
                    sb.Append(byteArray[i].ToString("X"));
                }
                sb.Append(']');
                return sb.ToString();
            }
        }
    }
}