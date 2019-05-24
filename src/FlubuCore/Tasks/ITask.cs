using FlubuCore.Context;

namespace FlubuCore.Tasks
{
    public interface ITask
    {
        /// <summary>
        /// Name of the task.
        /// </summary>
        string TaskName { get; }

        /// <summary>
        ///     Executes the task using the specified script execution environment.
        /// </summary>
        /// <param name="context">The script execution environment.</param>
        void ExecuteVoid(ITaskContext context);

        /// <summary>
        /// Execute task async
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        System.Threading.Tasks.Task ExecuteVoidAsync(ITaskContext context);
    }
}
