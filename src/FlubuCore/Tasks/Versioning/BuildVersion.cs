using System;

namespace FlubuCore.Tasks.Versioning
{
    public class BuildVersion
    {
        public BuildVersion()
        {
        }

        public BuildVersion(Version version)
        {
            Version = version;
        }

        public Version Version { get; set; }

        public string VersionQuality { get; set; }

        public string BuildVersionWithQuality(int? versionFieldCount = null)
        {
            string quality = null;

            if (!string.IsNullOrEmpty(VersionQuality))
            {
                quality = VersionQuality.StartsWith("-") ? VersionQuality : $"-{VersionQuality}";
            }

            return versionFieldCount.HasValue ? $"{Version.ToString(versionFieldCount.Value)}{quality}" : $"{Version.ToString()}{quality}";
        }
    }
}
