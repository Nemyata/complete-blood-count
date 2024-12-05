using System;
using System.Threading.Tasks;

namespace Common.Threading
{
    /// <summary>
    /// Provides synchronous extension methods for tasks.
    /// </summary>
    public static class TaskExtensions
    {
        /// <summary>
        /// Waits for the task to complete, unwrapping any exceptions.
        /// </summary>
        /// <param name="task">The task. May not be <c>null</c>.</param>
        /// <remarks>Код метода заимствован из библиотеки <see href="https://stephencleary.com/book/">AsyncEx</see>, за авторством <see href="https://blog.stephencleary.com/">Stephen Cleary</see>.
        /// Он же является автором книги <see href="https://stephencleary.com/book/">Concurrency in C# Cookbook</see></remarks>
        public static void WaitAndUnwrapException(this Task task)
        {
            if (task == null)
                throw new ArgumentNullException(nameof(task));
            task.GetAwaiter().GetResult();
        }
    }
}
