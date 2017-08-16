using System;
using Java.Lang;
using Repository.Common;
using JavaObject = Java.Lang.Object;

namespace Repository.JavaInterop.Internal
{
    internal class ActionRunnable : JavaObject, IRunnable
    {
        private readonly Action _action;

        public ActionRunnable(Action action)
        {
            Verify.NotNull(action, nameof(action));

            _action = action;
        }

        public void Run() => _action();
    }
}