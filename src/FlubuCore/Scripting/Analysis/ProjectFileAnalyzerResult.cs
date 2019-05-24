using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FlubuCore.Scripting.Analysis
{
    public class ProjectFileAnalyzerResult
    {
        public bool ProjectFileFound { get; set; }

        public string ProjectFileLocation { get; set; }

        public bool HasAnyNugetReferences => NugetReferences.Count != 0;

        public bool HasAnyAssemblyReferences => AssemblyReferences.Count != 0;

        public List<NugetPackageReference> NugetReferences { get; set; } = new List<NugetPackageReference>();

        public List<AssemblyReference> AssemblyReferences { get; set; } = new List<AssemblyReference>();
    }
}
