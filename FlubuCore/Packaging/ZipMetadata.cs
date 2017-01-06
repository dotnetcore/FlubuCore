using System.Collections.Generic;

namespace FlubuCore.Packaging
{
    public class ZipMetadata
    {
        public List<ZipMetadataItem> Items { get; } = new List<ZipMetadataItem>();
    }
}
