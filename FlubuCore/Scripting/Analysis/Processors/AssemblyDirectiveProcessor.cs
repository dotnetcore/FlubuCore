using System;
using System.IO;
using FlubuCore.IO.Wrappers;
using Microsoft.Extensions.Logging;

namespace FlubuCore.Scripting.Analysis.Processors
{
    public class AssemblyDirectiveProcessor : IScriptProcessor
    {
        private readonly ILogger<AssemblyDirectiveProcessor> _log;

        private readonly IFileWrapper _file;

        private readonly IPathWrapper _pathWrapper;

        public AssemblyDirectiveProcessor(IFileWrapper file, IPathWrapper pathWrapper,  ILogger<AssemblyDirectiveProcessor> log)
        {
            _log = log;
            _file = file;
            _pathWrapper = pathWrapper;
        }

        public bool Process(ScriptAnalyzerResult analyzerResult, string line, int lineIndex)
        {
            if (!line.StartsWith("//#ass", StringComparison.OrdinalIgnoreCase))
                return false;

            int dllIndex = line.IndexOf(" ", StringComparison.Ordinal);

            if (dllIndex < 0)
                return true;

            string dll = line.Substring(dllIndex);
            string pathToDll = Path.GetFullPath(dll.Trim());
            string extension = _pathWrapper.GetExtension(pathToDll);
            if (!extension.Equals(".dll", StringComparison.OrdinalIgnoreCase))
            {
                if (!extension.Equals(".exe", StringComparison.OrdinalIgnoreCase))
                {
                    throw new ScriptException($"File doesn't have dll extension. {pathToDll}");
                }
            }

            if (!_file.Exists(pathToDll))
            {
                throw new ScriptException($"Assembly not found at location: {pathToDll}");
            }

            _log.LogInformation("#ass directives are obsolete and will be removed in future versions. Use 'Assembly' attribute instead on build script class.");

            analyzerResult.AssemblyReferences.Add(new AssemblyInfo
            {
                Name = pathToDll,
                VersionStatus = VersionStatus.NotAvailable,
                FullPath = pathToDll
            });

            return true;
        }
    }
}
