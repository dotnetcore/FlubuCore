using System;
using System.Collections.Generic;
using System.Text;

namespace FlubuCore.Scripting.Analysis
{
    public class ProjectFileAnalyzerResult
    {
        public bool ProjectFileFound { get; set; }

        public string ProjectFileLocation { get; set; }

        public List<NugetPackageReference> NugetReferences { get; set; } = new List<NugetPackageReference>();

        public List<AssemblyReference> AssemblyReferences { get; set; } = new List<AssemblyReference>();
    }
}
