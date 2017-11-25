using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Repository.Editor.Android.Threading;

namespace Repository.Editor.Android.Internal.Threading
{
    internal sealed class DefaultYielder : IYielder
    {
        private struct YieldAwaitable : INotifyCompletion
        {
            public bool IsCompleted => false;

            public YieldAwaitable GetAwaiter() => this;

            public void GetResult()
            {
            }

            public void OnCompleted(Action continuation)
            {
                MostRecentContinuation = continuation;
                ThreadingUtilities.Post(continuation);
            }
        }

        public static DefaultYielder Instance { get; } = new DefaultYielder();

        public static Action MostRecentContinuation { get; private set; }

        private DefaultYielder()
        {
        }

        public async Task Yield() => await new YieldAwaitable();
    }
}
