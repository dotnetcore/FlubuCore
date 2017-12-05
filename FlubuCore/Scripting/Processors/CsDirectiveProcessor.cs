using System;
using System.IO;
using FlubuCore.Scripting.Analysis;

namespace FlubuCore.Scripting.Processors
{
    public class CsDirectiveProcessor : IDirectiveProcessor
    {
        public bool Process(AnalyserResult analyserResult, string line, int lineIndex)
        {
            if (!line.StartsWith("//#imp"))
                return false;

            int index = line.IndexOf(" ", StringComparison.Ordinal);

            if (index < 0)
            {
                return true;
            }

            string file = line.Substring(index);
            analyserResult.CsFiles.Add(Path.GetFullPath(file.Trim()));
            return true;
        }
    }
}
