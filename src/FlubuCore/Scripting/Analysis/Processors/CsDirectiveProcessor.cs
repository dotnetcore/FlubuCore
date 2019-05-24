using System;
using System.IO;

namespace FlubuCore.Scripting.Analysis.Processors
{
    public class CsDirectiveProcessor : IScriptProcessor
    {
        public bool Process(ScriptAnalyzerResult analyzerResult, string line, int lineIndex)
        {
            if (!line.StartsWith("//#imp", StringComparison.OrdinalIgnoreCase))
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
