using System;
using System.Collections.Generic;
using System.IO;
using FlubuCore.Context;
using FlubuCore.IO;
using FlubuCore.Packaging;
using FlubuCore.Packaging.Filters;

namespace FlubuCore.Tasks.Packaging
{
    public class PackageTask : TaskBase<int, PackageTask>
    {
        private readonly List<SourcePackagingInfo> _sourcePackagingInfos;
        private string _destinationRootDir;
        private string _zipFileName;
        private bool _addVersionAsPostFixToZipFileName;
        private int _versionFieldCount;
        private string _zipPrefix;
        private bool _optimizeZip;

        private bool _logFiles = true;
        private string _description;

        public PackageTask(string destinationRootDir = null)
        {
            if (!string.IsNullOrEmpty(destinationRootDir))
                _destinationRootDir = destinationRootDir;

            _sourcePackagingInfos = new List<SourcePackagingInfo>();
        }

        protected override string Description
        {
            get
            {
                if (string.IsNullOrEmpty(_description))
                {
                    return $"Packages specified files and folders to '{_destinationRootDir}'";
                }

                return _description;
            }

            set { _description = value; }
        }

        private bool ShouldPackageBeZipped => !string.IsNullOrEmpty(_zipFileName) || !string.IsNullOrEmpty(_zipPrefix);

        /// <summary>
        /// Add's specified directory to the package.
        /// </summary>
        /// <param name="sourceDirectoryPath">Path of the source directory to be copied.</param>
        /// <param name="destinationDirectory">Name of the directory that the source directory will be copied to.</param>
        /// <param name="recursive">If <c>true</c> subfolders in the source directory are also added. Otherwise not.</param>
        /// <returns></returns>
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

        /// <summary>
        /// Add's directory to the package.
        /// </summary>
        /// <param name="sourceDirectoryPath">Path of the source directory to be copied.</param>
        /// <param name="destinationDirectory">Name of the directory that the source directory will be copied to.</param>
        /// <param name="recursive">If <c>true</c> subfolders in the source directory are also added. Otherwise not.</param>
        /// <param name="fileFilters"></param>
        /// <returns></returns>
        public PackageTask AddDirectoryToPackage(string sourceDirectoryPath, string destinationDirectory, bool recursive = false, params IFilter[] fileFilters)
        {
            SourcePackagingInfo directoryToPackage = new SourcePackagingInfo(
                SourceType.Directory,
                sourceDirectoryPath,
                destinationDirectory)
            { Recursive = recursive };

            directoryToPackage.FileFilters.AddRange(fileFilters);

            _sourcePackagingInfos.Add(directoryToPackage);
            return this;
        }

        /// <summary>
        /// Add's directory to the package.
        /// </summary>
        /// <param name="sourceDirectoryPath">Path of the source directory to be copied.</param>
        /// <param name="destinationDirectory">Name of the directory that the source directory will be copied to.</param>
        /// <param name="filterOptions">Apply filtering options for directories and files inside directories.</param>
        /// <returns></returns>
        public PackageTask AddDirectoryToPackage(string sourceDirectoryPath, string destinationDirectory, Action<FilterOptions> filterOptions)
        {
            FilterOptions fo = new FilterOptions();
            filterOptions.Invoke(fo);

            SourcePackagingInfo directoryToPackage = new SourcePackagingInfo(
                    SourceType.Directory,
                    sourceDirectoryPath,
                    destinationDirectory)
                { Recursive = fo.Recursive };

            directoryToPackage.FileFilters.AddRange(fo.FileFilters);
            directoryToPackage.DirectoryFilters.AddRange(fo.DirectoryFilters);
            _sourcePackagingInfos.Add(directoryToPackage);
            return this;
        }

        /// <summary>
        /// Adds file to the package
        /// </summary>
        /// <param name="sourceFilePath">Path of the tile to be added to the package.</param>
        /// <param name="destinationDirectory">Name of the directory that the source file will be copied to.</param>
        /// <returns></returns>
        public PackageTask AddFileToPackage(string sourceFilePath, string destinationDirectory)
        {
            _sourcePackagingInfos.Add(new SourcePackagingInfo(SourceType.File, sourceFilePath, destinationDirectory));
            return this;
        }

        /// <summary>
        /// If <c>true</c> zip is optimized by removing duplicated files. When unzipped those files are copied to original locations.
        /// For unzipping Unzip task has to be used.
        /// </summary>
        /// <returns></returns>
        public PackageTask OptimizeZip()
        {
            _optimizeZip = true;
            return this;
        }

        /// <summary>
        /// Disables logging of filtered out files and files to be copied / zipped.
        /// </summary>
        /// <returns></returns>
        public PackageTask DisableLogging()
        {
            _logFiles = false;
            return this;
        }

        /// <summary>
        /// Zip's the package
        /// </summary>
        /// <param name="zipFileName">File name of the zip package.</param>
        /// <param name="addVersionPostfix">if <c>true</c> build version number is added to zip file as postfix</param>
        /// <param name="versionFeildCount">Number of version fields to be added.</param>
        /// <returns></returns>
        public PackageTask ZipPackage(string zipFileName, bool addVersionPostfix = true, int versionFeildCount = 3)
        {
            _zipFileName = zipFileName;
            _addVersionAsPostFixToZipFileName = addVersionPostfix;
            _versionFieldCount = versionFeildCount;
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
            ICopier copier = new Copier(context, _logFiles);
            IZipper zipper = new Zipper(context);
            IDirectoryFilesLister directoryFilesLister = new DirectoryFilesLister();
            StandardPackageDef packageDef = new StandardPackageDef();

            CopyProcessor copyProcessor = new CopyProcessor(context, copier, df, _logFiles);

            List<string> sourceIds = new List<string>();

            foreach (var sourceToPackage in _sourcePackagingInfos)
            {
                string sourceId;
                if (sourceToPackage.SourceType == SourceType.Directory)
                {
                    var sourceFullPath = new FullPath(sourceToPackage.SourcePath);
                    sourceId = sourceFullPath.GetHashCode().ToString();
                    DirectorySource directorySource = new DirectorySource(context, directoryFilesLister, sourceId, sourceFullPath, sourceToPackage.Recursive, sourceToPackage.DirectoryFilters, _logFiles);
                    directorySource.SetFileFilter(sourceToPackage.FileFilters);
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
                {
                    zipFile = _zipPrefix;
                    _addVersionAsPostFixToZipFileName = true;
                    _versionFieldCount = 3;
                }

                if (zipFile.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
                {
                    zipFile = zipFile.Substring(0, zipFile.Length - 4);
                }

                string tmp = _addVersionAsPostFixToZipFileName
                    ? $"{zipFile}_{context.Properties.GetBuildVersion().Version.ToString(_versionFieldCount)}.zip"
                    : $"{zipFile}.zip";

                zipFile = Path.Combine(_destinationRootDir, tmp);

                DoLogInfo($"Creating zip file {zipFile}");

                ZipProcessor zipProcessor = new ZipProcessor(context, zipper, new FileFullPath(zipFile), df, _optimizeZip, sourceIds, _logFiles);
                zipProcessor.Process(copiedPackageDef);
            }

            return 0;
        }
    }
}
