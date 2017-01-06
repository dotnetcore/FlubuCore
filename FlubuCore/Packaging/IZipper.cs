using System.Collections.Generic;
using FlubuCore.IO;

namespace FlubuCore.Packaging
{
    public interface IZipper
    {
        void ZipFiles(
            FileFullPath zipFileName,
            FullPath baseDir,
            IEnumerable<FileFullPath> filesToZip,
            bool optimizeFiles);
    }
}