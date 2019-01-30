using System;
using System.Collections.Generic;
using System.Text;
using FlubuCore.Scripting.Analysis;

namespace FlubuCore.Scripting
{
    public interface INugetPackageResolver
    {
        List<AssemblyInfo> ResolveNugetPackagesFromDirectives(List<NugetPackageReference> packageReferences, string pathToBuildScript);

        List<AssemblyInfo> ResolveNugetPackagesFromFlubuCsproj(ProjectFileAnalyzerResult analyzerResult);
    }
}
