using System;
using System.Collections.Generic;
using System.Text;

namespace FlubuCore.Scripting
{
    public interface ITargetExtractor
    {
        List<string> ExtractTargets(string scriptPath);

        List<string> ExtractTargets(List<string> scriptLines);

        string ExtractTarget(string scriptLine);
    }
}
