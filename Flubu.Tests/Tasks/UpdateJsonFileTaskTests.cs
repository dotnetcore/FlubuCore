using System;
using System.IO;
using System.Reflection;
using Flubu.Tasks;
using Flubu.Tasks.Text;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Flubu.Tests.Tasks
{
    [Collection(nameof(TaskTestCollection))]
    public class UpdateJsonFileTaskTests : TaskTestBase
    {
        private readonly TaskTestFixture _fixture;

        public UpdateJsonFileTaskTests(TaskTestFixture fixture)
            : base(fixture.LoggerFactory)
        {
            _fixture = fixture;
        }

        [Fact]
        public void MissingFile()
        {
            UpdateJsonFileTask task = new UpdateJsonFileTask("nonext.json");

            TaskExecutionException e = Assert.Throws<TaskExecutionException>(() => task.Execute(Context));

            Assert.Equal("JSON file nonext.json not found!", e.Message);
            Assert.Equal(1, e.ErrorCode);
        }

        [Fact]
        public void MissingUpdates()
        {
            UpdateJsonFileTask task = new UpdateJsonFileTask("TestData/testproject.json".ExpandToExecutingPath());

            TaskExecutionException e = Assert.Throws<TaskExecutionException>(() => task.Execute(Context));

            Assert.StartsWith("Nothing to update in file", e.Message);
            Assert.Equal(2, e.ErrorCode);
        }

        [Fact]
        public void OpenProjectJsonFile()
        {
            UpdateJsonFileTask task = new UpdateJsonFileTask("TestData/testproject.json".ExpandToExecutingPath());

            task
                .Update("version", JValue.CreateString("2.0.0.0"))
                .Output("project.json.new")
                .Execute(Context);
        }
    }
}
