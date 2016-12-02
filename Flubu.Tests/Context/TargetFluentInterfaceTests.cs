using FlubuCore.Context;
using FlubuCore.Context.FluentInterface;
using FlubuCore.Context.FluentInterface.Interfaces;
using FlubuCore.Context.FluentInterface.TaskExtensions;
using FlubuCore.Targeting;
using Moq;
using Xunit;

namespace Flubu.Tests.Context
{
    public class TargetFluentInterfaceTests
    {
        private readonly TargetFluentInterface _fluent;
        private readonly Mock<ITaskContextInternal> _context;
        private readonly Mock<ITarget> _target;

        public TargetFluentInterfaceTests()
        {
            _context = new Mock<ITaskContextInternal>();
            _target = new Mock<ITarget>();

            _fluent = new TargetFluentInterface(
                new TaskFluentInterface(new IisTaskFluentInterface()),
                new CoreTaskFluentInterface(new LinuxTaskFluentInterface()),
                new TaskExtensionsFluentInterface());

            _fluent.Target = _target.Object;
            _fluent.Context = _context.Object;
        }

        [Fact]
        public void DependsOnStringTest()
        {
            ITargetFluentInterface t = _fluent.DependsOn("target1");
            Assert.NotNull(t);

            _target.Verify(i => i.DependsOn("target1"), Times.Once);
        }

        [Fact]
        public void DependsOnTargetTest()
        {
            Mock<ITarget> target1 = new Mock<ITarget>();
            ITargetFluentInterface t = _fluent.DependsOn(target1.Object);
            Assert.NotNull(t);

            _target.Verify(i => i.DependsOn(target1.Object), Times.Once);
        }
    }
}
