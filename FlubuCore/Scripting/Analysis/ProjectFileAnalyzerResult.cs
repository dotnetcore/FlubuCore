using System;
using System.Collections.Generic;
using System.Text;

namespace FlubuCore.Scripting.Analysis
{
    public class ProjectFileAnalyzerResult
    {
        public bool ProjectFileFound { get; set; }

        public string ProjectFileLocation { get; set; }

        public bool HasAnyReferences
        {
            get
            {
                if (NugetReferences.Count != 0 && AssemblyReferences.Count != 0)
                {
                    return true;
                }

                return false;
            }
        }

        public List<NugetPackageReference> NugetReferences { get; set; } = new List<NugetPackageReference>();

        public List<AssemblyReference> AssemblyReferences { get; set; } = new List<AssemblyReference>();
    }
}
