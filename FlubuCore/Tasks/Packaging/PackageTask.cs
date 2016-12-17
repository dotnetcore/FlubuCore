using System;
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
        private bool _versionPostFix;
        private int _versionFieldCount;
        private string _zipPrefix;

        public PackageTask(string destinationRootDir = null)
        {
            if (!string.IsNullOrEmpty(destinationRootDir))
                _destinationRootDir = destinationRootDir;

            _sourcePackagingInfos = new List<SourcePackagingInfo>();
        }

        private bool ShouldPackageBeZipped => !string.IsNullOrEmpty(_zipFileName) || !string.IsNullOrEmpty(_zipPrefix);

        [Obsolete("Use overload without sourceId parameter instead.", false)]
        public PackageTask AddDirectoryToPackage(string sourceId, string sourceDirectoryPath, string destinationDirectory, bool recursive = false)
        {
            SourcePackagingInfo directoryToPackage = new SourcePackagingInfo(
                SourceType.Directory,
                sourceDirectoryPath,
                destinationDirectory) { Recursive = recursive };

            _sourcePackagingInfos.Add(directoryToPackage);
            return this;
        }

        public PackageTask AddDirectoryToPackage(string sourceDirectoryPath, string destinationDirectory, bool recursive = false)
        {
            SourcePackagingInfo directoryToPackage = new SourcePackagingInfo(
                SourceType.Directory,
                sourceDirectoryPath,
                destinationDirectory)
            { Recursive = recursive };

            _sourcePackagingInfos.Add(directoryToPackage);
            return this;
        }

        [Obsolete("Use overload without sourceId parameter instead.", false)]
        public PackageTask AddDirectoryToPackage(string sourceId, string sourceDirectoryPath, string destinationDirectory, bool recursive, params IFileFilter[] fileFilters)
        {
            SourcePackagingInfo directoryToPackage = new SourcePackagingInfo(
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

        public PackageTask AddDirectoryToPackage(string sourceDirectoryPath, string destinationDirectory, bool recursive, params IFileFilter[] fileFilters)
        {
            SourcePackagingInfo directoryToPackage = new SourcePackagingInfo(
                SourceType.Directory,
                sourceDirectoryPath,
                destinationDirectory)
            { Recursive = recursive };

            foreach (var filter in fileFilters)
            {
                directoryToPackage.FileFilters.Add(filter);
            }

            _sourcePackagingInfos.Add(directoryToPackage);
            return this;
        }

        [Obsolete("Use overload without sourceId parameter instead.", false)]
        public PackageTask AddFileToPackage(string sourceId, string sourceFilePath, string destinationDirectory)
        {
            _sourcePackagingInfos.Add(new SourcePackagingInfo(SourceType.File, sourceFilePath, destinationDirectory));
            return this;
        }

        public PackageTask AddFileToPackage(string sourceFilePath, string destinationDirectory)
        {
            _sourcePackagingInfos.Add(new SourcePackagingInfo(SourceType.File, sourceFilePath, destinationDirectory));
            return this;
        }

        public PackageTask ZipPackage(string zipFileName, bool addVersionPostfix = true, int versionFeildCount = 3)
        {
            _zipFileName = zipFileName;
            _versionPostFix = addVersionPostfix;
            _versionFieldCount = versionFeildCount;
            return this;
        }

        [Obsolete("Use ZipPackage with AddVersionPostFix parameter set to true instead.")]
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
                _destinationRootDir = context.Properties.GetOutputDir();

            FullPath df = new FullPath(_destinationRootDir);
            ICopier copier = new Copier(context);
            IZipper zipper = new Zipper(context);
            IDirectoryFilesLister directoryFilesLister = new DirectoryFilesLister();
            StandardPackageDef packageDef = new StandardPackageDef();

            CopyProcessor copyProcessor = new CopyProcessor(context, copier, df);

            List<string> sourceIds = new List<string>();

            foreach (var sourceToPackage in _sourcePackagingInfos)
            {
                string sourceId;
                if (sourceToPackage.SourceType == SourceType.Directory)
                {
                    var sourceFullPath = new FullPath(sourceToPackage.SourcePath);
                    sourceId = sourceFullPath.GetHashCode().ToString();
                    DirectorySource directorySource = new DirectorySource(context, directoryFilesLister, sourceId, sourceFullPath, sourceToPackage.Recursive);
                    directorySource.SetFilter(sourceToPackage.FileFilters);
                    packageDef.AddFilesSource(directorySource);
                }
                else
                {
                    var fileFullPath = new FileFullPath(sourceToPackage.SourcePath);
                    sourceId = fileFullPath.GetHashCode().ToString();
                    SingleFileSource fileSource = new SingleFileSource(sourceId, fileFullPath);
                    packageDef.AddFilesSource(fileSource);
                }

                copyProcessor.AddTransformation(sourceId, sourceToPackage.DestinationPath);
                sourceIds.Add(sourceId);
            }

            IPackageDef copiedPackageDef = copyProcessor.Process(packageDef);

            if (ShouldPackageBeZipped)
            {
                string zipFile = _zipFileName;

                if (string.IsNullOrEmpty(zipFile))
                     zipFile = _zipPrefix;

                zipFile = Path.GetFileNameWithoutExtension(zipFile);

                if (_versionPostFix)
                {
                    zipFile = Path.Combine(_destinationRootDir, $"{zipFile}_{context.Properties.GetBuildVersion().ToString(_versionFieldCount)}.zip");
                }
                else
                {
                    zipFile = Path.Combine(_destinationRootDir, $"{zipFile}.zip");
                }

                ZipProcessor zipProcessor = new ZipProcessor(context, zipper, new FileFullPath(zipFile), df, sourceIds);
                zipProcessor.Process(copiedPackageDef);
            }

            return 0;
        }
    }
}
