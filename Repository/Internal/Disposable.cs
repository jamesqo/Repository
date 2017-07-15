using System;
using Repository.Common;

namespace Repository.Internal
{
    internal class Disposable : IDisposable
    {
        private readonly Action _dispose;

        private Disposable(Action dispose)
        {
            Verify.NotNull(dispose, nameof(dispose));

            _dispose = dispose;
        }

        public static Disposable Create(Action dispose) => new Disposable(dispose);

        public void Dispose() => _dispose();
    }
}