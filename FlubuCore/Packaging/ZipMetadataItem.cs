using System.Collections.Generic;

namespace FlubuCore.Packaging
{
    public class ZipMetadataItem
    {
        public string FileName { get; set; }

        public List<string> DestinationFiles { get; } = new List<string>();
    }
}