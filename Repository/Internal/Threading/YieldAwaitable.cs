using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Android.OS;
using static Repository.Internal.Threading.ThreadingUtilities;

namespace Repository.Internal.Threading
{
    internal struct YieldAwaitable : INotifyCompletion
    {
        public YieldAwaitable GetAwaiter() => this;

        public bool IsCompleted => false;

        public void OnCompleted(Action continuation) => UIThreadHandler.Post(continuation);

        public void GetResult()
        {
        }
    }
}