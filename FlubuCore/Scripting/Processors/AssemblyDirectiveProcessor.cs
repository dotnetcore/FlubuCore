using System;
using System.IO;
using FlubuCore.Scripting.Analysis;

namespace FlubuCore.Scripting.Processors
{
    public class AssemblyDirectiveProcessor : IDirectiveProcessor
    {
        public bool Process(AnalyserResult analyserResult, string line, int lineIndex)
        {
            if (!line.TrimStart().StartsWith("//#ass", StringComparison.OrdinalIgnoreCase))
                return false;

            int dllIndex = line.IndexOf(" ", StringComparison.Ordinal);

            if (dllIndex < 0)
                return true;

            string dll = line.Substring(dllIndex);
            analyserResult.References.Add(Path.GetFullPath(dll.Trim()));
            return true;
        }
    }
}
