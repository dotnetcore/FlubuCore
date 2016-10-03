using System.Diagnostics;
using FlubuCore.Context;

namespace FlubuCore.Tasks
{
    /// <summary>
    ///     Specifies basic properties and methods for a task.
    /// </summary>
    public interface ITask
    {
        Stopwatch TaskStopwatch { get; }

        /// <summary>
        ///     Executes the task using the specified script execution environment.
        /// </summary>
        /// <param name="context">The script execution environment.</param>
        int Execute(ITaskContext context);
    }
}
