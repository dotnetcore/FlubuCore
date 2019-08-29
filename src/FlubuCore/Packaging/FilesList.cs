using System;
using System.Collections.Generic;

namespace FlubuCore.Packaging
{
    public class FilesList : IFilesSource
    {
        private readonly string _id;

        private List<PackagedFileInfo> _files = new List<PackagedFileInfo>();

        private IFilter _filter;

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
                return _files.FindAll(x => _filter.IsPassedThrough(x.LocalPath.ToString()));
            }

            return _files;
        }

        public void SetFileFilter(IFilter filter)
        {
            _filter = filter;
        }
    }
}