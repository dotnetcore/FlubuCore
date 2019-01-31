using System;

namespace FlubuCore.Scripting.Analysis.Processors
{
    public class ClassDirectiveProcessor : IScriptProcessor
    {
        public bool Process(ScriptAnalyzerResult analyzerResult, string line, int lineIndex)
        {
            var i = line.IndexOf("class", StringComparison.Ordinal);
            if (i < 0)
                return false;

            var tmp = line.Substring(i + 6);
            tmp = tmp.TrimStart();
            i = tmp.IndexOf(" ", StringComparison.Ordinal);
            if (i == -1)
            {
                i = tmp.Length;
            }

            analyzerResult.ClassName = tmp.Substring(0, i);
            return true;
        }
    }
}
