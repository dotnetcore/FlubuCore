using System.Collections.Generic;
using FlubuCore.Context;
using FlubuCore.IO;

namespace FlubuCore.Packaging
{
    public class ZipProcessor : IPackageProcessor
    {
        private readonly ITaskContextInternal _taskContext;

        private readonly IZipper _zipper;

        private readonly FileFullPath _zipFileName;

        private readonly FullPath _baseDir;

        private readonly bool _optimizeFiles;

        private readonly List<string> _sourcesToZip = new List<string>();

        private readonly bool _logFiles = true;

        private IFilter _filter;

        public ZipProcessor(
            ITaskContextInternal taskContext,
            IZipper zipper,
            FileFullPath zipFileName,
            FullPath baseDir,
            bool optimizeFiles,
            params string[] sources)
        {
            _taskContext = taskContext;
            _zipper = zipper;
            _zipFileName = zipFileName;
            _baseDir = baseDir;
            _optimizeFiles = optimizeFiles;
            _sourcesToZip.AddRange(sources);
        }

        public ZipProcessor(
          ITaskContextInternal taskContext,
          IZipper zipper,
          FileFullPath zipFileName,
          FullPath baseDir,
          bool optimizeFiles,
          bool logFiles,
          params string[] sources)
        {
            _taskContext = taskContext;
            _zipper = zipper;
            _zipFileName = zipFileName;
            _baseDir = baseDir;
            _optimizeFiles = optimizeFiles;
            _logFiles = logFiles;
            _sourcesToZip.AddRange(sources);
        }

        public ZipProcessor(
           ITaskContextInternal taskContext,
           IZipper zipper,
           FileFullPath zipFileName,
           FullPath baseDir,
           bool optimizeFiles,
           List<string> sources,
           bool logFiles = true)
        {
            _taskContext = taskContext;
            _zipper = zipper;
            _zipFileName = zipFileName;
            _baseDir = baseDir;
            _optimizeFiles = optimizeFiles;
            _sourcesToZip.AddRange(sources);
            _logFiles = logFiles;
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
                        if (!LoggingHelper.LogIfFilteredOut(file.FileFullPath.ToString(), _filter, _taskContext, _logFiles))
                        {
                            continue;
                        }

                        if (_logFiles)
                        {
                            _taskContext.LogInfo($"Adding file '{file.FileFullPath}' to zip package");
                        }

                        filesToZip.Add(file.FileFullPath);
                    }
                }
            }

            if (filesToZip.Count == 0)
            {
                _taskContext.LogInfo("No files to zip! Task skipped zipping.");
                return null;
            }

            _zipper.ZipFiles(_zipFileName, _baseDir, filesToZip, _optimizeFiles);

            SingleFileSource singleFileSource = new SingleFileSource("zip", _zipFileName);
            zippedPackageDef.AddFilesSource(singleFileSource);

            return zippedPackageDef;
        }

        public void SetFileFilter(IFilter filter)
        {
            _filter = filter;
        }
    }
}