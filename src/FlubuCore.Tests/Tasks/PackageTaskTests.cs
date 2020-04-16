using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using FlubuCore.Context;
using FlubuCore.Packaging;
using FlubuCore.Packaging.Filters;
using FlubuCore.Tasks.Packaging;
using FlubuCore.Tasks.Versioning;
using Xunit;

namespace FlubuCore.Tests.Tasks
{
    [Collection(nameof(FlubuTestCollection))]
    public class PackageTaskTests : FlubuTestBase
    {
         private static char _seperator = Path.DirectorySeparatorChar;

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
        public void PackagingAddFileToPackageTest()
        {
            Directory.CreateDirectory("tmp");
            Directory.CreateDirectory(@"tmp/Test");
            using (File.Create(@"tmp/Test/test.txt"))
            {
            }

            using (File.Create(@"tmp/Test/test1.txt"))
            {
            }

            Context.SetBuildVersion(new BuildVersion() { Version = new Version(1, 0, 0, 0) });

            new PackageTask(@"tmp/output")
                .AddFileToPackage(@"tmp/Test/test1.txt", "test")
                .AddFileToPackage(@"tmp/Test/test1.txt", string.Empty)
                .ZipPackage(@"test.zip", true, 4)
                .ExecuteVoid(Context);

            using (ZipArchive archive = ZipFile.OpenRead("tmp/output/test_1.0.0.0.zip"))
            {
                Assert.Equal(2, archive.Entries.Count);
                Assert.Equal($"test1.txt", archive.Entries[1].FullName);
                Assert.Equal($"test{_seperator}test1.txt", archive.Entries[0].FullName);
            }
        }

        [Fact]
        public void PackagingWihoutFiltersTest()
        {
            Directory.CreateDirectory("tmp");
            Directory.CreateDirectory(@"tmp/Test");
            using (File.Create(@"tmp/Test/test.txt"))
            {
            }

            using (File.Create(@"tmp/Test/test1.txt"))
            {
            }

            Directory.CreateDirectory(@"tmp/Test2");
            using (File.Create(@"tmp/Test2/test2.txt"))
            {
            }

            Context.SetBuildVersion(new BuildVersion() { Version = new Version(1, 0, 0, 0) });

            new PackageTask(@"tmp/output")
                .AddDirectoryToPackage(@"tmp/Test", "test")
                .AddDirectoryToPackage(@"tmp/Test2", "test2")
                .ZipPackage(@"test.zip", true, 4)
                .ExecuteVoid(Context);

            using (ZipArchive archive = ZipFile.OpenRead("tmp/output/test_1.0.0.0.zip"))
            {
                Assert.Equal(3, archive.Entries.Count);
                var list = archive.Entries.ToList<ZipArchiveEntry>();
                Assert.Contains(list, x => x.FullName == $"test{_seperator}test.txt");
                Assert.Contains(list, x => x.FullName == $"test{_seperator}test1.txt");
                Assert.Contains(list, x => x.FullName == $"test2{_seperator}test2.txt");
            }
        }

        [Fact]
        public void PackagingNoFilesToZipTest()
        {
            Directory.CreateDirectory("tmp");
            Directory.CreateDirectory(@"tmp/Test");
            using (File.Create(@"tmp/Test/test.txt"))
            {
            }

            Context.SetBuildVersion(new BuildVersion() { Version = new Version(1, 0, 0, 0) });

            new PackageTask(@"tmp/output")
                .AddDirectoryToPackage(@"tmp", "test", o =>
                {
                })
                .ZipPackage(@"test.zip", true, 4)
                .ExecuteVoid(Context);

            Assert.False(File.Exists("tmp/output/test_1.0.0.0.zip"));
        }

        [Fact]
        public void PackagingWithRegexFileFiltersTest()
        {
            Directory.CreateDirectory("tmp");
            Directory.CreateDirectory(@"tmp/Test");
            using (File.Create(@"tmp/Test/test.txt"))
            {
            }

            using (File.Create(@"tmp/Test/some.fln"))
            {
            }

            Directory.CreateDirectory(@"tmp/Test2");
            using (File.Create(@"tmp/Test2/test2.txt"))
            {
            }

            using (File.Create(@"tmp/Test2/fas.bl"))
            {
            }

            new PackageTask(@"tmp/output")
                .AddDirectoryToPackage(@"tmp/Test", "test", false, new RegexFilter(@".fln"))
                .AddDirectoryToPackage(@"tmp/Test2", "test2", false, new RegexFilter(@".bl"))
                .ZipPackage(@"test.zip", false)
                .ExecuteVoid(Context);

            using (ZipArchive archive = ZipFile.OpenRead("tmp/output/test.zip"))
            {
                Assert.Equal(2, archive.Entries.Count);
                Assert.Equal($"test{_seperator}test.txt", archive.Entries[0].FullName);
                Assert.Equal($"test2{_seperator}test2.txt", archive.Entries[1].FullName);
            }
        }

