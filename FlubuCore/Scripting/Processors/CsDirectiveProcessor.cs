using System;
using System.IO;
using FlubuCore.Scripting.Analysis;

namespace FlubuCore.Scripting.Processors
{
    public class CsDirectiveProcessor : IDirectiveProcessor
    {
        public bool Process(ScriptAnalyzerResult analyzerResult, string line, int lineIndex)
        {
            if (!line.TrimStart().StartsWith("//#imp", StringComparison.OrdinalIgnoreCase))
                return false;

            int index = line.IndexOf(" ", StringComparison.Ordinal);

            if (index < 0)
            {
                return true;
            }

            string file = line.Substring(index);
            analyzerResult.CsFiles.Add(Path.GetFullPath(file.Trim()));
            return true;
        }
    }
}
