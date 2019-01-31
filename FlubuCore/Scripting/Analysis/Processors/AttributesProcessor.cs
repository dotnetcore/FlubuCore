using System;
using System.Collections.Generic;
using System.Text;
using FlubuCore.Scripting.Attributes;

namespace FlubuCore.Scripting.Analysis.Processors
{
    public class AttributesProcessor : IScriptProcessor
    {
        public bool Process(ScriptAnalyzerResult analyzerResult, string line, int lineIndex)
        {
            if (!line.StartsWith("["))
            {
                return false;
            }

            var endAttributeIndex = line.IndexOf(']');

            if (endAttributeIndex < 0)
            {
                return false;
            }

            var attributeName = line.Substring(1, endAttributeIndex - 1);

            if (attributeName == nameof(DisableLoadScriptReferencesAutomaticallyAttribute).Replace("Attribute", string.Empty) ||
                attributeName == nameof(DisableLoadScriptReferencesAutomaticallyAttribute))
            {
                analyzerResult.ScriptAttributes.Add(ScriptAttributes.DisableLoadScriptReferencesAutomatically);
            }

            return true;
        }
    }
}
