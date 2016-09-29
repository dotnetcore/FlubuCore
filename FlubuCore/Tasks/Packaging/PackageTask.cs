using System.Collections.Generic;
using FlubuCore.Context;
using FlubuCore.IO;
using FlubuCore.Packaging;

namespace FlubuCore.Tasks.Packaging
{
    public class PackageTask : TaskBase
    {
        private List<SourceToPackage> _sourcesToPackage;

        private FullPath _destinationRootDir;

        private string _zipFileName;

        public PackageTask(string destinationRootDir, string zipFileName)
        {
            _sourcesToPackage = new List<SourceToPackage>();
            _destinationRootDir = new FullPath(destinationRootDir);
            _zipFileName = zipFileName;
        }

        public override string Description => "Packages specified folders.";

        public PackageTask AddDirectoryToPackage(string sourceId, string path, string destinationDir)
        {
            _sourcesToPackage.Add(new SourceToPackage(sourceId, path, destinationDir));
            return this;
        }

        public PackageTask AddDirectoryToPackage(string sourceId, string path, string destinationDir, params IFileFilter[] fileFilters)
        {
            var sourceToPackage = new SourceToPackage(sourceId, path, destinationDir);

            foreach (var filter in fileFilters)
            {
                sourceToPackage.FileFilters.Add(filter);
            }

            _sourcesToPackage.Add(sourceToPackage);
            return this;
        }

        protected override int DoExecute(ITaskContext context)
        {
            if (_sourcesToPackage.Count == 0)
            {
                return 0;
            }

            ICopier copier = new Copier(context);
            IZipper zipper = new Zipper(context);
            IDirectoryFilesLister directoryFilesLister = new DirectoryFilesLister();
            StandardPackageDef packageDef = new StandardPackageDef();
            CopyProcessor copyProcessor = new CopyProcessor(
            context,
            copier,
            _destinationRootDir);
            List<string> sourceIds = new List<string>();
            foreach (var sourceToPackage in _sourcesToPackage)
            {
                DirectorySource directorySource = new DirectorySource(context, directoryFilesLister, sourceToPackage.SourceId, sourceToPackage.SourcePath);
                directorySource.SetFilter(sourceToPackage.FileFilters);
                packageDef.AddFilesSource(directorySource);
                copyProcessor.AddTransformation(sourceToPackage.SourceId, sourceToPackage.DestinationPath);
                sourceIds.Add(sourceToPackage.SourceId);
            }

            IPackageDef copiedPackageDef = copyProcessor.Process(packageDef);
            ZipProcessor zipProcessor = new ZipProcessor(context, zipper, new FileFullPath(_zipFileName), _destinationRootDir, sourceIds);
            zipProcessor.Process(copiedPackageDef);

            return 0;
        }
    }
}
