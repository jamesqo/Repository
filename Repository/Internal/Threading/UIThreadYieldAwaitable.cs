using System;
using System.Runtime.CompilerServices;

namespace Repository.Internal.Threading
{
    internal struct UIThreadYieldAwaitable : INotifyCompletion
    {
        public UIThreadYieldAwaitable GetAwaiter() => this;

        public bool IsCompleted => false;

        public void OnCompleted(Action continuation) => UIThreadUtilities.Post(continuation);

        public void GetResult()
        {
        }
    }
}