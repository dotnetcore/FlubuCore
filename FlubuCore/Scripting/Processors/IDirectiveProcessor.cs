using FlubuCore.Scripting.Analysis;

namespace FlubuCore.Scripting.Processors
{
    public interface IDirectiveProcessor
    {
        bool Process(ScriptAnalyzerResult analyzerResult, string line, int lineIndex);
    }
}