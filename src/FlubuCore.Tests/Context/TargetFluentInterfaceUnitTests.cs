using FlubuCore.Context;
using FlubuCore.Context.FluentInterface;
using FlubuCore.Context.FluentInterface.Interfaces;
using FlubuCore.Infrastructure;
using FlubuCore.Targeting;
using Moq;
using Xunit;

namespace FlubuCore.Tests.Context
{
    public class TargetFluentInterfaceTests
    {
        private readonly TargetFluentInterface _fluent;
        private readonly Mock<ITaskContextInternal> _context;
        private readonly Mock<ITargetInternal> _target;

        public TargetFluentInterfaceTests()
        {
            _context = new Mock<ITaskContextInternal>();
            _target = new Mock<ITargetInternal>();

            _fluent = new TargetFluentInterface();

            _fluent.Target = _target.Object;
            _fluent.Context = _context.Object;
            _fluent.CoreTaskFluent = new CoreTaskFluentInterface(new LinuxTaskFluentInterface(), new ToolsFluentInterface());
            _fluent.TaskFluent = new TaskFluentInterface(new IisTaskFluentInterface(), new WebApiFluentInterface(), new GitFluentInterface(), new DockerFluentInterface(), new HttpClientFactory());
        }

        [Fact]
        public void DependsOnStringTest()
        {
            ITarget t = _fluent.DependsOn("target1");
            Assert.NotNull(t);

            _target.Verify(i => i.DependsOn("target1"), Times.Once);
        }

        [Fact]
        public void DependsOnTargetTest()
        {
            Mock<ITargetInternal> target1 = new Mock<ITargetInternal>();
            ITarget t = _fluent.DependsOn(target1.Object);
            Assert.NotNull(t);
            _target.Verify(i => i.DependsOn(target1.Object), Times.Once);
        }
    }
}
