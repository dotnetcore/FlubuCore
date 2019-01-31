using System.Collections.Generic;
using System.Linq;
using FlubuCore.Scripting.Analysis.Processors;

namespace FlubuCore.Scripting.Analysis
{
    public class ScriptAnalyzer : IScriptAnalyzer
    {
        private readonly List<IScriptProcessor> _processors;

        public ScriptAnalyzer(IEnumerable<IScriptProcessor> processors)
        {
            _processors = processors.ToList();
        }

        public ScriptAnalyzerResult Analyze(List<string> lines)
        {
            ScriptAnalyzerResult analyzerResult = new ScriptAnalyzerResult();
            int i = 0;
            while (true)
            {
                if (i >= lines.Count)
                    break;

                string line = lines[i].TrimStart();

                foreach (var processor in _processors)
                {
                    bool ret = processor.Process(analyzerResult, line, i);

                    if (!string.IsNullOrEmpty(analyzerResult.ClassName))
                    {
                        RemoveNamespace(lines, analyzerResult);
                        return analyzerResult;
                    }

                    if (ret)
                    {
                        lines.RemoveAt(i);
                        i--;
                        break;
                    }
                }

                i++;
            }

            return analyzerResult;
        }

        private void RemoveNamespace(List<string> lines, ScriptAnalyzerResult analyzerResult)
        {
            if (analyzerResult.NamespaceIndex.HasValue)
            {
                lines.RemoveAt(analyzerResult.NamespaceIndex.Value);
                lines.RemoveAt(analyzerResult.NamespaceIndex.Value);
                var indexOfLastClosingCurlyBracket = lines.LastIndexOf("}");
                lines.RemoveAt(indexOfLastClosingCurlyBracket);
            }
        }
    }
}
