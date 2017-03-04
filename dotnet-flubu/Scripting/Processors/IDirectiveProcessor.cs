using DotNet.Cli.Flubu.Scripting.Analysis;

namespace DotNet.Cli.Flubu.Scripting.Processor
{
    public interface IDirectiveProcessor
    {
        bool Process(AnalyserResult analyserResult, string line, int lineIndex);
    }
}