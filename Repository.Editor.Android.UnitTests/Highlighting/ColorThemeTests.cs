using NUnit.Framework;
using Repository.Editor.Android.Highlighting;

namespace Repository.Editor.Android.UnitTests.Highlighting
{
    [TestFixture]
    public class ColorThemeTests
    {
        [Test]
        public void Default_IsMonokai()
        {
            Assert.AreSame(ColorTheme.Monokai, ColorTheme.Default);
        }
    }
}