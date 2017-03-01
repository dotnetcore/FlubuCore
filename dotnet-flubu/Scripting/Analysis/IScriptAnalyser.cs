using System.Collections.Generic;

namespace DotNet.Cli.Flubu.Scripting.Analysis
{
    public interface IScriptAnalyser
    {
        AnalyserResult Analyze(List<string> lines);
    }
}
