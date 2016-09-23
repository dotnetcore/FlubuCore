using Flubu.Scripting;
using Flubu.Tasks;
using Microsoft.Extensions.Logging;
using Moq;

namespace Flubu.Tests.Tasks
{
    public abstract class TaskTestBase
    {
        protected TaskTestBase()
        {
            LoggerFactory factory = new LoggerFactory();
            Context = new TaskContext(factory.CreateLogger<TaskSession>(), new CommandArguments());
        }

        protected ITaskContext Context { get; set; }
    }
}
