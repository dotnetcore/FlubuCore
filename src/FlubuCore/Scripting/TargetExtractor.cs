using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FlubuCore.Scripting
{
    public class TargetExtractor : ITargetExtractor
    {
        public List<string> ExtractTargets(string scriptPath)
        {
            if (!File.Exists(scriptPath))
            {
                return null;
            }

            var scriptLines = File.ReadAllLines(scriptPath).ToList();

            return ExtractTargets(scriptLines);
        }

        public List<string> ExtractTargets(List<string> scriptLines)
        {
            List<string> targetNames = new List<string>();

            foreach (var scriptLine in scriptLines)
            {
                string targetName = ExtractTarget(scriptLine);

                if (targetName != null)
                {
                    targetNames.Add(targetName);
                }
            }

            return targetNames;
        }

        public string ExtractTarget(string scriptLine)
        {
            if (string.IsNullOrEmpty(scriptLine))
            {
                return null;
            }

            string search = "CreateTarget(\"";
            var index = scriptLine.IndexOf(search, StringComparison.Ordinal);

            if (index < 0)
            {
                return null;
            }

            string targetName = scriptLine.Substring(index + search.Length);
            targetName = targetName.Substring(0, targetName.IndexOf("\"", StringComparison.Ordinal));

            return targetName;
        }
    }
}
