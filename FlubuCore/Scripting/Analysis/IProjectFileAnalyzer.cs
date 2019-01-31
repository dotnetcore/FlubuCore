using System;
using System.Collections.Generic;
using System.Text;

namespace FlubuCore.Scripting.Analysis
{
    public interface IProjectFileAnalyzer
    {
        ProjectFileAnalyzerResult Analyze(string location = null, bool disableAnalysis = false);
    }
}
