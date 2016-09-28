using System.Collections.Generic;
using System.IO.Compression;
using FlubuCore.Context;
using FlubuCore.IO;

namespace FlubuCore.Packaging
{
    public class Zipper : IZipper
    {
        private readonly ITaskContext _taskContext;

        public Zipper(ITaskContext taskContext)
        {
            _taskContext = taskContext;
        }

        public void ZipFiles(
            FileFullPath zipFileName,
            FullPath baseDir,
            IEnumerable<FileFullPath> filesToZip)
        {
            _taskContext.WriteMessage(string.Format("Zipping {0}", zipFileName));

            using (ZipArchive newFile = ZipFile.Open(zipFileName.ToFullPath(), ZipArchiveMode.Create))
            {
                foreach (var fileToZip in filesToZip)
                {
                    LocalPath debasedFileName = fileToZip.ToFullPath().DebasePath(baseDir);
                    newFile.CreateEntryFromFile(fileToZip.ToString(), debasedFileName, CompressionLevel.Optimal);
                }
            }
        }
    }
}