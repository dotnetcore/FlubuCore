using FlubuCore.Context;
using FlubuCore.Scripting;
using Microsoft.Extensions.Logging;

namespace Flubu.Tests.Tasks
{
    public abstract class TaskTestBase
    {
        protected TaskTestBase(ILoggerFactory loggerFactory)
        {
            Context = new TaskContext(loggerFactory.CreateLogger<TaskSession>(), new TaskContextSession(), new CommandArguments());
        }

        protected ITaskContext Context { get; set; }
    }
}