        [Fact]
        public void PackagingWithGlobFileFiltersTest()
        {
            Directory.CreateDirectory("tmp");
            Directory.CreateDirectory(@"tmp/Test");
            using (File.Create(@"tmp/Test/test.txt"))
            {
            }

            using (File.Create(@"tmp/Test/some.fln"))
            {
            }

            Directory.CreateDirectory(@"tmp/Test2");
            using (File.Create(@"tmp/Test2/test2.txt"))
            {
            }

            using (File.Create(@"tmp/Test2/fas.bl"))
            {
            }

            new PackageTask(@"tmp/output")
                .AddDirectoryToPackage(@"tmp/Test", "test", false, new GlobFilter(@"**\*.fln"))
                .AddDirectoryToPackage(@"tmp/Test2", "test2", false, new GlobFilter(@"**\*.bl"))
                .ZipPackage(@"test.zip", false)
                .ExecuteVoid(Context);

            using (ZipArchive archive = ZipFile.OpenRead("tmp/output/test.zip"))
            {
                Assert.Equal(2, archive.Entries.Count);
                Assert.Equal($"test{_seperator}test.txt", archive.Entries[0].FullName);
                Assert.Equal($"test2{_seperator}test2.txt", archive.Entries[1].FullName);
            }
        }

        [Fact]
        [Trait("Category", "OnlyWindows")]
        public void PackagingWithGlobDirectoryFiltersTest()
        {
            Directory.CreateDirectory("tmp");
            Directory.CreateDirectory(@"tmp/Test");
            using (File.Create(@"tmp/Test/test.txt"))
            {
            }

            using (File.Create(@"tmp/Test/some.fln"))
            {
            }

            Directory.CreateDirectory(@"tmp/Test2");
            using (File.Create(@"tmp/Test2/test2.txt"))
            {
            }

            using (File.Create(@"tmp/Test2/fas.bl"))
            {
            }

            new PackageTask(@"tmp/output")
                .AddDirectoryToPackage(@"tmp", "test", options =>
                {
                    options.Recursive = true;
                    options.AddDirectoryFilterGlob("**/Test");
                })
                .ZipPackage(@"test.zip", false)
                .ExecuteVoid(Context);

            using (ZipArchive archive = ZipFile.OpenRead("tmp/output/test.zip"))
            {
                var enties = archive.Entries.OrderBy(x => x.FullName).ToList();
                Assert.Equal(2, archive.Entries.Count);
                Assert.Equal($"test{_seperator}Test2{_seperator}fas.bl", enties[0].FullName);
                Assert.Equal($"test{_seperator}Test2{_seperator}test2.txt", enties[1].FullName);
            }
        }

        [Fact]
        public void PackagingWithDoubleDotsInZipFileNameTest()
        {
            Directory.CreateDirectory("tmp");
            Directory.CreateDirectory(@"tmp/Test");
            using (File.Create(@"tmp/Test/test.txt"))
            {
            }

            new PackageTask(@"tmp/output")
                .AddDirectoryToPackage(@"tmp/Test", "test")
                .ZipPackage(@"test.flubu.zip", false)
                .ExecuteVoid(Context);

            using (ZipArchive archive = ZipFile.OpenRead("tmp/output/test.flubu.zip"))
            {
                Assert.Single(archive.Entries);
            }
        }

        [Fact]
        public void PackagingWithDoubleDotsInZipFileNameNoZipExtensionTest()
        {
            Directory.CreateDirectory("tmp");
            Directory.CreateDirectory(@"tmp/Test");
            using (File.Create(@"tmp/Test/test.txt"))
            {
            }

            new PackageTask(@"tmp/output")
                .AddDirectoryToPackage(@"tmp/Test", "test")
                .ZipPackage(@"test.flubu", false)
                .ExecuteVoid(Context);

            using (ZipArchive archive = ZipFile.OpenRead("tmp/output/test.flubu.zip"))
            {
                Assert.Single(archive.Entries);
            }
        }

        [Fact]
        public void PackagingWihoutZippingTest()
        {
            Directory.CreateDirectory("tmp");
            Directory.CreateDirectory(@"tmp/Test");
            using (File.Create(@"tmp/Test/test.txt"))
            {
            }

            using (File.Create(@"tmp/Test/test1.txt"))
            {
            }

            Directory.CreateDirectory(@"tmp/Test2");
            using (File.Create(@"tmp/Test2/test2.txt"))
            {
            }

            new PackageTask(@"tmp/output")
                .AddDirectoryToPackage(@"tmp/Test", "test")
                .AddDirectoryToPackage(@"tmp/Test2", "test2")
                .ExecuteVoid(Context);

            Assert.True(File.Exists(@"tmp/output/test/test.txt"));
            Assert.True(File.Exists(@"tmp/output/test2/test2.txt"));
        }

        [Fact]
        public void PackagingWithAddFileToPackageTest()
        {
            Directory.CreateDirectory("tmp");
            Directory.CreateDirectory(@"tmp/Test");
            using (File.Create(@"tmp/Test/test.txt"))
            {
            }

            Directory.CreateDirectory(@"tmp/Test2");
            using (File.Create(@"tmp/Test2/test2.txt"))
            {
            }

            new PackageTask(@"tmp/output")
                .AddDirectoryToPackage(@"tmp/Test", "test")
                .AddFileToPackage(@"tmp/Test2/test2.txt", @"test")
                .ExecuteVoid(Context);

            Assert.True(File.Exists(@"tmp/output/test/test.txt"));
            Assert.True(File.Exists(@"tmp/output/test/test2.txt"));
        }
    }
}
