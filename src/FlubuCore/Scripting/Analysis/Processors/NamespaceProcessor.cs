namespace FlubuCore.Scripting.Analysis.Processors
{
    public class NamespaceProcessor : IScriptProcessor
    {
        public bool Process(ScriptAnalyzerResult analyzerResult, string line, int lineIndex)
        {
            if (line.StartsWith("namespace"))
            {
                analyzerResult.NamespaceIndex = lineIndex;
                analyzerResult.Namespace = line.Substring(9).Trim();
                return false;
            }

            return false;
        }
    }
}
