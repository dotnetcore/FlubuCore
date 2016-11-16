using FlubuCore.Context;
using FlubuCore.Tasks.Text;
using Xunit;

namespace Flubu.Tests.Tasks
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
                "TestData/amazon.json".ExpandToExecutingPath(),
                "TestData/database.json".ExpandToExecutingPath());

            int res = task
                .ExecuteWithResult(Context);

            Assert.Equal(0, res);
        }
    }
}
