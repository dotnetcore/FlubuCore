using FlubuCore.Context;
using FlubuCore.Tasks.Text;
using Xunit;

namespace FlubuCore.Tests.Tasks
{
    [Collection(nameof(FlubuTestCollection))]
    public class UpdateJsonFileTaskTests : FlubuTestBase
    {
        private readonly FlubuTestFixture _fixture;

        public UpdateJsonFileTaskTests(FlubuTestFixture fixture)
            : base(fixture.LoggerFactory)
        {
            _fixture = fixture;
        }

        [Fact]
        public void MissingFile()
        {
            UpdateJsonFileTask task = new UpdateJsonFileTask("nonext.json");

            TaskExecutionException e = Assert.Throws<TaskExecutionException>(() => task.ExecuteVoid(Context));

            Assert.Equal("JSON file nonext.json not found!", e.Message);
            Assert.Equal(1, e.ErrorCode);
        }

        [Fact]
        public void MissingUpdates()
        {
            UpdateJsonFileTask task = new UpdateJsonFileTask("TestData/testproject.json".ExpandToExecutingPath());

            TaskExecutionException e = Assert.Throws<TaskExecutionException>(() => task.ExecuteVoid(Context));

            Assert.StartsWith("Nothing to update in file", e.Message);
            Assert.Equal(2, e.ErrorCode);
        }

        [Fact]
        public void FailOnUpdateNotFound()
        {
            UpdateJsonFileTask task = new UpdateJsonFileTask("TestData/testproject.json".ExpandToExecutingPath());
            task.Update("notfoundproperty", "test");

            TaskExecutionException e = Assert.Throws<TaskExecutionException>(() => task.ExecuteVoid(Context));

            Assert.StartsWith("Propety notfoundproperty not found in", e.Message);
            Assert.Equal(3, e.ErrorCode);
        }

        [Fact]
        public void FailOnTypeMissmatch()
        {
            UpdateJsonFileTask task = new UpdateJsonFileTask("TestData/testproject.json".ExpandToExecutingPath());
            task
                .FailOnTypeMismatch(true)
                .Update("version", 1);

            TaskExecutionException e = Assert.Throws<TaskExecutionException>(() => task.ExecuteVoid(Context));

            Assert.StartsWith("Propety version type mismatch.", e.Message);
            Assert.Equal(4, e.ErrorCode);
        }

        [Fact]
        public void DontFailOnUpdateNotFound()
        {
            UpdateJsonFileTask task = new UpdateJsonFileTask("TestData/testproject.json".ExpandToExecutingPath());
            task
                .FailIfPropertyNotFound(false)
                .Update("notfoundproperty", "test");

            int res = task.Execute(Context);
            Assert.Equal(3, res);
        }

        [Fact]
        public void UpdateSucess()
        {
            UpdateJsonFileTask task = new UpdateJsonFileTask("TestData/testproject.json".ExpandToExecutingPath());

            int res = task
                .Update("version", "2.0.0.0")
                .Output("project.json.new")
                .Execute(Context);

            Assert.Equal(0, res);
        }

        [Fact]
        public void UpdateSuccessTypeMismatch()
        {
            UpdateJsonFileTask task = new UpdateJsonFileTask("TestData/testproject.json".ExpandToExecutingPath());

            int res = task
                .Update("version", 2)
                .Output("project.json.new")
                .Execute(Context);

            Assert.Equal(4, res);
        }
    }
}
