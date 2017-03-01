using DotNet.Cli.Flubu.Scripting.Analysis;
using DotNet.Cli.Flubu.Scripting.Processor;

namespace DotNet.Cli.Flubu.Scripting.Processors
{
    public class ReferenceDirectiveProcessor : IDirectiveProcessor
    {
        public bool Process(AnalyserResult analyserResult, string line)
        {
            if (!line.StartsWith("#ref"))
                return false;

            return true;
        }
    }
}
