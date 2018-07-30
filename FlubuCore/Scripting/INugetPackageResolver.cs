using System;
using System.Collections.Generic;
using System.Text;

namespace FlubuCore.Scripting
{
    public interface INugetPackageResolver
    {
        List<string> ResolveNugetPackages(List<NugetPackageReference> packageReferences, string pathToBuildScript);
    }
}
