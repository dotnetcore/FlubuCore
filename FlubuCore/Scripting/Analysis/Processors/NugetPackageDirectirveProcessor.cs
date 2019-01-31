using System;

namespace FlubuCore.Scripting.Analysis.Processors
{
    public class NugetPackageDirectirveProcessor : IScriptProcessor
    {
        public bool Process(ScriptAnalyzerResult analyzerResult, string line, int lineIndex)
        {
            if (!line.StartsWith("//#nuget", StringComparison.OrdinalIgnoreCase))
                return false;

            int nugetIndex = line.IndexOf(" ", StringComparison.Ordinal);

            if (nugetIndex < 0)
                return true;

            string nugetPackage = line.Substring(nugetIndex);

            var nugetInfos = nugetPackage.Split(',');

            if (nugetInfos.Length != 2)
            {
                throw new ScriptException("Invalid nuget package directive. Example of valid directive: '//#nuget: FlubuCore, 2.8.0'");
            }

            analyzerResult.NugetPackageReferences.Add(new NugetPackageReference { Id = nugetInfos[0].Trim(), Version = nugetInfos[1].Trim() });
            return true;
        }
    }
}
