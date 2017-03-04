using System.Collections.Generic;

namespace DotNet.Cli.Flubu.Scripting.Analysis
{
    public class AnalyserResult
    {
        public string ClassName { get; set; }

        public int? NamespaceIndex { get; set; }

        public List<string> References { get; } = new List<string>();
    }
}
