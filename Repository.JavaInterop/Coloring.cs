using System.Diagnostics;
using Android.Graphics;
using Repository.Common;

namespace Repository.JavaInterop
{
    [DebuggerDisplay(DebuggerStrings.DisplayFormat)]
    public struct Coloring
    {
        private Coloring(Color color, int count)
        {
            Verify.InRange(count > 0, nameof(count));

            Color = color;
            Count = count;
        }

        public static Coloring Create(Color color, int count) => new Coloring(color, count);

        public Color Color { get; }

        public int Count { get; }

        private string DebuggerDisplay => $"{nameof(Color)}: {Color}, {nameof(Count)}: {Count}";

        public long ToLong() => ((long)Color.ToArgb() << 32) | (uint)Count;
    }
}