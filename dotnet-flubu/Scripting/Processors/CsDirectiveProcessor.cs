using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using DotNet.Cli.Flubu.Scripting.Analysis;
using DotNet.Cli.Flubu.Scripting.Processor;

namespace DotNet.Cli.Flubu.Scripting.Processors
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
