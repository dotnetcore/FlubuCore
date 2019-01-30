using System.Collections.Generic;

namespace FlubuCore.Scripting.Analysis
{
    public interface IScriptAnalyzer
    {
        ScriptAnalyzerResult Analyze(List<string> lines);
    }
}
