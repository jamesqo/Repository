using System;
using Repository.Common;
using JavaObject = Java.Lang.Object;

namespace Repository.JavaInterop.Internal
{
    internal class OnInitialRequestCallback : JavaObject, HighlightRequester.IOnInitialRequestCallback
    {
        private readonly Action<HighlightRequester> _action;

        public OnInitialRequestCallback(Action<HighlightRequester> action)
        {
            Verify.NotNull(action, nameof(action));

            _action = action;
        }

        public void Run(HighlightRequester requester) => _action(requester);
    }
}