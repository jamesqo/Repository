using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Repository.Common.Validation;
using Repository.Editor.Android.Threading;

namespace Repository.Editor.Android.UnitTests.TestInternal.Threading
{
    internal class CallbackRunnerYielder : IYielder
    {
        private readonly IYielder _yielder;
        private readonly Dictionary<int, Action> _callbackMap;

        private int _numberOfYields;

        public CallbackRunnerYielder(IYielder yielder)
        {
            Verify.NotNull(yielder, nameof(yielder));

            _yielder = yielder;
            _callbackMap = new Dictionary<int, Action>();
        }

        public void SetCallback<TArg>(int numberOfYields, Action<CallbackRunnerYielder, TArg> callback, TArg arg)
        {
            Verify.InRange(numberOfYields >= 0, nameof(numberOfYields));
            Verify.NotNull(callback, nameof(callback));

            _callbackMap.Add(numberOfYields, () => callback(this, arg));
        }

        public Task Yield()
        {
            int n = _numberOfYields;
            _numberOfYields++;

            if (_callbackMap.TryGetValue(n, out var callback))
            {
                _callbackMap.Remove(n);
                callback();
            }

            return _yielder.Yield();
        }
    }
}
