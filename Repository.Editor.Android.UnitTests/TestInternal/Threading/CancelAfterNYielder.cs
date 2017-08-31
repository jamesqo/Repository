using System.Threading.Tasks;
using Repository.Common.Validation;
using Repository.Editor.Android.Threading;

namespace Repository.Editor.Android.UnitTests.TestInternal.Threading
{
    internal class CancelAfterNYielder : IYielder
    {
        private readonly IYielder _yielder;
        private readonly int _n;

        private int _numberOfYields;

        internal CancelAfterNYielder(IYielder yielder, int n)
        {
            Verify.NotNull(yielder, nameof(yielder));
            Verify.InRange(n >= 0, nameof(n));

            _yielder = yielder;
            _n = n;
        }

        public Task YieldAsync()
        {
            if (_numberOfYields == _n)
            {
                return ThreadingUtilities.CanceledTask;
            }

            _numberOfYields++;
            return _yielder.YieldAsync();
        }
    }
}