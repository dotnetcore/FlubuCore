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
        /// Status of the task.
        /// </summary>
        TaskStatus TaskStatus { get; }

        /// <summary>
        /// If true task is target otherwise it is task.
        /// </summary>
        bool IsTarget { get; }

        /// <summary>
        /// Enables / disables sequential logging in asynchronous executed tasks.
        /// </summary>
        bool SequentialLogging { get; set; }

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
