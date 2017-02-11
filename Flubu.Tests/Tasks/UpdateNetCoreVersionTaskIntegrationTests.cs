using System;
using System.Collections.Generic;
using System.Text;
using FlubuCore.Context;
using FlubuCore.IO.Wrappers;
using Xunit;
using FlubuCore.Tasks.Text;
using FlubuCore.Tasks.Versioning;
using Microsoft.Extensions.Logging;

namespace Flubu.Tests.Tasks
{
    [Collection(nameof(FlubuTestCollection))]
    public class UpdateNetCoreVersionTaskIntegrationTests : FlubuTestBase
    {
        public UpdateNetCoreVersionTaskIntegrationTests(FlubuTestFixture fixture) : base(fixture.LoggerFactory)
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
