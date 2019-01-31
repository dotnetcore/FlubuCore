using FlubuCore.Scripting.Analysis;

namespace FlubuCore.Scripting.Processors
{
    public interface IScriptProcessor
    {
        bool Process(ScriptAnalyzerResult analyzerResult, string line, int lineIndex);
    }
}