﻿using System.Diagnostics;
using Android.Graphics;
using Repository.Common;
using Repository.Common.Validation;
using Repository.Editor.Android.Internal.JavaInterop;

namespace Repository.Editor.Android.Internal.Editor.Highlighting
{
    [DebuggerDisplay(DebuggerStrings.DisplayFormat)]
    internal struct Coloring
    {
        public const int Size = 8;

        public Coloring(Color color, int count)
        {
            Verify.InRange(count > 0, nameof(count));

            Color = color;
            Count = count;
        }

        public Color Color { get; }

        public int Count { get; }

        private string DebuggerDisplay => $"{nameof(Color)} = {Color}, {nameof(Count)} = {Count}";

        public void WriteTo(NativeByteBuffer buffer)
        {
            buffer.WriteInt32(Color.ToArgb());
            buffer.WriteInt32(Count);
        }
    }
}