using System;
using System.Collections.Generic;
using System.Text;

namespace FlubuCore.Scripting
{
    public class AssemblyInfo
    {
        public string Name { get; set; }

        public Version Version { get; set; }

        public VersionStatus VersionStatus { get; set; }

        public string FullPath { get; set; }

        /// <summary>
        /// Path to the runtime implementation assembly when <see cref="FullPath"/> points to a ref assembly.
        /// </summary>
        public string RuntimePath { get; set; }

        public bool IsCompileOnly { get; set; }

        public bool IsFromDependencyContext { get; set; }
    }

#pragma warning disable SA1201 // Elements should appear in the correct order
    public enum VersionStatus
#pragma warning restore SA1201 // Elements should appear in the correct order
    {
        Available,
        NotAvailable,
        Sealed,
    }
}
