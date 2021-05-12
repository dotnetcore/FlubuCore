using System.Collections.Generic;
using FlubuCore.Context;
using FlubuCore.IO;

namespace FlubuCore.Packaging
{
    public class DirectorySource : IFilesSource
    {
        private readonly ITaskContextInternal _taskContext;

        private readonly IDirectoryFilesLister _directoryFilesLister;

        private readonly string _id;

        private readonly FullPath _directoryPath;

        private readonly bool _recursive = true;

        private readonly bool _logFiles = true;

        public DirectorySource(
            ITaskContextInternal taskContext,
            IDirectoryFilesLister directoryFilesLister,
            string id,
            FullPath directoryName)
            : this(taskContext, directoryFilesLister, id, directoryName, true, null)
        {
            _taskContext = taskContext;
        }

        public DirectorySource(
            ITaskContextInternal taskContext,
            IDirectoryFilesLister directoryFilesLister,
            string id,
            FullPath directoryName,
            bool recursive,
            IFilter filter,
            bool logFiles = true)
        {
            _taskContext = taskContext;
            _directoryFilesLister = directoryFilesLister;
            _id = id;
            _recursive = recursive;
            DirectoryFilter = filter;
            _directoryPath = directoryName;
            _logFiles = logFiles;
        }

        public string Id
        {
            get { return _id; }
        }

        private IFilter FileFilter { get; set; }

        private IFilter DirectoryFilter { get; set; }

        public static DirectorySource NoFilterSource(
           ITaskContextInternal taskContext,
           IDirectoryFilesLister directoryFilesLister,
           string id,
           FullPath directoryName,
           bool recursive)
        {
            return new DirectorySource(taskContext, directoryFilesLister, id, directoryName, recursive, null);
        }

        public static DirectorySource WebFilterSource(
            ITaskContextInternal taskContext,
            IDirectoryFilesLister directoryFilesLister,
            string id,
            FullPath directoryName,
            bool recursive)
        {
            DirectorySource source = new DirectorySource(taskContext, directoryFilesLister, id, directoryName, recursive, null);
            source.SetFileFilter(new NegativeFilter(
                    new RegexFilter(@"^.*\.(svc|asax|config|aspx|ascx|css|js|gif|PNG)$")));

            return source;
        }

        public ICollection<PackagedFileInfo> ListFiles()
        {
            List<PackagedFileInfo> files = new List<PackagedFileInfo>();

            foreach (string fileName in _directoryFilesLister.ListFiles(
                _directoryPath.ToString(),
                _recursive, DirectoryFilter))
            {
                FileFullPath fileNameFullPath = new FileFullPath(fileName);
                LocalPath debasedFileName = fileNameFullPath.ToFullPath().DebasePath(_directoryPath);

                if (!LoggingHelper.LogIfFilteredOut(fileName, FileFilter, _taskContext, _logFiles))
                {
                    continue;
                }

                PackagedFileInfo packagedFileInfo = new PackagedFileInfo(fileNameFullPath, debasedFileName);
                files.Add(packagedFileInfo);
            }

            return files;
        }

        public void SetFileFilter(IFilter filter)
        {
            FileFilter = filter;
        }

        public void SetDirectoryFilter(IFilter filter)
        {
            DirectoryFilter = filter;
        }
    }
}