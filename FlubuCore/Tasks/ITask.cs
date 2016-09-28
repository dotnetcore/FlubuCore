using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using FlubuCore.Context;

namespace FlubuCore.Tasks
{
    /// <summary>
    ///     Specifies basic properties and methods for a task.
    /// </summary>
    public interface ITask
    {
        /// <summary>
        ///     Gets the task description.
        /// </summary>
        /// <value>The task description.</value>
        string Description { get; }

        Stopwatch TaskStopwatch { get; }

        /// <summary>
        ///     Executes the task using the specified script execution environment.
        /// </summary>
        /// <param name="context">The script execution environment.</param>
        int Execute(ITaskContext context);
    }
}
