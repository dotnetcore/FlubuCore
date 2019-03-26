using System;
using System.Reflection;
using Microsoft.Extensions.Logging;

namespace FlubuCore.Scripting.Analysis.Processors
{
    public class ReferenceDirectiveProcessor : IScriptProcessor
    {
        private readonly ILogger<ReferenceDirectiveProcessor> _logger;

        public ReferenceDirectiveProcessor(ILogger<ReferenceDirectiveProcessor> logger)
        {
            _logger = logger;
        }

        public bool Process(ScriptAnalyzerResult analyzerResult, string line, int lineIndex)
        {
            if (!line.StartsWith("//#ref", StringComparison.OrdinalIgnoreCase))
                return false;

            int dllIndex = line.IndexOf(" ", StringComparison.Ordinal);

            if (dllIndex < 0)
                return true;

            string dll = line.Substring(dllIndex);
            var type = Type.GetType(dll, true);
            var ass = type.GetTypeInfo().Assembly;
            _logger.LogInformation("#ref directives are obsolete and will be removed in future versions. Use 'Reference' attribute instead on build script class.");

            analyzerResult.AssemblyReferences.Add(ass.ToAssemblyInfo());
            return true;
        }
    }
}
