using System;
using System.Collections.Generic;
using System.Text;

namespace FlubuCore.Scripting
{
    public interface INugetPackageResolver
    {
        List<AssemblyInfo> ResolveNugetPackages(List<NugetPackageReference> packageReferences, string pathToBuildScript);
    }
}
