using FlubuCore.Tasks.Solution;
using FlubuCore.Tasks.Solution.VSSolutionBrowsing;
using Moq;
using Xunit;

namespace FlubuCore.Tests.Tasks
{
    public class LoadSolutionTaskTests : TaskUnitTestBase
    {
        [Fact]
        public void LoadSimpleSolutionWithDuplicateProps()
        {
            Properties.Setup(i => i.Set<VSSolution>("solution", It.IsAny<VSSolution>()));
            var task = new LoadSolutionTask("TestData/TestSln.sln");
            task.ExecuteVoid(Context.Object);
        }
    }
}
