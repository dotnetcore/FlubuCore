using System;
using System.Collections.Generic;
using System.Text;
using FlubuCore.Context;
using FlubuCore.Tasks.Process;

namespace FlubuCore.Tasks.Testing
{
    public class OpenCoverTask : TaskBase
    {
        private readonly List<string> _includeList = new List<string>();
        private readonly List<string> _excludeList = new List<string>();
        private string _openCoverPath = "tools/opencover/OpenCover.Console.exe";
        private string _testExecutable = "C:/Program Files/dotnet/dotnet.exe";
        private string _testExecutableArgs = "test";
        private string _output = "coverage.xml";
        private string _workingFolder = ".";

        public override string Description => "Execute OpenCover";

        public OpenCoverTask WorkingFolder(string path)
        {
            _workingFolder = path;
            return this;
        }

        public OpenCoverTask OpenCoverPath(string path)
        {
            _openCoverPath = path;
            return this;
        }

        public OpenCoverTask TestExecutable(string fullPath)
        {
            _testExecutable = fullPath;
            return this;
        }

        public OpenCoverTask TestExecutableArgs(string args)
        {
            _testExecutable = args;
            return this;
        }

        public OpenCoverTask Output(string fullPath)
        {
            _output = fullPath;
            return this;
        }

        public OpenCoverTask AddInclude(params string[] args)
        {
            if (args == null)
                return this;

            _includeList.AddRange(args);
            return this;
        }

        public OpenCoverTask AddExclude(params string[] args)
        {
            if (args == null)
                return this;

            _excludeList.AddRange(args);
            return this;
        }

        public OpenCoverTask UseDotNet(string executablePath = null)
        {
            // todo find dotnet executable
            _testExecutable = executablePath ?? "C:/Program Files/dotnet/dotnet.exe";
            _testExecutableArgs = "test";
            return this;
        }

        protected override int DoExecute(ITaskContext context)
        {
            RunProgramTask task = new RunProgramTask(_openCoverPath);

            task
                .WorkingFolder(_workingFolder)
                .WithArguments(
                    $"-target:{_testExecutable}",
                    "-register:user",
                    "-oldstyle",
                    $"-output:{_output}");

            if (!string.IsNullOrEmpty(_testExecutableArgs))
                task.WithArguments($"-targetargs:{_testExecutableArgs}");

            string filter = PrepareFilter();

            if (!string.IsNullOrEmpty(filter))
                task.WithArguments($"-filter:\"{filter}\"");

            return task.Execute(context);
        }

        private string PrepareFilter()
        {
            if (_includeList.Count <= 0 && _excludeList.Count <= 0)
                return null;

            StringBuilder b = new StringBuilder();

            foreach (string s in _includeList)
            {
                b.Append($"+{s} ");
            }

            foreach (string s in _excludeList)
            {
                b.Append($"-{s} ");
            }

            return b.ToString();
        }
    }
}
