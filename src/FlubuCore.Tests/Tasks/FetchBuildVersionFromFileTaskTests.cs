using System.Runtime.InteropServices;
using FlubuCore.Context;
using FlubuCore.Tasks.Versioning;
using Xunit;

namespace FlubuCore.Tests.Tasks
{
    [Collection(nameof(FlubuTestCollection))]
    public class FetchBuildVersionFromFileTaskTests : FlubuTestBase
    {
        private FlubuTestFixture _fixture;

        public FetchBuildVersionFromFileTaskTests(FlubuTestFixture fixture)
            : base(fixture.LoggerFactory)
        {
            _fixture = fixture;
        }

        [Fact]
        public void FetchBuildVersionFromFileTest()
        {
            var fileName = GetOsPlatform() == OSPlatform.Windows ? @"TestData\Flubu.ProjectVersion.txt" : @"TestData/Flubu.ProjectVersion.txt";

            Context.Properties.Set(BuildProps.ProductRootDir, ".");
            var task = new FetchBuildVersionFromFileTask();
            task.ProjectVersionFileName(fileName.ExpandToExecutingPath());
            var buildVersion = task.Execute(Context);

            Assert.Equal(4, buildVersion.Major);
            Assert.Equal(5, buildVersion.Build);
            Assert.Equal(1, buildVersion.Minor);
            Assert.Equal(2, buildVersion.Revision);
        }

        [Fact]
        public void FetchBuildVersionFromFileVersionNotInFirstLineWithPrefixAndSufixTest()
        {
            var fileName = GetOsPlatform() == OSPlatform.Windows ? @"TestData\ReleaseNotes.md" : @"TestData/ReleaseNotes.md";

            Context.Properties.Set(BuildProps.ProductRootDir, ".");
            var task = new FetchBuildVersionFromFileTask();
            task.ProjectVersionFileName(fileName.ExpandToExecutingPath()).RemovePrefix("##").AllowSuffix();
            var buildVersion = task.Execute(Context);

            Assert.Equal(4, buildVersion.Major);
            Assert.Equal(5, buildVersion.Build);
            Assert.Equal(1, buildVersion.Minor);
            Assert.Equal(2, buildVersion.Revision);
        }
    }
}
