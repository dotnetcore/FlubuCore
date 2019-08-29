using FlubuCore.Context;
using FlubuCore.IO;

namespace FlubuCore.Packaging
{
    public class StandardPackageDef : CompositeFilesSource, IPackageDef
    {
        private readonly ITaskContextInternal _taskContext;

        private readonly IDirectoryFilesLister _fileLister = new DirectoryFilesLister();

        public StandardPackageDef()
            : base(string.Empty)
        {
        }

        public StandardPackageDef(string id)
            : base(id)
        {
        }

        public StandardPackageDef(string id, ITaskContextInternal taskContext)
            : base(id)
        {
            _taskContext = taskContext;
        }

        public StandardPackageDef(string id, ITaskContextInternal taskContext, IDirectoryFilesLister directoryFilesLister)
            : base(id)
        {
            _taskContext = taskContext;
            _fileLister = directoryFilesLister;
        }

        public StandardPackageDef AddFolderSource(string id, FullPath directoryName, bool recursive)
        {
            DirectorySource source = new DirectorySource(_taskContext, _fileLister, id, directoryName, recursive, null);
            AddFilesSource(source);
            return this;
        }

        public StandardPackageDef AddFolderSource(string id, FullPath directoryName, bool recursive, IFilter filter)
        {
            DirectorySource source = new DirectorySource(_taskContext, _fileLister, id, directoryName, recursive, null);
            source.SetFileFilter(filter);
            AddFilesSource(source);
            return this;
        }

        public StandardPackageDef AddWebFolderSource(string id, FullPath directoryName, bool recursive)
        {
            DirectorySource source = new DirectorySource(_taskContext, _fileLister, id, directoryName, recursive, null);
            source.SetFileFilter(new NegativeFilter(
                    new RegexFilter(@"^.*\.(svc|asax|config|aspx|ascx|css|js|gif|png|jpg|jpeg|Master|eot|svg|ttf|woff|cshtml|swf|html|ico|txt|xml|json)$")));
            AddFilesSource(source);
            return this;
        }
    }
}