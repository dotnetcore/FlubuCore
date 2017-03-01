using System;
using DotNet.Cli.Flubu.Scripting.Analysis;
using DotNet.Cli.Flubu.Scripting.Processor;

namespace DotNet.Cli.Flubu.Scripting.Processors
{
    public class AssemblyDirectiveProcessor : IDirectiveProcessor
    {
        public bool Process(AnalyserResult analyserResult, string line)
        {
            if (!line.StartsWith("//#ass"))
                return false;

            int dllIndex = line.IndexOf(" ", StringComparison.Ordinal);

            if (dllIndex < 0)
                return true;

            string dll = line.Substring(dllIndex);
            
            analyserResult.References.Add(dll.Trim());
            return true;
        }
    }
}
