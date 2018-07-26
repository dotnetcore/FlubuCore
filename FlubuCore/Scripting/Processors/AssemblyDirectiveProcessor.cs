using System;
using System.IO;
using FlubuCore.IO.Wrappers;
using FlubuCore.Scripting.Analysis;

namespace FlubuCore.Scripting.Processors
{
    public class AssemblyDirectiveProcessor : IDirectiveProcessor
    {
        private readonly IFileWrapper _file;

        private readonly IPathWrapper _pathWrapper;

        public AssemblyDirectiveProcessor(IFileWrapper file, IPathWrapper pathWrapper)
        {
            _file = file;
            _pathWrapper = pathWrapper;
        }

        public bool Process(AnalyserResult analyserResult, string line, int lineIndex)
        {
            if (!line.TrimStart().StartsWith("//#ass", StringComparison.OrdinalIgnoreCase))
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

            analyserResult.References.Add(pathToDll);
            return true;
        }
    }
}
