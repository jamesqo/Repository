using System;
using Android.Graphics;
using Repository.Editor.Android.Highlighting;
using Repository.Editor.Highlighting;

namespace Repository.Editor.Android.UnitTests.TestInternal.Editor.Highlighting
{
    internal sealed class TestColorTheme : IColorTheme
    {
        public static TestColorTheme Instance { get; } = new TestColorTheme();

        private TestColorTheme()
        {
        }

        public static SyntaxKind Decode(int argb) => (SyntaxKind)argb;

        public static Color Encode(SyntaxKind kind) => new Color(argb: (int)kind);

        public Color BackgroundColor => throw new NotSupportedException();

        public Color GetForegroundColor(SyntaxKind kind) => Encode(kind);
    }
}