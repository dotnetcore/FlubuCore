using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlubuCore.Context;
using FlubuCore.IO.Wrappers;
using FlubuCore.Tasks.Text;
using FlubuCore.Tasks.Versioning;
using Moq;
using Xunit;

namespace Flubu.Tests.Tasks
{
    [Collection(nameof(FlubuTestCollection))]
    public class UpdateNetCoreVersionTaskUnitTests : FlubuTestBase
    {
        private readonly FlubuTestFixture _fixture;

        private Mock<IFileWrapper> _file;

        private Mock<IPathWrapper> _path;

        public UpdateNetCoreVersionTaskUnitTests(FlubuTestFixture fixture)
            : base(fixture.LoggerFactory)
        {
            _file = new Mock<IFileWrapper>();
            _path = new Mock<IPathWrapper>();
            _fixture = fixture;
        }

        [Fact]
        public void MissingFileWithOneProp()
        {
            Context.SetBuildVersion(new Version(1, 1, 2, 2));

            UpdateNetCoreVersionTask task = new UpdateNetCoreVersionTask(_path.Object, _file.Object, "nonext.json")
                .AdditionalProp("dep.test");

            TaskExecutionException e = Assert.Throws<TaskExecutionException>(() => task.ExecuteVoid(Context));

            Assert.Equal(1, task.AdditionalProperties.Count);
            Assert.Equal(1, e.ErrorCode);
        }

        [Fact]
        public void MissingFileWithProps()
        {
            Context.SetBuildVersion(new Version(1, 1, 2, 2));

            UpdateNetCoreVersionTask task = new UpdateNetCoreVersionTask(_path.Object, _file.Object, "nonext.json")
                .AdditionalProp("dep.test", "dep.test1", "dep.test2");

            TaskExecutionException e = Assert.Throws<TaskExecutionException>(() => task.ExecuteVoid(Context));

            Assert.Equal(3, task.AdditionalProperties.Count);
            Assert.Equal(1, e.ErrorCode);
        }
    }
}
