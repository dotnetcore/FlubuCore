using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace FlubuCore.Packaging
{
    public class CompositeFilesSource : ICompositeFilesSource
    {
        private readonly Dictionary<string, IFilesSource> _filesSources = new Dictionary<string, IFilesSource>();

        private IFilter _filter;

        public CompositeFilesSource(string id)
        {
            Id = id;
        }

        public string Id
        {
            get; set;
        }

        public void AddFilesSource(IFilesSource filesSource)
        {
            _filesSources.Add(filesSource.Id, filesSource);
        }

        public ICollection<PackagedFileInfo> ListFiles()
        {
            List<PackagedFileInfo> allFiles = new List<PackagedFileInfo>();

            foreach (IFilesSource filesSource in _filesSources.Values)
            {
                allFiles.AddRange(filesSource.ListFiles());
            }

            return allFiles;
        }

        public void SetFileFilter(IFilter filter)
        {
            _filter = filter;
        }

        public ICollection<IFilesSource> ListChildSources()
        {
            return _filesSources.Values;
        }
    }
}