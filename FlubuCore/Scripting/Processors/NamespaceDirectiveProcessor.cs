using FlubuCore.Scripting.Analysis;

namespace FlubuCore.Scripting.Processors
{
    public class NamespaceDirectiveProcessor : IDirectiveProcessor
    {
        public bool Process(ScriptAnalyzerResult analyzerResult, string line, int lineIndex)
        {
            if (line.TrimStart().StartsWith("namespace"))
            {
                analyzerResult.NamespaceIndex = lineIndex;
                return false;
            }

            return false;
        }
    }
}
