using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using FlubuCore.Packaging;
using FlubuCore.Tasks.Packaging;
using Xunit;

namespace Flubu.Tests.Tasks
{
    [Collection(nameof(TaskTestCollection))]
    public class PackageTaskTests : TaskTestBase
    {
        private readonly TaskTestFixture _fixture;

        public PackageTaskTests(TaskTestFixture fixture)
            : base(fixture.LoggerFactory)
        {
            _fixture = fixture;
            if (Directory.Exists("tmp"))
            {
                Directory.Delete("tmp", true);
            }
        }

        [Fact]
        public void PackagingWihoutFiltersTest()
        {
            Directory.CreateDirectory("tmp");
            Directory.CreateDirectory(@"tmp\Test");
            using (File.Create(@"tmp\Test\test.txt"))
            {
            }

            using (File.Create(@"tmp\Test\test1.txt"))
            {
            }

            Directory.CreateDirectory(@"tmp\Test2");
            using (File.Create(@"tmp\Test2\test2.txt"))
            {
            }

            new PackageTask(@"tmp\output", @"tmp\test.zip")
                .AddDirectoryToPackage("test", @"tmp\test", "test")
                .AddDirectoryToPackage("test2", @"tmp\test2", "test2")
                .Execute(Context);

            using (ZipArchive archive = ZipFile.OpenRead("tmp\\test.zip"))
            {
                Assert.Equal(3, archive.Entries.Count);
                Assert.Equal(@"test\test.txt", archive.Entries[0].FullName);
                Assert.Equal(@"test\test1.txt", archive.Entries[0].FullName);
                Assert.Equal(@"test2\test2.txt", archive.Entries[1].FullName);
            }
        }

        [Fact]
        public void PackagingWithFiltersTest()
        {
            Directory.CreateDirectory("tmp");
            Directory.CreateDirectory(@"tmp\Test");
            using (File.Create(@"tmp\Test\test.txt"))
            {
            }

            using (File.Create(@"tmp\Test\some.fln"))
            {
            }

            Directory.CreateDirectory(@"tmp\Test2");
            using (File.Create(@"tmp\Test2\test2.txt"))
            {
            }

            using (File.Create(@"tmp\Test2\fas.bl"))
            {
            }

            new PackageTask(@"tmp\output", @"tmp\test.zip")
                .AddDirectoryToPackage("test", @"tmp\test", "test", new RegexFileFilter(@".fln"))
                .AddDirectoryToPackage("test2", @"tmp\test2", "test2", new RegexFileFilter(@".bl"))
                .Execute(Context);

            using (ZipArchive archive = ZipFile.OpenRead("tmp\\test.zip"))
            {
                Assert.Equal(2, archive.Entries.Count);
                Assert.Equal(@"test\test.txt", archive.Entries[0].FullName);
                Assert.Equal(@"test2\test2.txt", archive.Entries[1].FullName);
            }
        }
    }
}
