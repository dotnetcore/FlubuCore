namespace FlubuCore.Scripting.Analysis.Processors
{
    public interface IScriptProcessor
    {
        bool Process(ScriptAnalyzerResult analyzerResult, string line, int lineIndex);
    }
}