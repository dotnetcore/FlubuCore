using Flubu.Context;
using Flubu.IO;
using Flubu.Tasks;

namespace Flubu.Packaging
{
    public class StandardPackageDef : CompositeFilesSource, IPackageDef
    {
        private readonly ITaskContext _taskContext;

        private readonly IDirectoryFilesLister _fileLister = new DirectoryFilesLister();

        public StandardPackageDef()
            : base(string.Empty)
        {
        }

        public StandardPackageDef(string id)
            : base(id)
        {
        }

        public StandardPackageDef(string id, ITaskContext taskContext)
            : base(id)
        {
            _taskContext = taskContext;
        }

        public StandardPackageDef(string id, ITaskContext taskContext, IDirectoryFilesLister directoryFilesLister)
            : base(id)
        {
            _taskContext = taskContext;
            _fileLister = directoryFilesLister;
        }

        public StandardPackageDef AddFolderSource(string id, FullPath directoryName, bool recursive)
        {
            DirectorySource source = new DirectorySource(_taskContext, _fileLister, id, directoryName, recursive);
            AddFilesSource(source);
            return this;
        }

        public StandardPackageDef AddFolderSource(string id, FullPath directoryName, bool recursive, IFileFilter filter)
        {
            DirectorySource source = new DirectorySource(_taskContext, _fileLister, id, directoryName, recursive);
            source.SetFilter(filter);
            AddFilesSource(source);
            return this;
        }

        public StandardPackageDef AddWebFolderSource(string id, FullPath directoryName, bool recursive)
        {
            DirectorySource source = new DirectorySource(_taskContext, _fileLister, id, directoryName, recursive);
            source.SetFilter(new NegativeFilter(
                    new RegexFileFilter(@"^.*\.(svc|asax|config|aspx|ascx|css|js|gif|png|jpg|jpeg|Master|eot|svg|ttf|woff|cshtml|swf|html|ico|txt|xml|json)$")));
            AddFilesSource(source);
            return this;
        }
    }
}