using System.Diagnostics;
using System.Threading.Tasks;
using FlubuCore.Context;

namespace FlubuCore.Tasks
{
    public interface ITask
    {
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
        Task ExecuteVoidAsync(ITaskContext context);
    }
}
