using System.Threading.Tasks;
using Repository.Editor.Android.Threading;

namespace Repository.Editor.Android.UnitTests.TestInternal.Threading
{
    internal sealed class NopYielder : IYielder
    {
        public static NopYielder Instance { get; } = new NopYielder();

        private NopYielder()
        {
        }

        public Task YieldAsync() => Task.CompletedTask;
    }
}
