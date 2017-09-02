using System;
using Java.Lang;
using Repository.Common.Validation;

namespace Repository.JavaInterop.Internal
{
    internal class Runnable : Java.Lang.Object, IRunnable
    {
        private readonly Action _action;

        public Runnable(Action action)
        {
            Verify.NotNull(action, nameof(action));

            _action = action;
        }

        public void Run() => _action();
    }
}