using System;
using System.Reflection;
using FlubuCore.Scripting.Analysis;

namespace FlubuCore.Scripting.Processors
{
    public class ReferenceDirectiveProcessor : IDirectiveProcessor
    {
        public bool Process(AnalyserResult analyserResult, string line, int lineIndex)
        {
            if (!line.TrimStart().StartsWith("//#ref", StringComparison.OrdinalIgnoreCase))
                return false;

            int dllIndex = line.IndexOf(" ", StringComparison.Ordinal);

            if (dllIndex < 0)
                return true;

            string dll = line.Substring(dllIndex);
            var type = Type.GetType(dll, true);
            var ass = type.GetTypeInfo().Assembly;

            analyserResult.References.Add(ass.ToAssemblyInfo());
            return true;
        }
    }
}
