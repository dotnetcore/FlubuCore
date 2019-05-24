using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using FlubuCore.Context;
using Microsoft.DotNet.Cli.Utils;

namespace FlubuCore.Tasks.Process
{
    public class RunProgramTask : TaskBase<int, IRunProgramTask>, IRunProgramTask
    {
        private readonly List<(string arg, bool maskArg)> _arguments = new List<(string arg, bool masgArg)>();
        private readonly StringBuilder _output = new StringBuilder();
        private readonly StringBuilder _errorOutput = new StringBuilder();
        private string _programToExecute;
        private ICommandFactory _commandFactory;
        private string _workingFolder;
        private bool _captureOutput;
        private bool _captureErrorOutput;
        private bool _doNotLogOutput;
        private string _description;

        /// <inheritdoc />
        public RunProgramTask(ICommandFactory commandFactory, string programToExecute)
        {
            _commandFactory = commandFactory;
            _programToExecute = programToExecute;
        }

        protected override string Description
        {
            get
            {
                if (string.IsNullOrEmpty(_description))
                {
                    return $"Runs program '{_programToExecute}'";
                }

                return _description;
            }

            set { _description = value; }
        }

        /// <inheritdoc />
        /// <summary>
        /// Add's argument to the program.
        /// </summary>
        /// <param name="arg"></param>
        /// <param name="maskArg"></param>
        /// <returns></returns>
        public IRunProgramTask WithArguments(string arg, bool maskArg)
        {
            _arguments.Add((arg, maskArg));
            return this;
        }

        /// <summary>
        /// Add's arguments to the program.
        /// </summary>
        public IRunProgramTask WithArguments(params string[] args)
        {
            foreach (var arg in args)
            {
                _arguments.Add((arg, false));
            }

            return this;
        }

        /// <inheritdoc />
        /// <summary>
        /// Working folder of the program.
        /// </summary>
        /// <param name="folder"></param>
        /// <returns></returns>
        public IRunProgramTask WorkingFolder(string folder)
        {
            if (string.IsNullOrEmpty(folder) || folder.Equals(".", StringComparison.OrdinalIgnoreCase))
                return this;

            _workingFolder = folder;
            return this;
        }

        /// <inheritdoc />
        public IRunProgramTask CaptureOutput()
        {
            _captureOutput = true;
            return this;
        }

        /// <inheritdoc />
        public IRunProgramTask CaptureErrorOutput()
        {
            _captureErrorOutput = true;
            return this;
        }

        /// <inheritdoc />
        /// <summary>
        /// Get the output produced by executable.
        /// </summary>
        /// <returns></returns>
        public string GetOutput()
        {
            return _output.ToString();
        }

        /// <inheritdoc />
        /// <summary>
        /// Get the error output produced by executable.
        /// </summary>
        /// <returns></returns>
        public string GetErrorOutput()
        {
            return _errorOutput.ToString();
        }

        /// <inheritdoc />
        public IRunProgramTask DoNotLogOutput()
        {
            _doNotLogOutput = true;
            return this;
        }

        /// <inheritdoc />
        protected override int DoExecute(ITaskContextInternal context)
        {
            if (_commandFactory == null)
                _commandFactory = new CommandFactory();

            string currentDirectory = Directory.GetCurrentDirectory();

            FileInfo info = new FileInfo(_programToExecute);

            string cmd = _programToExecute;

            if (info.Exists)
                cmd = info.FullName;

            ICommand command = _commandFactory.Create(cmd, _arguments.Select(x => x.arg));

            command
                .CaptureStdErr()
                .CaptureStdOut()
                .WorkingDirectory(_workingFolder ?? currentDirectory)
                .OnErrorLine(l =>
                {
                    if (!_doNotLogOutput)
                        DoLogInfo(l);

                    if (_captureErrorOutput)
                        _errorOutput.AppendLine(l);
                })
                .OnOutputLine(l =>
                {
                    if (!_doNotLogOutput)
                        DoLogInfo(l);

                    if (_captureOutput)
                        _output.AppendLine(l);
                });

            string commandArgs = null;
            foreach (var arg in _arguments)
            {
                commandArgs = !arg.maskArg ? $"{commandArgs} {arg.arg}" : $"{commandArgs} ####";
            }

            DoLogInfo(
                $"Running program '{command.CommandName}':(work.dir='{_workingFolder}',args='{commandArgs}')");

            int res = command.Execute()
                .ExitCode;

            if (!DoNotFail && res != 0)
                context.Fail($"External program {cmd} failed with {res}.", res);

            return res;
        }

        public IRunProgramTask Executable(string executableFullFilePath)
        {
            _programToExecute = executableFullFilePath;
            return this;
        }

        public IRunProgramTask ClearArguments()
        {
            _arguments.Clear();
            return this;
        }
    }
}
