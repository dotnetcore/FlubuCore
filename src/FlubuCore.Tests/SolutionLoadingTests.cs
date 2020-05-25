using System.Linq;
using FlubuCore.Tasks.Solution.VSSolutionBrowsing;
using Xunit;

namespace FlubuCore.Tests
{
    public class SolutionLoadingTests
    {
        [Fact]
        public void LoadSolution_OldCsprojAndNewCsproj_Succesfull()
        {
            var solution = VSSolution.Load(@"TestData/Test.sln");
            Assert.Equal(11, solution.Projects.Count);
            Assert.Equal(12, solution.SolutionVersion);
            Assert.Equal("Hsl.Simobil.SelfCare.WebApi", solution.Projects[0].ProjectName);
            Assert.Equal("9a19103f-16f7-4668-be54-9a1e7a4f7556", solution.Projects[0].ProjectTypeGuid.ToString());
            Assert.Equal("213084f6-184a-47ee-95ae-db0f537671ed", solution.Projects[0].ProjectGuid.ToString());
            Assert.Equal("Comtrade.A1.SelfCare.Core.Tests", solution.Projects[1].ProjectName);
        }

        [Fact]
        public void LoadSolution_MixedProjects()
        {
            VSSolution solution = VSSolution.Load(@"TestData/SolutionWithProjects.sln");
            Assert.Equal(3, solution.Projects.Count);
        }

        [Fact]
        public void LoadProjects_MixedProjects()
        {
            VSSolution solution = VSSolution.Load(@"TestData/SolutionWithProjects.sln");
            solution.LoadProjects();

            var coreProjects = solution.Projects.Where(i => i.ProjectDetails.IsNetCoreProjectType).ToList();
            Assert.Equal(2, coreProjects.Count);
        }
    }
}
