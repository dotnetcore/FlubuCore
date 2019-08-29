using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using FlubuCore.Context;
using FlubuCore.IO;

namespace FlubuCore.Packaging
{
    public class CopyProcessor : IPackageProcessor
    {
        private readonly ITaskContextInternal _taskContext;

        private readonly ICopier _copier;

        private readonly FullPath _destinationRootDir;

        private readonly Dictionary<string, string> _fileTransformations = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        private Dictionary<string, CopyProcessorTransformation> _transformations =
            new Dictionary<string, CopyProcessorTransformation>();

        private IFilter _filter;

        private bool _logFiles;

        public CopyProcessor(ITaskContextInternal taskContext, ICopier copier, FullPath destinationRootDir, bool logFiles = true)
        {
            _taskContext = taskContext;
            _copier = copier;
            _destinationRootDir = destinationRootDir;
            _logFiles = logFiles;
        }

        public CopyProcessor AddTransformation(string sourceId, LocalPath destinationDir)
        {
            CopyProcessorTransformation transformation = new CopyProcessorTransformation(
                sourceId, destinationDir, CopyProcessorTransformationOptions.None);
            _transformations.Add(sourceId, transformation);
            return this;
        }

        public CopyProcessor AddTransformationWithDirFlattening(string sourceId, LocalPath destinationDir)
        {
            CopyProcessorTransformation transformation = new CopyProcessorTransformation(
                sourceId, destinationDir, CopyProcessorTransformationOptions.FlattenDirStructure);
            _transformations.Add(sourceId, transformation);
            return this;
        }

        /// <summary>
        /// Defines a transformation for <see cref="SingleFileSource"/> which copies the file to the destination
        /// and renames the file in the process.
        /// </summary>
        /// <param name="sourceId">ID of the <see cref="SingleFileSource"/>.</param>
        /// <param name="destinationFileName">The destination directory and file name (local path).</param>
        /// <returns>This same instance of the <see cref="CopyProcessor"/>.</returns>
        public CopyProcessor AddSingleFileTransformation(string sourceId, LocalPath destinationFileName)
        {
            CopyProcessorTransformation transformation = new CopyProcessorTransformation(sourceId, destinationFileName, CopyProcessorTransformationOptions.SingleFile);
            _transformations.Add(sourceId, transformation);
            return this;
        }

        /// <summary>
        /// Replace all occurrences of source file name with newFileName.
        /// </summary>
        /// <param name="fileName">Source file name to replace.</param>
        /// <param name="newFileName">Replace with new name.</param>
        /// <returns>Returns <see cref="CopyProcessor"/>.</returns>
        public CopyProcessor AddFileTransformation(string fileName, string newFileName)
        {
            _fileTransformations.Add(fileName, newFileName);
            return this;
        }

        public IPackageDef Process(IPackageDef packageDef)
        {
            return (IPackageDef)ProcessPrivate(packageDef, true);
        }

        public void SetFileFilter(IFilter filter)
        {
            _filter = filter;
        }

        private ICompositeFilesSource ProcessPrivate (
            ICompositeFilesSource compositeFilesSource,
            bool isRoot)
        {
            CompositeFilesSource transformedCompositeSource = isRoot
                ? new StandardPackageDef(compositeFilesSource.Id)
                : new CompositeFilesSource(compositeFilesSource.Id);

            foreach (IFilesSource filesSource in compositeFilesSource.ListChildSources())
            {
                if (filesSource is ICompositeFilesSource)
                {
                    throw new NotImplementedException("Child composites are currently not supported");
                }

                FilesList filesList = new FilesList(filesSource.Id);

                bool transformed = false;
                if (filesSource is SingleFileSource)
                {
                    transformed = TryToTransformSingleFileSource((SingleFileSource)filesSource, filesList);
                }

                if (!transformed)
                {
                    TransformSource(filesSource, filesList);
                }

                transformedCompositeSource.AddFilesSource(filesList);
            }

            return transformedCompositeSource;
        }

        private bool TryToTransformSingleFileSource(SingleFileSource source, FilesList filesList)
        {
            if (!_transformations.ContainsKey(source.Id))
            {
                return false;
            }

            CopyProcessorTransformation transformation = _transformations[source.Id];

            if ((transformation.Options & CopyProcessorTransformationOptions.SingleFile) == 0)
            {
                return false;
            }

            LocalPath destinationPath = transformation.DestinationPath;

            PackagedFileInfo sourceFile = source.ListFiles().ToList().First();
            FullPath destinationFullPath = _destinationRootDir.CombineWith(destinationPath);
            FileFullPath destinationFileFullPath = destinationFullPath.ToFileFullPath();

            filesList.AddFile(new PackagedFileInfo(destinationFileFullPath));
            _copier.Copy(sourceFile.FileFullPath, destinationFileFullPath);

            return true;
        }

        private void TransformSource(IFilesSource filesSource, FilesList filesList)
        {
            CopyProcessorTransformation transformation = FindTransformationForSource(filesSource.Id);
            bool flattenDirs = (transformation.Options & CopyProcessorTransformationOptions.FlattenDirStructure) != 0;

            LocalPath destinationPath = transformation.DestinationPath;

            foreach (PackagedFileInfo sourceFile in filesSource.ListFiles())
            {
                if (!LoggingHelper.LogIfFilteredOut(sourceFile.FileFullPath.ToString(), _filter, _taskContext, _logFiles))
                {
                    continue;
                }

                FullPath destinationFullPath = _destinationRootDir.CombineWith(destinationPath);

                if (sourceFile.LocalPath != null)
                {
                    LocalPath destLocalPath = sourceFile.LocalPath;
                    if (flattenDirs)
                    {
                        destLocalPath = destLocalPath.Flatten;
                    }

                    destinationFullPath = destinationFullPath.CombineWith(destLocalPath);
                }
                else
                {
                    destinationFullPath =
                        destinationFullPath.CombineWith(new LocalPath(sourceFile.FileFullPath.FileName));
                }

                string destFile = destinationFullPath.FileName;
                if (_fileTransformations.ContainsKey(destFile))
                {
                    destinationFullPath = destinationFullPath.ParentPath.CombineWith(
                        _fileTransformations[destFile]);
                }

                FileFullPath destinationFileFullPath = destinationFullPath.ToFileFullPath();
                filesList.AddFile(new PackagedFileInfo(destinationFileFullPath));

                _copier.Copy(sourceFile.FileFullPath, destinationFileFullPath);
            }
        }

        private CopyProcessorTransformation FindTransformationForSource(string sourceId)
        {
            if (!_transformations.ContainsKey(sourceId))
            {
                string message = string.Format(
                    CultureInfo.InvariantCulture,
                    "Source '{0}' is not registered for the transformation",
                    sourceId);
                throw new KeyNotFoundException(message);
            }

            return _transformations[sourceId];
        }
    }
}