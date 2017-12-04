using System.Collections.Generic;
using System.Linq;
using FlubuCore.Scripting.Processors;

namespace FlubuCore.Scripting.Analysis
{
    public class ScriptAnalyser : IScriptAnalyser
    {
        private readonly List<IDirectiveProcessor> _processors;

        public ScriptAnalyser(IEnumerable<IDirectiveProcessor> processors)
        {
            _processors = processors.ToList();
        }

        public AnalyserResult Analyze(List<string> lines)
        {
            AnalyserResult analyserResult = new AnalyserResult();
            int i = 0;
            while (true)
            {
                if (i >= lines.Count)
                    break;

                string line = lines[i];

                foreach (var processor in _processors)
                {
                    bool ret = processor.Process(analyserResult, line, i);

                    if (!string.IsNullOrEmpty(analyserResult.ClassName))
                    {
                        RemoveNamespace(lines, analyserResult);
                        return analyserResult;
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

            return analyserResult;
        }

        private void RemoveNamespace(List<string> lines, AnalyserResult analyserResult)
        {
            if (analyserResult.NamespaceIndex.HasValue)
            {
                lines.RemoveAt(analyserResult.NamespaceIndex.Value);
                lines.RemoveAt(analyserResult.NamespaceIndex.Value);
                var indexOfLastClosingCurlyBracket = lines.LastIndexOf("}");
                lines.RemoveAt(indexOfLastClosingCurlyBracket);
            }
        }
    }
}
