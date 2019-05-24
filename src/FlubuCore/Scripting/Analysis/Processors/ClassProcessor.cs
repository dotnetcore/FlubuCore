using System;
using FlubuCore.IO.Wrappers;

namespace FlubuCore.Scripting.Analysis.Processors
{
    public class ClassDirectiveProcessor : IScriptProcessor
    {
        private readonly IFileWrapper _file;

        private readonly IPathWrapper _path;

        public ClassDirectiveProcessor(IFileWrapper file, IPathWrapper path)
        {
            _file = file;
            _path = path;
        }

        public bool Process(ScriptAnalyzerResult analyzerResult, string line, int lineIndex)
        {
            var i = line.IndexOf("class", StringComparison.Ordinal);
            if (i < 0)
                return false;

            ProcessPartial(analyzerResult, line, i);

            ProcessClassName(analyzerResult, line, i);

            ProcessBaseClass(analyzerResult, line);
            return true;
        }

        private static void ProcessClassName(ScriptAnalyzerResult analyzerResult, string line, int i)
        {
            var tmp = line.Substring(i + 6);
            tmp = tmp.TrimStart();
            i = tmp.IndexOf(" ", StringComparison.Ordinal);
            if (i == -1)
            {
                i = tmp.Length;
            }

            analyzerResult.ClassName = tmp.Substring(0, i);
        }

        private static void ProcessPartial(ScriptAnalyzerResult analyzerResult, string line, int i)
        {
            var partialIndex = line.IndexOf("partial", StringComparison.Ordinal);
            if (partialIndex > 0 && partialIndex < i)
            {
                analyzerResult.IsPartial = true;
            }
        }

        private void ProcessBaseClass(ScriptAnalyzerResult analyzerResult, string line)
        {
            var baseClassIndex = line.IndexOf(":", StringComparison.Ordinal);

            if (baseClassIndex < 1)
            {
                return;
            }

            var baseClassName = line.Substring(baseClassIndex + 1);
            baseClassName = baseClassName.TrimStart();
            var baseClassEndIndex = baseClassName.IndexOf(" ", StringComparison.Ordinal);

            if (baseClassEndIndex != -1)
            {
                baseClassName = baseClassName.Substring(0, baseClassEndIndex);
            }

            analyzerResult.BaseClassName = baseClassName.TrimEnd();

            var baseClassfileName = $"{baseClassName}.cs";

            if (baseClassName != nameof(DefaultBuildScript) && _file.Exists(baseClassfileName))
            {
                analyzerResult.CsFiles.Add(_path.GetFullPath(baseClassfileName));
            }
        }
    }
}
