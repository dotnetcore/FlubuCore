using System.Collections.Generic;
using FlubuCore.Tasks.Text;
using Xunit;

namespace FlubuCore.Tests.Tasks
{
    [Collection(nameof(FlubuTestCollection))]
    public class MergeConfigurationFileTaskTests : FlubuTestBase
    {
        private readonly FlubuTestFixture _fixture;

        public MergeConfigurationFileTaskTests(FlubuTestFixture fixture)
            : base(fixture.LoggerFactory)
        {
            _fixture = fixture;
        }

        [Fact]
        public void UpdateSucess()
        {
            MergeConfigurationTask task = new MergeConfigurationTask(
                "merged.json".ExpandToExecutingPath(),
                new List<string>
                {
                    "TestData/amazon.json".ExpandToExecutingPath(),
                    "TestData/database.json".ExpandToExecutingPath()
                });

            int res = task
                .Execute(Context);

            Assert.Equal(0, res);
        }
    }
}
