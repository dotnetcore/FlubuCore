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
