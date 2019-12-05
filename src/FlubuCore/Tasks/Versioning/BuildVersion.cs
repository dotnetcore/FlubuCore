using System;

namespace FlubuCore.Tasks.Versioning
{
    public class BuildVersion
    {
        public Version Version { get; set; }

        public string VersionQuality { get; set; }

        public string BuildVersionWithQuality(int versionFieldCount)
        {
            string quality = !string.IsNullOrEmpty(VersionQuality) ? $"-{VersionQuality}" : null;
            return $"{Version.ToString(versionFieldCount)}{quality}";
        }
    }
}
