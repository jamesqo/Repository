using System;
using System.Threading.Tasks;
using Repository.Common.Validation;

namespace Repository.Editor.Android.UnitTests.TestInternal.Threading
{
    internal static class TaskExtensions
    {
        public static async Task RunToCancellation(this Task task)
        {
            Verify.NotNull(task, nameof(task));

            try
            {
                await task;
                throw new ArgumentException("The task was never canceled.", nameof(task));
            }
            catch (TaskCanceledException)
            {
                return;
            }
        }
    }
}