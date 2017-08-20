using System.Diagnostics;
using Android.Graphics;
using Repository.Common;

namespace Repository.Internal.Editor.Highlighting
{
    [DebuggerDisplay(DebuggerStrings.DisplayFormat)]
    internal struct Coloring
    {
        public Coloring(Color color, int count)
        {
            Verify.InRange(count > 0, nameof(count));

            Color = color;
            Count = count;
        }

        public Color Color { get; }

        public int Count { get; }

        private string DebuggerDisplay => $"{nameof(Color)}: {Color}, {nameof(Count)}: {Count}";

        public long ToLong() => ((long)Color.ToArgb() << 32) | (uint)Count;
    }
}