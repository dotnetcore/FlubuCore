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
        public SourceToPackage(string sourceId, SourceType sourceType, string sourcePath, string destinationPath)
        {
            SourceId = sourceId;
            SourcePath = sourcePath;
            SourceType = sourceType;
            DestinationPath = new LocalPath(destinationPath);
            FileFilters = new FilterCollection();
        }

        public string SourceId { get; set; }

        public SourceType SourceType { get; set; }

        public string SourcePath { get; set; }

        public LocalPath DestinationPath { get; set; }

        public FilterCollection FileFilters { get; set; }
    }
}
