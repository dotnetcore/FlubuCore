using System.Collections.Generic;
using System.Linq;
using DotNet.Cli.Flubu.Scripting.Processor;

namespace DotNet.Cli.Flubu.Scripting.Analysis
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

            foreach (string line in lines)
            {
                foreach(var processor in _processors)
                {
                    bool ret = processor.Process(analyserResult, line);
                    if (ret)
                        break;
                }

                if (!string.IsNullOrEmpty(analyserResult.ClassName))
                    return analyserResult;
            }

            return analyserResult;
        }
    }
}
