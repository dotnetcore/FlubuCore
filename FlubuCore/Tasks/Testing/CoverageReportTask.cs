using System.Collections.Generic;
using FlubuCore.Context;
using FlubuCore.Tasks.Process;

namespace FlubuCore.Tasks.Testing
{
    public class CoverageReportTask : TaskBase
    {
        private readonly List<string> _inputFiles = new List<string>();
        private string _toolPath = "tools/reportgenerator/ReportGenerator.exe";
        private string _workingFolder = ".";
        private string _targetFolder = "coverage";

        public CoverageReportTask(params string[] inputFiles)
        {
            _inputFiles.AddRange(inputFiles);
        }

        public CoverageReportTask TargetFolder(string path)
        {
            _targetFolder = path;
            return this;
        }

        protected override int DoExecute(ITaskContext context)
        {
            RunProgramTask task = new RunProgramTask(_toolPath);

            task
                .WorkingFolder(_workingFolder)
                .WithArguments($"-targetdir:{_targetFolder}", $"-reports:{string.Join(";", _inputFiles)}");

            return task.Execute(context);
        }
    }
}
