using System;
using System.Collections.Generic;
using System.Text;
using DotNet.Cli.Flubu.Scripting.Analysis;
using DotNet.Cli.Flubu.Scripting.Processor;

namespace DotNet.Cli.Flubu.Scripting.Processors
{
    public class NamespaceDirectiveProcessor : IDirectiveProcessor
    {
        public bool Process(AnalyserResult analyserResult, string line, int lineIndex)
        {
            if (line.TrimStart().StartsWith("namespace"))
            {
                analyserResult.NamespaceIndex = lineIndex;
                return false;
            }

            return false;
        }
    }
}
