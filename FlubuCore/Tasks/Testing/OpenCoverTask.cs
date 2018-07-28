using System;
using System.Collections.Generic;
using System.Text;
using FlubuCore.Context;
using FlubuCore.Tasks.NetCore;
using FlubuCore.Tasks.Process;
using Microsoft.DotNet.Cli.Utils;

namespace FlubuCore.Tasks.Testing
{
    public class OpenCoverTask : TaskBase<int, OpenCoverTask>
    {
        private readonly List<string> _includeList = new List<string>();
        private readonly List<string> _excludeList = new List<string>();
        private string _openCoverPath = "./tools/opencover/OpenCover.Console.exe";
        private string _testExecutable;
        private string _testExecutableArgs;
        private string _output = "coverage.xml";
        private string _workingFolder;

        private UnitTestProvider _provider;

        protected override string Description { get; set; }

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
            _testExecutableArgs = args;
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

        public OpenCoverTask IncludeAll()
        {
            _includeList.Add("[*]*");
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
            _testExecutable = executablePath;

            if (string.IsNullOrEmpty(_testExecutableArgs))
            {
                _testExecutableArgs = "test";
            }

            _provider = UnitTestProvider.DotnetCore;
            return this;
        }

        protected override int DoExecute(ITaskContextInternal context)
        {
            RunProgramTask task = new RunProgramTask(new CommandFactory(), _openCoverPath);

            string testExecutable = GetExecutable();

            task
                .WithArguments(
                    $"-target:{testExecutable}",
                    "-register:user",
                    "-oldstyle",
                    "-returntargetcode",
                    $"-output:{_output}");

            if (!string.IsNullOrEmpty(_workingFolder))
                task.WorkingFolder(_workingFolder);

            if (!string.IsNullOrEmpty(_testExecutableArgs))
                task.WithArguments($"-targetargs:{_testExecutableArgs}");

            string filter = PrepareFilter();

            if (!string.IsNullOrEmpty(filter))
                task.WithArguments($"-filter:{filter}");

            task.ExecuteVoid(context);

            return 0;
        }

        private string GetExecutable()
        {
            if (!string.IsNullOrEmpty(_testExecutable))
            {
                return _testExecutable;
            }

            switch (_provider)
            {
                case UnitTestProvider.DotnetCore:
                    return ExecuteDotnetTask.FindDotnetExecutable();
                default:
                    throw new NotSupportedException($"Provider {_provider} not supported yet. Set test executable manually.");
            }
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

            return b.ToString().Trim();
        }
    }
}
