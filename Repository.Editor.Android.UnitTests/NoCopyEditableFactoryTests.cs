using Android.Text;
using NSubstitute;
using NUnit.Framework;

namespace Repository.Editor.Android.UnitTests
{
    [TestFixture]
    public class NoCopyEditableFactoryTests
    {
        [Test]
        public void NewEditable_ReturnsSameEditable()
        {
            var source = Substitute.For<IEditable>();

            var result = NoCopyEditableFactory.Instance.NewEditable(source);

            Assert.AreSame(source, result);
        }
    }
}
