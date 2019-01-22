using System.Collections.Generic;
using System.Reflection.Metadata;

namespace FlubuCore.Scripting.Analysis
{
    public class AnalyserResult
    {
        public string ClassName { get; set; }

        public int? NamespaceIndex { get; set; }

        public List<AssemblyInfo> References { get; } = new List<AssemblyInfo>();

        public List<string> CsFiles { get; } = new List<string>();

        public List<NugetPackageReference> NugetPackages { get; } = new List<NugetPackageReference>();
    }
}
