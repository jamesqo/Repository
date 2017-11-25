using System.Threading.Tasks;
using Repository.Common.Android.Threading;

namespace Repository.Editor.Android.UnitTests.TestInternal.Threading
{
    internal sealed class NopYielder : IYielder
    {
        public static NopYielder Instance { get; } = new NopYielder();

        private NopYielder()
        {
        }

        public Task Yield() => Task.CompletedTask;
    }
}
