using System;
using System.Collections.Generic;
using System.Text;
using FlubuCore.Scripting.Analysis;

namespace FlubuCore.Scripting.Processors
{
    public class NugetPackageDirectirveProcessor : IDirectiveProcessor
    {
        public bool Process(AnalyserResult analyserResult, string line, int lineIndex)
        {
            if (!line.StartsWith("//#nuget"))
                return false;

            int nugetIndex = line.IndexOf(" ", StringComparison.Ordinal);

            if (nugetIndex < 0)
                return true;

            string nugetPackageName = line.Substring(nugetIndex);

            analyserResult.NugetPackage.Add(nugetPackageName.Trim());
            return true;
        }
    }
}
