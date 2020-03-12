using System;
using System.Collections.Generic;
using System.Text;

namespace FlubuCore.Tasks.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class FetchBuildVersionFromFileAttribute : Attribute
    {
        public string ProjectVersionFileName { get; set; }

        public bool AllowSuffix { get; set; }

        public string[] PrefixesToRemove { get; set; }
    }
}
