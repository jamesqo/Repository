using System;
using System.Threading.Tasks;
using Repository.Common.Validation;

namespace Repository.Editor.Android.UnitTests.TestInternal.Threading
{
    internal static class TaskExtensions
    {
        public static async Task IgnoreCancellations(this Task task)
        {
            Verify.NotNull(task, nameof(task));

            try
            {
                await task;
            }
            catch (TaskCanceledException)
            {
            }
        }
    }
}