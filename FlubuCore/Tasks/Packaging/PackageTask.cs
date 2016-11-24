using System.Collections.Generic;
using System.IO;
using FlubuCore.Context;
using FlubuCore.IO;
using FlubuCore.Packaging;

namespace FlubuCore.Tasks.Packaging
{
    public class PackageTask : TaskBase<int>
    {
        private readonly List<SourcePackagingInfo> _sourcePackagingInfos;

        private string _destinationRootDir;

        private string _zipFileName;
        private string _zipPrefix;

        public PackageTask(string destinationRootDir = null)
        {
            _destinationRootDir = destinationRootDir;
            _sourcePackagingInfos = new List<SourcePackagingInfo>();
        }

        private bool ShouldPackageBeZipped => !string.IsNullOrEmpty(_zipFileName) || !string.IsNullOrEmpty(_zipPrefix);

        public PackageTask AddDirectoryToPackage(string sourceId, string sourceDirectoryPath, string destinationDirectory, bool recursive = false)
        {
            SourcePackagingInfo directoryToPackage = new SourcePackagingInfo(
                sourceId,
                SourceType.Directory,
                sourceDirectoryPath,
                destinationDirectory) { Recursive = recursive };

            _sourcePackagingInfos.Add(directoryToPackage);
            return this;
        }

        public PackageTask AddDirectoryToPackage(string sourceId, string sourceDirectoryPath, string destinationDirectory, bool recursive, params IFileFilter[] fileFilters)
        {
            SourcePackagingInfo directoryToPackage = new SourcePackagingInfo(
                sourceId,
                SourceType.Directory,
                sourceDirectoryPath,
                destinationDirectory) { Recursive = recursive };

            foreach (var filter in fileFilters)
            {
                directoryToPackage.FileFilters.Add(filter);
            }

            _sourcePackagingInfos.Add(directoryToPackage);
            return this;
        }

        public PackageTask AddFileToPackage(string sourceId, string sourceFilePath, string destinationDirectory)
        {
            _sourcePackagingInfos.Add(new SourcePackagingInfo(sourceId, SourceType.File, sourceFilePath, destinationDirectory));
            return this;
        }

        public PackageTask ZipPackage(string fullZipFile)
        {
            _zipFileName = fullZipFile;
            return this;
        }

        public PackageTask ZipPrefix(string zipPrefix)
        {
            _zipPrefix = zipPrefix;
            return this;
        }

        protected override int DoExecute(ITaskContextInternal context)
        {
            if (_sourcePackagingInfos.Count == 0)
            {
                return 0;
            }

            if (string.IsNullOrEmpty(_destinationRootDir))
                _destinationRootDir = context.GetOutputDir();

            FullPath df = new FullPath(_destinationRootDir);
            ICopier copier = new Copier(context);
            IZipper zipper = new Zipper(context);
            IDirectoryFilesLister directoryFilesLister = new DirectoryFilesLister();
            StandardPackageDef packageDef = new StandardPackageDef();

            CopyProcessor copyProcessor = new CopyProcessor(context, copier, df);

            List<string> sourceIds = new List<string>();

            foreach (var sourceToPackage in _sourcePackagingInfos)
            {
                if (sourceToPackage.SourceType == SourceType.Directory)
                {
                    DirectorySource directorySource = new DirectorySource(context, directoryFilesLister, sourceToPackage.SourceId, new FullPath(sourceToPackage.SourcePath), sourceToPackage.Recursive);
                    directorySource.SetFilter(sourceToPackage.FileFilters);
                    packageDef.AddFilesSource(directorySource);
                }
                else
                {
                    SingleFileSource fileSource = new SingleFileSource(sourceToPackage.SourceId, new FileFullPath(sourceToPackage.SourcePath));
                    packageDef.AddFilesSource(fileSource);
                }

                copyProcessor.AddTransformation(sourceToPackage.SourceId, sourceToPackage.DestinationPath);
                sourceIds.Add(sourceToPackage.SourceId);
            }

            IPackageDef copiedPackageDef = copyProcessor.Process(packageDef);

            if (ShouldPackageBeZipped)
            {
                string zipFile = _zipFileName;

                if (string.IsNullOrEmpty(zipFile))
                    zipFile = Path.Combine(_destinationRootDir, $"{_zipPrefix}_{context.GetBuildVersion().ToString(3)}.zip");

                ZipProcessor zipProcessor = new ZipProcessor(context, zipper, new FileFullPath(zipFile), df, sourceIds);
                zipProcessor.Process(copiedPackageDef);
            }

            return 0;
        }
    }
}
