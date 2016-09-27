using System.Collections.Generic;

using Flubu.IO;

namespace Flubu.Packaging
{
    public interface IZipper
    {
        void ZipFiles(
            FileFullPath zipFileName,
            FullPath baseDir,
            IEnumerable<FileFullPath> filesToZip);
    }
}