using System;
using FlubuCore.Tasks.Versioning;

namespace FlubuCore.Tasks.Attributes
{
    /// <summary>
    /// Executes <see cref="FetchBuildVersionFromFileTask"/> and injects Build version into property.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class FetchBuildVersionFromFileAttribute : Attribute
    {
        public string ProjectVersionFileName { get; set; }

        public bool AllowSuffix { get; set; }

        public string[] PrefixesToRemove { get; set; }
    }
}
