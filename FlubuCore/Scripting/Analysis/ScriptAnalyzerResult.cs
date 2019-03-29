using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using FlubuCore.Scripting.Attributes;

namespace FlubuCore.Scripting.Analysis
{
    public class ScriptAnalyzerResult
    {
        public string ClassName { get; set; }

        public bool IsPartial { get; set; }

        public string Namespace { get; set; }

        public int? NamespaceIndex { get; set; }

        public List<AssemblyInfo> AssemblyReferences { get; } = new List<AssemblyInfo>();

        public List<string> CsFiles { get; } = new List<string>();

        public List<Tuple<(string path, bool includeSubDirectories)>> CsDirectories { get; } = new List<Tuple<(string path, bool includeSubDirectories)>>();

        public List<string> PartialCsFiles { get; } = new List<string>();

        public List<NugetPackageReference> NugetPackageReferences { get; } = new List<NugetPackageReference>();

        public List<ScriptConfigAttributes> ScriptAttributes { get; } = new List<ScriptConfigAttributes>();
    }
}
