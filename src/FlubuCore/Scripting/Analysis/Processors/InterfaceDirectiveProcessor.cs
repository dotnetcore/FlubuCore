using System;
using System.Collections.Generic;
using System.Text;

namespace FlubuCore.Scripting.Analysis.Processors
{
    public class InterfaceDirectiveProcessor : IScriptProcessor
    {
        public bool Process(ScriptAnalyzerResult analyzerResult, string line, int lineIndex)
        {
            var i = line.IndexOf("interface", StringComparison.Ordinal);
            if (i < 0)
            {
                return false;
            }

            var tmp = line.Substring(i + 10);
            tmp = tmp.TrimStart();
            i = tmp.IndexOf(" ", StringComparison.Ordinal);
            if (i == -1)
            {
                i = tmp.Length;
            }

            analyzerResult.InterfaceName = tmp.Substring(0, i);

            return true;
        }
    }
}
