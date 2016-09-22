using Flubu.Tasks;
using Moq;

namespace Flubu.Tests.Tasks
{
    public abstract class TaskTestBase
    {
        protected TaskTestBase()
        {
            Context = new Mock<ITaskContext>();
        }

        protected Mock<ITaskContext> Context { get; set; }
    }
}
