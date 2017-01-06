using System.IO;
using System.IO.Compression;
using FlubuCore.IO;
using FlubuCore.Packaging;
using FlubuCore.Tasks.Packaging;
using Xunit;

namespace Flubu.Tests.Packaging
{
    [Collection(nameof(FlubuTestCollection))]
    public class PackagingTests : FlubuTestBase
    {
        private readonly FlubuTestFixture _fixture;

        public PackagingTests(FlubuTestFixture fixture)
            : base(fixture.LoggerFactory)
        {
            _fixture = fixture;
            if (Directory.Exists("tmp"))
            {
                Directory.Delete("tmp", true);
            }

            Directory.CreateDirectory("tmp");
            Directory.CreateDirectory("tmp\\Test");
            Directory.CreateDirectory("tmp\\Test2");
            Directory.CreateDirectory("tmp\\Test3");

            CreateTestFile("tmp\\Test\\test.txt", "test.txt");
            CreateTestFile("tmp\\Test2\\test2.txt", "test.txt");
            CreateTestFile("tmp\\Test2\\test.txt", "test.txt");
            CreateTestFile("tmp\\Test\\test2.txt", "test2.txt");
        }

        [Fact]
        public void BasePackagingWithCopyProcessorAndZipProcessorTest()
        {
            ICopier copier = new Copier(Context);
            IZipper zipper = new Zipper(Context);
            IDirectoryFilesLister directoryFilesLister = new DirectoryFilesLister();
            StandardPackageDef packageDef = new StandardPackageDef();
            DirectorySource test = new DirectorySource(Context, directoryFilesLister, "test", new FullPath("tmp\\test"));
            DirectorySource test2 = new DirectorySource(Context, directoryFilesLister, "test2", new FullPath("tmp\\test2"));
            packageDef.AddFilesSource(test);
            packageDef.AddFilesSource(test2);
            CopyProcessor copyProcessor = new CopyProcessor(
               Context,
               copier,
               new FullPath("tmp\\output"));
            copyProcessor
                .AddTransformation("test", new LocalPath(@"test"))
                .AddTransformation("test2", new LocalPath(@"test2"));
            IPackageDef copiedPackageDef = copyProcessor.Process(packageDef);

            ZipProcessor zipProcessor = new ZipProcessor(Context, zipper, new FileFullPath("tmp\\test.zip"), new FullPath("tmp\\output"), false, null, "test", "test2");
            zipProcessor.Process(copiedPackageDef);

            using (ZipArchive archive = ZipFile.OpenRead("tmp\\test.zip"))
            {
                Assert.Equal(4, archive.Entries.Count);
                Assert.Equal("test\\test.txt", archive.Entries[0].FullName);
                Assert.Equal("test\\test2.txt", archive.Entries[1].FullName);
                Assert.Equal("test2\\test.txt", archive.Entries[2].FullName);
                Assert.Equal("test2\\test2.txt", archive.Entries[3].FullName);
            }
        }

        [Fact]
        public void OptimizeZipTest()
        {
            ICopier copier = new Copier(Context);
            IZipper zipper = new Zipper(Context);
            IDirectoryFilesLister directoryFilesLister = new DirectoryFilesLister();
            StandardPackageDef packageDef = new StandardPackageDef();
            DirectorySource test = new DirectorySource(Context, directoryFilesLister, "test", new FullPath("tmp\\test"));
            DirectorySource test2 = new DirectorySource(Context, directoryFilesLister, "test2", new FullPath("tmp\\test2"));
            packageDef.AddFilesSource(test);
            packageDef.AddFilesSource(test2);
            CopyProcessor copyProcessor = new CopyProcessor(
               Context,
               copier,
               new FullPath("tmp\\output"));
            copyProcessor
                .AddTransformation("test", new LocalPath(@"test"))
                .AddTransformation("test2", new LocalPath(@"test2"));
            IPackageDef copiedPackageDef = copyProcessor.Process(packageDef);

            ZipProcessor zipProcessor = new ZipProcessor(Context, zipper, new FileFullPath("tmp\\test.zip"), new FullPath("tmp\\output"), true, null, "test", "test2");
            zipProcessor.Process(copiedPackageDef);

            using (ZipArchive archive = ZipFile.OpenRead("tmp\\test.zip"))
            {
                Assert.Equal(4, archive.Entries.Count);
                Assert.Equal("test2\\test.txt", archive.Entries[1].FullName);
                Assert.Equal("test2\\test2.txt", archive.Entries[0].FullName);
                Assert.Equal("test\\test2.txt", archive.Entries[2].FullName);
                Assert.Equal("_zipmetadata.json", archive.Entries[3].FullName);
            }
        }

        [Fact]
        public void OptimizeZipThenUnzipTest()
        {
            ICopier copier = new Copier(Context);
            IZipper zipper = new Zipper(Context);
            IDirectoryFilesLister directoryFilesLister = new DirectoryFilesLister();
            StandardPackageDef packageDef = new StandardPackageDef();
            DirectorySource test = new DirectorySource(Context, directoryFilesLister, "test", new FullPath("tmp\\test"));
            DirectorySource test2 = new DirectorySource(Context, directoryFilesLister, "test2", new FullPath("tmp\\test2"));
            packageDef.AddFilesSource(test);
            packageDef.AddFilesSource(test2);
            CopyProcessor copyProcessor = new CopyProcessor(
               Context,
               copier,
               new FullPath("tmp\\output"));
            copyProcessor
                .AddTransformation("test", new LocalPath(@"test"))
                .AddTransformation("test2", new LocalPath(@"test2"));
            IPackageDef copiedPackageDef = copyProcessor.Process(packageDef);

            ZipProcessor zipProcessor = new ZipProcessor(Context, zipper, new FileFullPath("tmp\\test.zip"), new FullPath("tmp\\output"), true, null, "test", "test2");
            zipProcessor.Process(copiedPackageDef);
            string zf = "tmp\\test.zip";
            using (ZipArchive archive = ZipFile.OpenRead(zf))
            {
                Assert.Equal(4, archive.Entries.Count);
                Assert.Equal("test2\\test.txt", archive.Entries[1].FullName);
                Assert.Equal("test2\\test2.txt", archive.Entries[0].FullName);
                Assert.Equal("test\\test2.txt", archive.Entries[2].FullName);
                Assert.Equal("_zipmetadata.json", archive.Entries[3].FullName);
            }

            string unzipPath = "tmp/tt/";
            UnzipTask unzip = new UnzipTask(zf, unzipPath);
            unzip.Execute(Context);

            CheckTestFile(Path.Combine(unzipPath, "test2\\test.txt"), "test.txt\r\n");
            CheckTestFile(Path.Combine(unzipPath, "test2\\test2.txt"), "test.txt\r\n");
            CheckTestFile(Path.Combine(unzipPath, "test\\test.txt"), "test.txt\r\n");
            CheckTestFile(Path.Combine(unzipPath, "test\\test2.txt"), "test2.txt\r\n");
        }

        private static void CreateTestFile(string fileName, string data)
        {
            using (var s = File.Create(fileName))
            using (StreamWriter w = new StreamWriter(s))
            {
                w.WriteLine(data);
                w.Flush();
            }
        }

        private static void CheckTestFile(string fileName, string expectedData)
        {
            var actual = File.ReadAllText(fileName);
            Assert.Equal(expectedData, actual);
        }
    }
}