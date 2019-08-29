using System.Collections.Generic;
using FlubuCore.IO;

namespace FlubuCore.Packaging
{
    public class SingleFileSource : IFilesSource
    {
        private readonly string _id;

        private readonly FileFullPath _fileName;

        public SingleFileSource(string id, FileFullPath fileName)
        {
            _id = id;
            _fileName = fileName;
        }

        public string Id
        {
            get { return _id; }
        }

        public ICollection<PackagedFileInfo> ListFiles()
        {
            List<PackagedFileInfo> files = new List<PackagedFileInfo>();
            files.Add(new PackagedFileInfo(_fileName));
            return files;
        }

        public void SetFileFilter(IFilter filter)
        {
        }
    }
}