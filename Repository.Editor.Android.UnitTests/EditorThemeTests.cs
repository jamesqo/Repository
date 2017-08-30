using System.Collections.Generic;
using Android.Graphics;
using NSubstitute;
using NUnit.Framework;

namespace Repository.Editor.Android.UnitTests
{
    [TestFixture]
    public class EditorThemeTests
    {
        public static IEnumerable<object[]> SystemTypefaces_Data()
        {
            yield return new object[] { Typeface.Default };
            yield return new object[] { Typeface.DefaultBold };
            yield return new object[] { Typeface.Monospace };
            yield return new object[] { Typeface.SansSerif };
            yield return new object[] { Typeface.Serif };
        }

        [TestCaseSource(nameof(SystemTypefaces_Data))]
        public void GetDefault_ChoosesInconsolata(Typeface t)
        {
            var typefaces = Substitute.For<ITypefaceProvider>();
            typefaces.Inconsolata.Returns(t);

            var theme = EditorTheme.GetDefault(typefaces);

            Assert.AreSame(t, theme.Typeface);
        }
    }
}
