using System.Collections.Generic;
using FlubuCore.Context;
using FlubuCore.Tasks.Process;
using Microsoft.DotNet.Cli.Utils;

namespace FlubuCore.Tasks.Testing
{
    public class CoverageReportTask : TaskBase<int, CoverageReportTask>
    {
        private readonly List<string> _inputFiles = new List<string>();
        private string _toolPath = "./tools/reportgenerator/ReportGenerator.exe";
        private string _workingFolder = ".";
        private string _targetFolder = "coverage";

        public CoverageReportTask(params string[] inputFiles)
        {
            _inputFiles.AddRange(inputFiles);
        }

        protected override string Description { get; set; }

        public CoverageReportTask TargetFolder(string path)
        {
            _targetFolder = path;
            return this;
        }

        protected override int DoExecute(ITaskContextInternal context)
        {
            RunProgramTask task = new RunProgramTask(new CommandFactory(), _toolPath);

            task
                .WorkingFolder(_workingFolder)
                .WithArguments($"-targetdir:{_targetFolder}", $"-reports:{string.Join(";", _inputFiles)}");

            task.ExecuteVoid(context);

            return 0;
        }
    }
}
