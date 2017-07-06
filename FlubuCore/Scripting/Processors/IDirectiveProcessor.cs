using FlubuCore.Scripting.Analysis;

namespace FlubuCore.Scripting.Processors
{
    public interface IDirectiveProcessor
    {
        bool Process(AnalyserResult analyserResult, string line, int lineIndex);
    }
}