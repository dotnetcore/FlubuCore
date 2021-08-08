using System;
using System.Collections.Generic;
using System.Text;
using FlubuCore.Packaging;
using FlubuCore.Packaging.Filters;

namespace FlubuCore.Tasks.Packaging
{
    public class PackageBuilder
    {
        private readonly List<SourcePackagingInfo> _sourcePackagingInfos = new List<SourcePackagingInfo>();

        public List<SourcePackagingInfo> SourcePackagingInfo => _sourcePackagingInfos;

        /// <summary>
        /// Add's specified directory to the package.
        /// </summary>
        /// <param name="sourceDirectoryPath">Path of the source directory to be copied.</param>
        /// <param name="destinationDirectory">Name of the directory that the source directory will be copied to.</param>
        /// <param name="recursive">If <c>true</c> subfolders in the source directory are also added. Otherwise not.</param>
        /// <returns></returns>
        public PackageBuilder AddDirectoryToPackage(string sourceDirectoryPath, string destinationDirectory, bool recursive = false)
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
        public PackageBuilder AddDirectoryToPackage(string sourceDirectoryPath, string destinationDirectory, bool recursive = false, params IFilter[] fileFilters)
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
        public PackageBuilder AddDirectoryToPackage(string sourceDirectoryPath, string destinationDirectory, Action<FilterOptions> filterOptions)
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
        public PackageBuilder AddFileToPackage(string sourceFilePath, string destinationDirectory)
        {
            _sourcePackagingInfos.Add(new SourcePackagingInfo(SourceType.File, sourceFilePath, destinationDirectory));
            return this;
        }
    }
}
