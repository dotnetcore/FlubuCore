using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlubuCore.Context;
using FlubuCore.Tasks.Versioning;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Flubu.Tests.Tasks
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
            Context.Properties.Set(BuildProps.ProductRootDir, ".");
            var task = new FetchBuildVersionFromFileTask();
            task.ProjectVersionFileName(@"TestData\Flubu.ProjectVersion.txt".ExpandToExecutingPath());
            var buildVersion = task.Execute(Context);

            Assert.Equal(4, buildVersion.Major);
            Assert.Equal(5, buildVersion.Build);
            Assert.Equal(1, buildVersion.Minor);
            Assert.Equal(2, buildVersion.Revision);
        }
    }
}
