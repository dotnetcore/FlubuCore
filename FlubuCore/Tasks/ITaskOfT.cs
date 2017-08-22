using System.Threading.Tasks;
using FlubuCore.Context;

namespace FlubuCore.Tasks
{
    /// <summary>
    ///     Specifies basic properties and methods for a task.
    /// </summary>
    public interface ITaskOfT<T> : ITask
    {
        /// <summary>
        ///     Executes the task using the specified script execution environment.
        /// </summary>
        /// <param name="context">The script execution environment.</param>
        T Execute(ITaskContext context);
        
        Task<T> ExecuteAsync(ITaskContext context);

        /// <summary>
        /// Task won't fail if error occurs.
        /// </summary>
        /// <returns></returns>
        ITaskOfT<T> DoNotFailOnError();

        /// <summary>
        /// Retry task if execution of the task fails.
        /// </summary>
        /// <param name="numberOfRetries">Number of retries before task fails.</param>
        /// <param name="delay">Delay time in miliseconds between retries.</param>
        /// <returns></returns>
        ITaskOfT<T> Retry(int numberOfRetries, int delay);
    }
}
