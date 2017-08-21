using System.Threading.Tasks;
using Repository.Common.Validation;

namespace Repository.Internal.Threading
{
    internal static class TaskExtensions
    {
        /// <summary>
        /// Returns a value indicating whether a task becomes canceled as it is awaited.
        /// </summary>
        /// <param name="task">The task.</param>
        public static async Task<bool> BecomesCanceled(this Task task)
        {
            Verify.NotNull(task, nameof(task));

            try
            {
                await task;
                return false;
            }
            catch (TaskCanceledException)
            {
                return true;
            }
        }
    }
}