using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using FlubuCore.Context;
using FlubuCore.Tasks.NetCore;
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

        private LogLevel _outputLogLevel = LogLevel.Info;

        private string _description;

        private string _additionalOptionPrefix = "/o:";

        private List<string> _additionalOptionPrefixes = new List<string>();

        private char? _additionalOptionKeyValueSeperator = null;

        private Func<string, string> _addPrefixToAdditionalOptionKey = null;

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
        [Obsolete("Use `WithOutputLogLevel(LogLevel.None)` instead.")]
        public IRunProgramTask DoNotLogOutput()
        {
            _outputLogLevel = LogLevel.None;
            return this;
        }

        /// <inheritdoc />
        public IRunProgramTask WithOutputLogLevel(LogLevel logLevel)
        {
            _outputLogLevel = logLevel;
            return this;
        }

        public IRunProgramTask AddNewAdditionalOptionPrefix(string newPrefix)
        {
            _additionalOptionPrefixes.Add(newPrefix);
            return this;
        }

        public IRunProgramTask AddNewAdditionalOptionPrefix(List<string> newPrefixes)
        {
            _additionalOptionPrefixes.AddRange(newPrefixes);
            return this;
        }

        public IRunProgramTask ChangeDefaultAdditionalOptionPrefix(string newPrefix)
        {
            if (!string.IsNullOrEmpty(newPrefix))
            {
                _additionalOptionPrefix = newPrefix;
            }

            return this;
        }

        public IRunProgramTask ChangeAdditionalOptionKeyValueSeperator(char newSeperator)
        {
            _additionalOptionKeyValueSeperator = newSeperator;
            return this;
        }

        public IRunProgramTask AddPrefixToAdditionalOptionKey(Func<string, string> action)
        {
            _addPrefixToAdditionalOptionKey = action;
            return this;
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

        /// <inheritdoc />
        protected override int DoExecute(ITaskContextInternal context)
        {
            if (_commandFactory == null)
                _commandFactory = new CommandFactory();

            string rootDir = context.Properties.TryGet(BuildProps.ProductRootDir, defaultValue: Path.GetFullPath("."));

            FileInfo info = new FileInfo(_programToExecute);

            if (_programToExecute.Equals("dotnet", StringComparison.OrdinalIgnoreCase) ||
                _programToExecute.Equals("dotnet.exe", StringComparison.OrdinalIgnoreCase))
            {
                _programToExecute = ExecuteDotnetTask.FindDotnetExecutable();
            }

            string cmd = _programToExecute;

            if (info.Exists)
                cmd = info.FullName;

            ICommand command = _commandFactory.Create(cmd, _arguments.Select(x => x.arg));
            string workingFolder = _workingFolder ?? rootDir;
            command
                .CaptureStdErr()
                .CaptureStdOut()
                .WorkingDirectory(workingFolder)
                .OnErrorLine(l =>
                {
                    if (_outputLogLevel >= LogLevel.Error)
                        DoLogInfo(l);

                    if (_captureErrorOutput)
                        _errorOutput.AppendLine(l);
                })
                .OnOutputLine(l =>
                {
                    if (_outputLogLevel >= LogLevel.Info)
                        DoLogInfo(l);

                    if (_captureOutput)
                        _output.AppendLine(l);
                });

            string commandArgs = null;
            ProcessAdditionalOptions(context);

            foreach (var arg in _arguments)
            {
                commandArgs = !arg.maskArg ? $"{commandArgs} {arg.arg}" : $"{commandArgs} ####";
            }

            DoLogInfo($"Running program from '{Path.GetFullPath(workingFolder)}':");
            DoLogInfo($"{cmd}{commandArgs}{Environment.NewLine}", Color.DarkCyan);

            int res = command.Execute()
                .ExitCode;

            if (!DoNotFail && res != 0)
                context.Fail($"External program {cmd} failed with {res}.", res);

            return res;
        }

        private void ProcessAdditionalOptions(ITaskContextInternal context)
        {
            _additionalOptionPrefixes.Add(_additionalOptionPrefix);
            foreach (var additionalOption in context.Args.AdditionalOptions)
            {
                foreach (var additionalOptionPrefix in _additionalOptionPrefixes)
                {
                    if (additionalOption.StartsWith(additionalOptionPrefix, StringComparison.OrdinalIgnoreCase))
                    {
                        var option = additionalOption.Remove(0, additionalOptionPrefix.Length);
                        var splitOption = option.Split('=');
                        option = $"{_addPrefixToAdditionalOptionKey?.Invoke(splitOption[0])}={splitOption[1]}";

                        if (_additionalOptionKeyValueSeperator.HasValue)
                        {
                            option = option.Replace('=', _additionalOptionKeyValueSeperator.Value);
                        }

                        _arguments.Add((option, false));
                        break;
                    }
                }
            }
        }
    }
}
