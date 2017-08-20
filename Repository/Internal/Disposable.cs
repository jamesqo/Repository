using System;
using Repository.Common;

namespace Repository.Internal
{
    internal class Disposable : IDisposable
    {
        private readonly Action _dispose;

        public Disposable(Action dispose)
        {
            Verify.NotNull(dispose, nameof(dispose));

            _dispose = dispose;
        }

        public void Dispose() => _dispose();
    }
}