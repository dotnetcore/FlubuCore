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

            Assert.Equal(4, buildVersion.Version.Major);
            Assert.Equal(5, buildVersion.Version.Build);
            Assert.Equal(1, buildVersion.Version.Minor);
            Assert.Equal(2, buildVersion.Version.Revision);
            Assert.Null(buildVersion.VersionQuality);
        }

        [Fact]
        public void FetchBuildVersionFromFileWithVersionQualityTest()
        {
            var fileName = GetOsPlatform() == OSPlatform.Windows ? @"TestData\Flubu.ProjectVersionWithVersionQuality.txt" : @"TestData/Flubu.ProjectVersionWithVersionQuality.txt";

            Context.Properties.Set(BuildProps.ProductRootDir, ".");
            var task = new FetchBuildVersionFromFileTask();
            task.ProjectVersionFileName(fileName.ExpandToExecutingPath());
            var buildVersion = task.Execute(Context);

            Assert.Equal(17, buildVersion.Version.Major);
            Assert.Equal(11, buildVersion.Version.Minor);
            Assert.Equal(0, buildVersion.Version.Build);
            Assert.Equal(2, buildVersion.Version.Revision);
            Assert.Equal("Beta20", buildVersion.VersionQuality);
        }

        [Fact]
        public void FetchBuildVersionFromFileVersionNotInFirstLineWithPrefixAndSufixTest()
        {
            var fileName = GetOsPlatform() == OSPlatform.Windows ? @"TestData\ReleaseNotes.md" : @"TestData/ReleaseNotes.md";

            Context.Properties.Set(BuildProps.ProductRootDir, ".");
            var task = new FetchBuildVersionFromFileTask();
            task.ProjectVersionFileName(fileName.ExpandToExecutingPath()).RemovePrefix("##").AllowSuffix();
            var buildVersion = task.Execute(Context);

            Assert.Equal(4, buildVersion.Version.Major);
            Assert.Equal(5, buildVersion.Version.Build);
            Assert.Equal(1, buildVersion.Version.Minor);
            Assert.Equal(2, buildVersion.Version.Revision);
            Assert.Null(buildVersion.VersionQuality);
        }
    }
}
