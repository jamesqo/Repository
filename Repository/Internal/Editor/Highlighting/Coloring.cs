using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Android.Graphics;

namespace Repository.Internal.Editor.Highlighting
{
    // TODO: Debug view
    internal struct Coloring
    {
        private Coloring(Color color, int count)
        {
            Debug.Assert(count > 0);

            Color = color;
            Count = count;
        }

        public static Coloring Create(Color color, int count) => new Coloring(color, count);

        public Color Color { get; }

        public int Count { get; }

        public static Coloring FromLong(long value)
        {
            var color = new Color((int)(value >> 32));
            int count = (int)value;
            return Create(color, count);
        }

        public long ToLong() => ((long)Color.ToArgb() << 32) | (uint)Count;

        public Coloring WithCount(int count) => Create(Color, count);
    }
}