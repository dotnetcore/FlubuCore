using System;
using System.Collections.Generic;

namespace Flubu.Packaging
{
    public class FilesList : IFilesSource
    {
        private readonly string _id;

        private List<PackagedFileInfo> _files = new List<PackagedFileInfo>();

        private IFileFilter _filter;

        public FilesList(string id)
        {
            _id = id;
        }

        public string Id
        {
            get { return _id; }
        }

        public void AddFile(PackagedFileInfo packagedFileInfo)
        {
            _files.Add(packagedFileInfo);
        }

        public ICollection<PackagedFileInfo> ListFiles()
        {
            if (_filter != null)
            {
                return _files.FindAll(x => _filter.IsPassedThrough(x.FileFullPath.ToString()));
            }

            return _files;
        }

        public void SetFilter(IFileFilter filter)
        {
            _filter = filter;
        }
    }
}