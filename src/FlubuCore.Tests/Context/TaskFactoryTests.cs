using FlubuCore.Tasks.NetCore;
using FlubuCore.Tasks.Solution;
using Xunit;

namespace FlubuCore.Tests.Context
{
    [Collection(nameof(FlubuTestCollection))]
    public class TaskFactoryTests : FlubuTestBase
    {
        private readonly FlubuTestFixture _fixture;

        public TaskFactoryTests(FlubuTestFixture fixture)
            : base(fixture.LoggerFactory)
        {
            _fixture = fixture;
        }

        [Fact]
        public void ResolveSimpleTask()
        {
            LoadSolutionTask instance = Factory.Create<LoadSolutionTask>();
            Assert.IsType<LoadSolutionTask>(instance);
        }

        [Fact]
        public void ResolveTaskWithArgument()
        {
            LoadSolutionTask instance = Factory.Create<LoadSolutionTask>("solutionName");
            Assert.IsType<LoadSolutionTask>(instance);
            Assert.Equal("solutionName", instance.SolutionFile);
        }

        [Fact]
        public void ResolveDotnetTaskWithArguments()
        {
            ExecuteDotnetTask instance = Factory.Create<ExecuteDotnetTask>("compile");
            Assert.IsType<ExecuteDotnetTask>(instance);
            Assert.Equal("compile", instance.Command);
        }
    }
}
