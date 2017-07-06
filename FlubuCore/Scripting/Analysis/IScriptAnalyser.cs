using System.Collections.Generic;

namespace FlubuCore.Scripting.Analysis
{
    public interface IScriptAnalyser
    {
        AnalyserResult Analyze(List<string> lines);
    }
}
