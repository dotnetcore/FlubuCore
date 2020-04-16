using System;
using FlubuCore.Context;
using FlubuCore.IO.Wrappers;
using FlubuCore.Tasks.Versioning;
using Xunit;

namespace FlubuCore.Tests.Tasks
{
    [Collection(nameof(FlubuTestCollection))]
    public class UpdateNetCoreVersionTaskIntegrationTests : FlubuTestBase
    {
        public UpdateNetCoreVersionTaskIntegrationTests(FlubuTestFixture fixture)
            : base(fixture.LoggerFactory)
        {
        }

        [Fact(Skip = "Explicit run for now.")]
        public void UpdateXmlFileTaskTest()
        {
            var task = new UpdateNetCoreVersionTask(new PathWrapper(), new FileWrapper(), @"TestData/ProjectFiles/UpdateNetCoreVersion.csproj").AddFiles("TestData/ProjectFiles/UpdateNetCoreVersion2.csproj");
            Context.SetBuildVersion(new BuildVersion() { Version = new Version(1, 0, 0, 0), VersionQuality = "preview1" });

            task.Execute(Context);
        }
    }
}
