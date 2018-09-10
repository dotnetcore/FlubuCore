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

        [Fact(Skip = "Use test data file.")]
        public void UpdateXmlFileTaskTest()
        {
            var task = new UpdateNetCoreVersionTask(new PathWrapper(), new FileWrapper(), @"K:\_git\FlubuCoreTmp\FlubuCore\FlubuCore.csproj");
            Context.SetBuildVersion(new Version(1, 2, 3, 0));
            task.Execute(Context);
        }
    }
}
