using System.Collections.Generic;
using Flubu.Context;
using Flubu.IO;
using Flubu.Tasks;

namespace Flubu.Packaging
{
    public class DirectorySource : IFilesSource
    {
        private readonly ITaskContext _taskContext;

        private readonly IDirectoryFilesLister _directoryFilesLister;

        private readonly string _id;

        private readonly FullPath _directoryPath;

        private readonly bool _recursive = true;

        public DirectorySource(
            ITaskContext taskContext,
            IDirectoryFilesLister directoryFilesLister,
            string id,
            FullPath directoryName)
            : this(taskContext, directoryFilesLister, id, directoryName, true)
        {
            _taskContext = taskContext;
        }

        public DirectorySource(
            ITaskContext taskContext,
            IDirectoryFilesLister directoryFilesLister,
            string id,
            FullPath directoryName,
            bool recursive)
        {
            _taskContext = taskContext;
            _directoryFilesLister = directoryFilesLister;
            _id = id;
            _recursive = recursive;
            _directoryPath = directoryName;
        }

        public string Id
        {
            get { return _id; }
        }

        private IFileFilter Filter { get; set; }

        public static DirectorySource NoFilterSource(
           ITaskContext taskContext,
           IDirectoryFilesLister directoryFilesLister,
           string id,
           FullPath directoryName,
           bool recursive)
        {
            return new DirectorySource(taskContext, directoryFilesLister, id, directoryName, recursive);
        }

        public static DirectorySource WebFilterSource(
            ITaskContext taskContext,
            IDirectoryFilesLister directoryFilesLister,
            string id,
            FullPath directoryName,
            bool recursive)
        {
            DirectorySource source = new DirectorySource(taskContext, directoryFilesLister, id, directoryName, recursive);
            source.SetFilter(new NegativeFilter(
                    new RegexFileFilter(@"^.*\.(svc|asax|config|aspx|ascx|css|js|gif|PNG)$")));

            return source;
        }

        public ICollection<PackagedFileInfo> ListFiles()
        {
            List<PackagedFileInfo> files = new List<PackagedFileInfo>();

            foreach (string fileName in _directoryFilesLister.ListFiles(
                _directoryPath.ToString(),
                _recursive))
            {
                FileFullPath fileNameFullPath = new FileFullPath(fileName);
                LocalPath debasedFileName = fileNameFullPath.ToFullPath().DebasePath(_directoryPath);

                if (!LoggingHelper.LogIfFilteredOut(fileName, Filter, _taskContext))
                {
                    continue;
                }

                PackagedFileInfo packagedFileInfo = new PackagedFileInfo(fileNameFullPath, debasedFileName);
                files.Add(packagedFileInfo);
            }

            return files;
        }

        public void SetFilter(IFileFilter filter)
        {
            Filter = filter;
        }
    }
}