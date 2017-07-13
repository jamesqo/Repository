using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Graphics;
using Repository.JavaInterop;

namespace Repository.Internal.Editor.Highlighting
{
    internal class ColoredTextSource
    {
        private ColoredTextSource(string text)
        {
            throw null;
        }

        public static ColoredTextSource Create(string text) => new ColoredTextSource(text);

        public ColoredText InitialWindow
        {
            get
            {

            }
        }

        public void Color(Color color, int count)
        {

        }
    }
}