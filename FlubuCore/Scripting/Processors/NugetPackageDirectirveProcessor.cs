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

            string nugetPackage = line.Substring(nugetIndex);

           var nugetInfos = nugetPackage.Split(',');

            analyserResult.NugetPackages.Add(new NugetPackageReference { Id = nugetInfos[0].Trim(), Version = nugetInfos[1].Trim() });
            return true;
        }
    }
}
