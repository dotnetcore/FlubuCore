using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlubuCore.IO;
using FlubuCore.Packaging;

namespace FlubuCore.Tasks.Packaging
{
    public class SourceToPackage
    {
        public SourceToPackage(string sourceId, string sourcePath, string destinationPath)
        {
            SourceId = sourceId;
            SourcePath = new FullPath(sourcePath);
            DestinationPath = new LocalPath(destinationPath);
            FileFilters = new FilterCollection();
        }

        public string SourceId { get; set; }

        public FullPath SourcePath { get; set; }

        public LocalPath DestinationPath { get; set; }

        public FilterCollection FileFilters { get; set; }
    }
}
