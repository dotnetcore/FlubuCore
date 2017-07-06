using FlubuCore.Scripting.Analysis;

namespace FlubuCore.Scripting.Processors
{
    public class NamespaceDirectiveProcessor : IDirectiveProcessor
    {
        public bool Process(AnalyserResult analyserResult, string line, int lineIndex)
        {
            if (line.TrimStart().StartsWith("namespace"))
            {
                analyserResult.NamespaceIndex = lineIndex;
                return false;
            }

            return false;
        }
    }
}
