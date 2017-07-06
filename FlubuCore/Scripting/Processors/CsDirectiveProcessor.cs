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

            int csIndex = line.IndexOf(" ", StringComparison.Ordinal);

            if (csIndex < 0)
            {
                return true;
            }

            string csFile = line.Substring(csIndex);
            analyserResult.CsFiles.Add(Path.GetFullPath(csFile.Trim()));
            return true;
        }
    }
}
