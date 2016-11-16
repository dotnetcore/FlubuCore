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
    [Collection(nameof(FlubuTestCollection))]
    public class PackageTaskTests : FlubuTestBase
    {
        private readonly FlubuTestFixture _fixture;

        public PackageTaskTests(FlubuTestFixture fixture)
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

            new PackageTask(@"tmp\output")
                .AddDirectoryToPackage("test", @"tmp\test", "test")
                .AddDirectoryToPackage("test2", @"tmp\test2", "test2")
                .ZipPackage(@"tmp\test.zip")
                .ExecuteVoid(Context);

            using (ZipArchive archive = ZipFile.OpenRead("tmp\\test.zip"))
            {
                Assert.Equal(3, archive.Entries.Count);
                Assert.Equal(@"test\test.txt", archive.Entries[0].FullName);
                Assert.Equal(@"test\test1.txt", archive.Entries[1].FullName);
                Assert.Equal(@"test2\test2.txt", archive.Entries[2].FullName);
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

            new PackageTask(@"tmp\output")
                .AddDirectoryToPackage("test", @"tmp\test", "test", false, new RegexFileFilter(@".fln"))
                .AddDirectoryToPackage("test2", @"tmp\test2", "test2", false, new RegexFileFilter(@".bl"))
                .ZipPackage(@"tmp\test.zip")
                .ExecuteVoid(Context);

            using (ZipArchive archive = ZipFile.OpenRead("tmp\\test.zip"))
            {
                Assert.Equal(2, archive.Entries.Count);
                Assert.Equal(@"test\test.txt", archive.Entries[0].FullName);
                Assert.Equal(@"test2\test2.txt", archive.Entries[1].FullName);
            }
        }

        [Fact]
        public void PackagingWihoutZippingTest()
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

            new PackageTask(@"tmp\output")
                .AddDirectoryToPackage("test", @"tmp\test", "test")
                .AddDirectoryToPackage("test2", @"tmp\test2", "test2")
                .ExecuteVoid(Context);

            Assert.True(File.Exists(@"tmp\output\test\test.txt"));
            Assert.True(File.Exists(@"tmp\output\test2\test2.txt"));
        }

        [Fact]
        public void PackagingWithAddFileToPackageTest()
        {
            Directory.CreateDirectory("tmp");
            Directory.CreateDirectory(@"tmp\Test");
            using (File.Create(@"tmp\Test\test.txt"))
            {
            }

            Directory.CreateDirectory(@"tmp\Test2");
            using (File.Create(@"tmp\Test2\test2.txt"))
            {
            }

            new PackageTask(@"tmp\output")
                .AddDirectoryToPackage("test", @"tmp\test", "test")
                .AddFileToPackage("test2", @"tmp\Test2\test2.txt", @"test")
                .ExecuteVoid(Context);

            Assert.True(File.Exists(@"tmp\output\test\test.txt"));
            Assert.True(File.Exists(@"tmp\output\test\test2.txt"));
        }
    }
}
