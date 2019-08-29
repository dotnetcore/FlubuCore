using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlubuCore.IO;
using FlubuCore.Packaging;

namespace FlubuCore.Tasks.Packaging
{
    public class SourcePackagingInfo
    {
        public SourcePackagingInfo(SourceType sourceType, string sourcePath, string destinationPath)
        {
            SourcePath = sourcePath;
            SourceType = sourceType;
            DestinationPath = new LocalPath(destinationPath);
            FileFilters = new FilterCollection();
            DirectoryFilters = new FilterCollection();
        }

        public SourceType SourceType { get; set; }

        public string SourcePath { get; set; }

        public LocalPath DestinationPath { get; set; }

        public FilterCollection FileFilters { get; set; }

        public FilterCollection DirectoryFilters { get; set; }

        public bool Recursive { get; set; }
    }
}
