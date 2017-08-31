using System.Threading.Tasks;
using Repository.Editor.Android.Threading;

namespace Repository.Editor.Android.Internal.Threading
{
    internal sealed class DefaultYielder : IYielder
    {
        public static DefaultYielder Instance { get; } = new DefaultYielder();

        private DefaultYielder()
        {
        }

        public async Task Yield() => await Task.Yield();
    }
}