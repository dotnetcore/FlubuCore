using System.IO;
using System.IO.Compression;
using System.Linq;
using FlubuCore.IO;
using FlubuCore.Packaging;
using FlubuCore.Tasks.Packaging;
using Xunit;

namespace FlubuCore.Tests.Packaging
{
    [Collection(nameof(FlubuTestCollection))]
    public class PackagingTests : FlubuTestBase
    {
        private static char _seperator = Path.DirectorySeparatorChar;

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
            Directory.CreateDirectory("tmp/test");
            Directory.CreateDirectory("tmp/test2");
            Directory.CreateDirectory("tmp/test3");

            CreateTestFile("tmp/test/test.txt", "test.txt");
            CreateTestFile("tmp/test2/test2.txt", "test.txt");
            CreateTestFile("tmp/test2/test.txt", "test.txt");
            CreateTestFile("tmp/test/test2.txt", "test2.txt");
        }

        [Fact]
        public void BasePackagingWithCopyProcessorAndZipProcessorTest()
        {
            ICopier copier = new Copier(Context);
            IZipper zipper = new Zipper(Context);
            IDirectoryFilesLister directoryFilesLister = new DirectoryFilesLister();
            StandardPackageDef packageDef = new StandardPackageDef();
            DirectorySource test = new DirectorySource(Context, directoryFilesLister, "test", new FullPath("tmp/test"));
            DirectorySource test2 = new DirectorySource(Context, directoryFilesLister, "test2", new FullPath("tmp/test2"));
            packageDef.AddFilesSource(test);
            packageDef.AddFilesSource(test2);
            CopyProcessor copyProcessor = new CopyProcessor(
               Context,
               copier,
               new FullPath("tmp/output"));
            copyProcessor
                .AddTransformation("test", new LocalPath(@"test"))
                .AddTransformation("test2", new LocalPath(@"test2"));
            IPackageDef copiedPackageDef = copyProcessor.Process(packageDef);

            ZipProcessor zipProcessor = new ZipProcessor(Context, zipper, new FileFullPath("tmp/test.zip"), new FullPath("tmp/output"), false, null, "test", "test2");
            zipProcessor.Process(copiedPackageDef);

            using (ZipArchive archive = ZipFile.OpenRead("tmp/test.zip"))
            {
                Assert.Equal(4, archive.Entries.Count);
                var list = archive.Entries.ToList<ZipArchiveEntry>();
                Assert.Contains(list, x => x.FullName == $"test{_seperator}test.txt");
                Assert.Contains(list, x => x.FullName == $"test{_seperator}test2.txt");
                Assert.Contains(list, x => x.FullName == $"test2{_seperator}test.txt");
                Assert.Contains(list, x => x.FullName == $"test2{_seperator}test2.txt");
            }
        }

        [Fact]
        public void OptimizeZipTest()
        {
            ICopier copier = new Copier(Context);
            IZipper zipper = new Zipper(Context);
            IDirectoryFilesLister directoryFilesLister = new DirectoryFilesLister();
            StandardPackageDef packageDef = new StandardPackageDef();
            DirectorySource test = new DirectorySource(Context, directoryFilesLister, "test", new FullPath("tmp/test"));
            DirectorySource test2 = new DirectorySource(Context, directoryFilesLister, "test2", new FullPath("tmp/test2"));
            packageDef.AddFilesSource(test);
            packageDef.AddFilesSource(test2);
            CopyProcessor copyProcessor = new CopyProcessor(
               Context,
               copier,
               new FullPath("tmp/output"));
            copyProcessor
                .AddTransformation("test", new LocalPath(@"test"))
                .AddTransformation("test2", new LocalPath(@"test2"));
            IPackageDef copiedPackageDef = copyProcessor.Process(packageDef);

            ZipProcessor zipProcessor = new ZipProcessor(Context, zipper, new FileFullPath("tmp/test.zip"), new FullPath("tmp/output"), true, null, "test", "test2");
            zipProcessor.Process(copiedPackageDef);

            using (ZipArchive archive = ZipFile.OpenRead("tmp/test.zip"))
            {
                Assert.Equal(4, archive.Entries.Count);
                var list = archive.Entries.ToList<ZipArchiveEntry>();

                Assert.Contains(list, x => x.FullName == "_zipmetadata.json");
                Assert.Contains(list, x => x.FullName == $"test{_seperator}test2.txt");
                Assert.Contains(list, x => x.FullName == $"test2{_seperator}test.txt");
                Assert.Contains(list, x => x.FullName == $"test2{_seperator}test2.txt");
            }
        }

        [Fact]
        public void OptimizeZipThenUnzipTest()
        {
            ICopier copier = new Copier(Context);
            IZipper zipper = new Zipper(Context);
            IDirectoryFilesLister directoryFilesLister = new DirectoryFilesLister();
            StandardPackageDef packageDef = new StandardPackageDef();
            DirectorySource test = new DirectorySource(Context, directoryFilesLister, "test", new FullPath("tmp/test"));
            DirectorySource test2 = new DirectorySource(Context, directoryFilesLister, "test2", new FullPath("tmp/test2"));
            packageDef.AddFilesSource(test);
            packageDef.AddFilesSource(test2);
            CopyProcessor copyProcessor = new CopyProcessor(
               Context,
               copier,
               new FullPath("tmp/output"));
            copyProcessor
                .AddTransformation("test", new LocalPath(@"test"))
                .AddTransformation("test2", new LocalPath(@"test2"));
            IPackageDef copiedPackageDef = copyProcessor.Process(packageDef);

            ZipProcessor zipProcessor = new ZipProcessor(Context, zipper, new FileFullPath("tmp/test.zip"), new FullPath("tmp/output"), true, null, "test", "test2");
            zipProcessor.Process(copiedPackageDef);
            string zf = "tmp/test.zip";
            using (ZipArchive archive = ZipFile.OpenRead(zf))
            {
                Assert.Equal(4, archive.Entries.Count);
                var list = archive.Entries.ToList<ZipArchiveEntry>();
                Assert.Contains(list, x => x.FullName == $"test2{_seperator}test.txt");
                Assert.Contains(list, x => x.FullName == $"test2{_seperator}test2.txt");
                Assert.Contains(list, x => x.FullName == $"test{_seperator}test2.txt");
                Assert.Contains(list, x => x.FullName == "_zipmetadata.json");
            }

            string unzipPath = "tmp/tt/";
            UnzipTask unzip = new UnzipTask(zf, unzipPath);
            unzip.Execute(Context);
            var newLine = System.Environment.NewLine;
            CheckTestFile(Path.Combine(unzipPath, $"test2{_seperator}test.txt"), $"test.txt{newLine}");
            CheckTestFile(Path.Combine(unzipPath, $"test2{_seperator}test2.txt"), $"test.txt{newLine}");
            CheckTestFile(Path.Combine(unzipPath, $"test{_seperator}test.txt"), $"test.txt{newLine}");
            CheckTestFile(Path.Combine(unzipPath, $"test{_seperator}test2.txt"), $"test2.txt{newLine}");
        }

        [Fact]
        public void UnzipFileZippedWithExternalZipper()
        {
            new UnzipTask(@"TestData/apiSimulator.zip", "tmp").Execute(Context);
            Assert.True(Directory.Exists(@"tmp/apiSimulator"));
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