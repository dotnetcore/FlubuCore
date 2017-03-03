using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using FlubuCore.Context;

namespace FlubuCore.Tasks
{
    public interface ITask
    {
        Stopwatch TaskStopwatch { get; }

        /// <summary>
        ///     Executes the task using the specified script execution environment.
        /// </summary>
        /// <param name="context">The script execution environment.</param>
        void ExecuteVoid(ITaskContext context);

        Task ExecuteVoidAsync(ITaskContext context);
    }
}
