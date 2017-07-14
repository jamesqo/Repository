using System;
using System.Runtime.CompilerServices;

namespace Repository.Internal.Threading
{
    internal struct YieldAwaitable : INotifyCompletion
    {
        public YieldAwaitable GetAwaiter() => this;

        public bool IsCompleted => false;

        public void OnCompleted(Action continuation) => AsyncUtilities.Post(continuation);

        public void GetResult()
        {
        }
    }
}