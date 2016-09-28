using System.Collections.Generic;
using FlubuCore.Context;
using FlubuCore.IO;

namespace FlubuCore.Packaging
{
    public class ZipProcessor : IPackageProcessor
    {
        private readonly ITaskContext _taskContext;

        private readonly IZipper _zipper;

        private readonly FileFullPath _zipFileName;

        private readonly FullPath _baseDir;

        private readonly int? _compressionLevel;

        private List<string> _sourcesToZip = new List<string>();

        private IFileFilter _filter;

        public ZipProcessor(
            ITaskContext taskContext,
            IZipper zipper,
            FileFullPath zipFileName,
            FullPath baseDir,
            int? compressionLevel,
            params string[] sources)
        {
            _taskContext = taskContext;
            _zipper = zipper;
            _zipFileName = zipFileName;
            _baseDir = baseDir;
            _compressionLevel = compressionLevel;
            _sourcesToZip.AddRange(sources);
        }

        public IPackageDef Process(IPackageDef packageDef)
        {
            StandardPackageDef zippedPackageDef = new StandardPackageDef();
            List<FileFullPath> filesToZip = new List<FileFullPath>();

            foreach (IFilesSource childSource in packageDef.ListChildSources())
            {
                if (_sourcesToZip.Contains(childSource.Id))
                {
                    foreach (PackagedFileInfo file in childSource.ListFiles())
                    {
                        if (!LoggingHelper.LogIfFilteredOut(file.FileFullPath.ToString(), _filter, _taskContext))
                        {
                            continue;
                        }

                        _taskContext.WriteMessage(string.Format("Adding file '{0}' to zip package", file.FileFullPath));
                        filesToZip.Add(file.FileFullPath);
                    }
                }
            }

            _zipper.ZipFiles(_zipFileName, _baseDir, filesToZip);

            SingleFileSource singleFileSource = new SingleFileSource("zip", _zipFileName);
            zippedPackageDef.AddFilesSource(singleFileSource);

            return zippedPackageDef;
        }

        public void SetFilter(IFileFilter filter)
        {
            _filter = filter;
        }
    }
}