using System.Collections.Generic;

namespace FlubuCore.Scripting.Analysis
{
    public class AnalyserResult
    {
        public string ClassName { get; set; }

        public int? NamespaceIndex { get; set; }

        public List<string> References { get; } = new List<string>();

        public List<string> CsFiles { get; } = new List<string>();

        public List<string> NugetPackage { get; } = new List<string>();
    }
}
