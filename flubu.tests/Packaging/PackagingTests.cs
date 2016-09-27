using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using Flubu.Context;
using Flubu.IO;
using Flubu.Packaging;
using Flubu.Tests.Tasks;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Flubu.Tests.Packaging
{
    [Collection(nameof(TaskTestCollection))]
    public class PackagingTests : TaskTestBase
    {
        private readonly TaskTestFixture _fixture;

        public PackagingTests(TaskTestFixture fixture)
            : base(fixture.LoggerFactory)
        {
            _fixture = fixture;
            if (Directory.Exists("tmp"))
            {
                Directory.Delete("tmp", true);
            }
        }

        [Fact]
        public void BasePackagingWithCopyProcessorAndZipProcessorTest()
        {
            Directory.CreateDirectory("tmp");
            Directory.CreateDirectory("tmp\\Test");
            using (File.Create("tmp\\Test\\test.txt"))
            {
            }

            Directory.CreateDirectory("tmp\\Test2");
            using (File.Create("tmp\\Test2\\test2.txt"))
            {
            }

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

            ZipProcessor zipProcessor = new ZipProcessor(Context, zipper, new FileFullPath("tmp\\test.zip"), new FullPath("tmp\\output"), null, "test", "test2");
            zipProcessor.Process(copiedPackageDef);

            using (ZipArchive archive = ZipFile.OpenRead("tmp\\test.zip"))
            {
                Assert.Equal(2, archive.Entries.Count);
                Assert.Equal("test\\test.txt", archive.Entries[0].FullName);
                Assert.Equal("test2\\test2.txt", archive.Entries[1].FullName);
            }
        }
    }
}