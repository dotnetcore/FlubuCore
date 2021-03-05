using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using FlubuCore.Context;
using FlubuCore.Tasks.Attributes;

namespace FlubuCore.Tasks.Process
{
    public abstract class ExternalProcessTaskBase<TResult, TTask> : TaskBase<TResult, TTask>, IExternalProcess<TTask>
        where TTask : class, ITask
    {
        private readonly List<string> _additionalOptionPrefixes = new List<string>();

        private readonly Dictionary<string, List<string>> _longAndShortKeys = new Dictionary<string, List<string>>();

        private Func<string, string> _prefixToAdditionalOptionKeyFunc;

        private char _additionalOptionKeyValueSeperator;

        private IRunProgramTask _task;

        protected ExternalProcessTaskBase()
        {
            var name = GetType().Name.Replace("Task", string.Empty);
            AddAdditionalOptionPrefix(name);
        }

        // ReSharper disable once InconsistentNaming
#pragma warning disable SA1300 // Element should begin with upper-case letter
        private List<Argument> _arguments { get; } = new List<Argument>();
#pragma warning restore SA1300 // Element should begin with upper-case letter

        protected internal virtual List<string> OverridableArguments { get; set; }

        /// <summary>
        /// Gets or sets working folder.
        /// </summary>
        /// <value>
        /// Working folder.
        /// </value>
        protected string ExecuteWorkingFolder { get; set; }

        protected bool KeepProgramOutput { get; set; }

        protected bool KeepProgramErrorOutput { get; set; }

        protected virtual string KeyValueSeparator { get; } = null;

        /// <summary>
        /// Executable path.
        /// </summary>
        protected string ExecutablePath { get; set; }

        /// <summary>
        /// Sets the process log level.
        /// </summary>
        protected LogLevel OutputLogLevel { get; private set; } = LogLevel.Info;

        protected virtual string AdditionalOptionPrefix { get; set; } = "/o:";

        /// <inheritdoc />
        public TTask WithArguments(string arg, bool maskArg)
        {
            if (!string.IsNullOrEmpty(arg))
            {
                _arguments.Add(new Argument(arg, null, false, maskArg));
            }

            return this as TTask;
        }

        /// <inheritdoc />
        public TTask WithArguments(params string[] args)
        {
            foreach (var arg in args)
            {
                if (!string.IsNullOrEmpty(arg))
                {
                    _arguments.Add(new Argument(arg, null, false, false));
                }
            }

            return this as TTask;
        }

        protected internal List<string> GetArguments()
        {
            var argumentsFlat = new List<string>();
            foreach (var arg in _arguments)
            {
                argumentsFlat.Add(arg.ArgKey);

                if (!string.IsNullOrEmpty(arg.ArgValue))
                {
                    argumentsFlat.Add(arg.ArgValue);
                }
            }

            return argumentsFlat;
        }

        protected TTask InsertArgument(int index, string arg, bool maskArg = false)
        {
            if (!string.IsNullOrEmpty(arg))
            {
                 _arguments.Insert(index, new Argument(arg, null, false, maskArg));
            }

            return this as TTask;
        }

        /// <summary>
        /// Adds argument with corresponding value. eg --Framework .net462 where --Framework is key and .net462 is value.
        /// If value is null or empty task fails on execution. if key is null both argument's are ignored.
        /// </summary>
        /// <param name="argKey"></param>
        /// <param name="argValue"></param>
        /// <param name="maskValue">If <c>true</c> value is masked.</param>
        /// <returns></returns>
        protected TTask WithArgumentsValueRequired(string argKey, string argValue, bool maskValue = false)
        {
            if (!string.IsNullOrEmpty(argKey))
            {
                _arguments.Add(new Argument(argKey, argValue, true, maskValue));
            }

            return this as TTask;
        }

        protected virtual void WithArgumentsKeyFromAttribute(bool maskArg = false, [CallerMemberName]string memberName = "")
        {
            var allKeys = GetAllKeysFromAttribute(memberName);
            var firstKey = allKeys.First();
            if (!_longAndShortKeys.ContainsKey(firstKey))
            {
                _longAndShortKeys.Add(firstKey, allKeys);
            }

            _arguments.Add(new Argument(firstKey, null, false, maskArg));
        }

        protected virtual void WithArgumentsKeyFromAttribute(string value, bool maskArg = false, string separator = null, [CallerMemberName]string memberName = "")
        {
            if (separator == null)
            {
                separator = KeyValueSeparator;
            }

            var allKeys = GetAllKeysFromAttribute(memberName);
            var firstKey = allKeys.First();
            if (!_longAndShortKeys.ContainsKey(firstKey))
            {
                _longAndShortKeys.Add(firstKey, allKeys);
            }

            _arguments.Add(new Argument(firstKey, value, true, maskArg, separator));
        }

        protected string GetFirstKeyFromAttribute([CallerMemberName]string memberName = "")
        {
            var attribute = GetArgKeyAttribute(memberName);
            return attribute.Keys[0];
        }

        private ArgKeyAttribute GetArgKeyAttribute(string memberName)
        {
            var method = GetType().GetRuntimeMethods().FirstOrDefault(x => x.Name == memberName);
            if (method == null) return null;

            var attribute = method.GetCustomAttribute<ArgKeyAttribute>();
            return attribute;
        }

        protected List<string> GetAllKeysFromAttribute([CallerMemberName] string memberName = "")
        {
            var attribute = GetArgKeyAttribute(memberName);
            return attribute.Keys.ToList();
        }

        /// <inheritdoc />
        public TTask WorkingFolder(string folder)
        {
            ExecuteWorkingFolder = folder;
            return this as TTask;
        }

        /// <inheritdoc />
        [Obsolete("Use `WithOutputLogLevel(LogLevel.None)` instead.")]
        public TTask DoNotLogOutput()
        {
            OutputLogLevel = LogLevel.None;
            return this as TTask;
        }

        /// <inheritdoc />
        public TTask WithOutputLogLevel(LogLevel logLevel)
        {
            OutputLogLevel = logLevel;
            return this as TTask;
        }

        /// <inheritdoc />
        /// <summary>
        /// Clear all arguments for command line executable.
        /// </summary>
        /// <returns></returns>
        public TTask ClearArguments()
        {
            _arguments.Clear();
            return this as TTask;
        }

        /// <inheritdoc />
        /// <summary>
        /// Full file path to the executable.
        /// </summary>
        /// <param name="executableFullFilePath"></param>
        /// <returns></returns>
        public TTask Executable(string executableFullFilePath)
        {
            ExecutablePath = executableFullFilePath;
            return this as TTask;
        }

        /// <summary>
        /// Captures the output of the executable.
        /// </summary>
        public TTask CaptureOutput()
        {
            KeepProgramOutput = true;
            return this as TTask;
        }

        /// <summary>
        /// Captures the error output of the executable.
        /// </summary>
        public TTask CaptureErrorOutput()
        {
            KeepProgramErrorOutput = true;
            return this as TTask;
        }

        public TTask ChangeDefaultAdditionalOptionPrefix(string newPrefix)
        {
            AdditionalOptionPrefix = newPrefix;
            return this as TTask;
        }

        /// <summary>
        /// Get the output produced by the executable.
        /// </summary>
        /// <returns></returns>
        public virtual string GetOutput() => _task?.GetOutput();

        /// <summary>
        /// Get the error output produced by the executable.
        /// </summary>
        /// <returns></returns>
        public virtual string GetErrorOutput() => _task?.GetErrorOutput();

        /// <inheritdoc />
        protected override TResult DoExecute(ITaskContextInternal context)
        {
            if (string.IsNullOrEmpty(ExecutablePath))
            {
                throw new TaskExecutionException($"{nameof(ExecutablePath)} must be set.", 5);
            }

            _task = context.Tasks()
                          .RunProgramTask(ExecutablePath)
                          .DoNotLogTaskExecutionInfo()
                          .WorkingFolder(ExecuteWorkingFolder);

            _task.WithOutputLogLevel(OutputLogLevel);

            _task.WithLogLevel(TaskLogLevel);

            if (KeepProgramOutput)
                _task.CaptureOutput();

            if (KeepProgramErrorOutput)
                _task.CaptureErrorOutput();

            BeforeExecute(context, _task);
            AddOrOverrideArgumentsFromConsole(context);
            var argumentsFlat = ValidateAndGetArgumentsFlat();

            foreach (var arg in argumentsFlat)
            {
                _task.WithArguments(arg.arg, arg.maskArg);
            }

            var result = _task
                .ChangeDefaultAdditionalOptionPrefix(AdditionalOptionPrefix)
                .AddPrefixToAdditionalOptionKey(_prefixToAdditionalOptionKeyFunc)
                .ChangeAdditionalOptionKeyValueSeperator(_additionalOptionKeyValueSeperator)
                .AddNewAdditionalOptionPrefix(_additionalOptionPrefixes)
                .Execute(context);

            if (typeof(TResult) == typeof(int))
            {
                return (TResult)(object)result;
            }

            return default(TResult);
        }

        protected List<(string arg, bool maskArg)> ValidateAndGetArgumentsFlat()
        {
            var argumentsFlat = new List<(string arg, bool maskArg)>();

            foreach (var arg in _arguments)
            {
                if (string.IsNullOrEmpty(arg.ArgValue))
                {
                    if (arg.ValueRequired)
                    {
                        throw new TaskExecutionException($"Argument {arg.ArgKey} requires value. Task: {TaskName}", 0);
                    }

                    argumentsFlat.Add((arg.ArgKey, arg.MaskArg));
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(arg.Separator))
                    {
                        argumentsFlat.Add((arg.ArgKey, false));
                        argumentsFlat.Add((arg.ArgValue, arg.MaskArg));
                    }
                    else
                    {
                        argumentsFlat.Add(($"{arg.ArgKey}{arg.Separator}{arg.ArgValue}", arg.MaskArg));
                    }
                }
            }

            return argumentsFlat;
        }

        protected TTask AddPrefixToAdditionalOptionKey(Func<string, string> func)
        {
            _prefixToAdditionalOptionKeyFunc = func;
            return this as TTask;
        }

        protected TTask ChangeAdditionalOptionKeyValueSeperator(char newSeperator)
        {
            _additionalOptionKeyValueSeperator = newSeperator;
            return this as TTask;
        }

        protected TTask AddAdditionalOptionPrefix(string newPrefix)
        {
            if (string.IsNullOrWhiteSpace(newPrefix))
            {
                return this as TTask;
            }

            if (!newPrefix.StartsWith("/"))
            {
                newPrefix = $"/{newPrefix}";
            }

            if (!newPrefix.EndsWith(":"))
            {
                newPrefix = $"{newPrefix}:";
            }

            _additionalOptionPrefixes.Add(newPrefix);
            return this as TTask;
        }

        /// <summary>
        /// Action taken before executing external process.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="runProgramTask"></param>
        protected virtual void BeforeExecute(ITaskContextInternal context, IRunProgramTask runProgramTask)
        {
        }

        private void AddOrOverrideArgumentsFromConsole(ITaskContext context)
        {
            if (context.ScriptArgs.Count == 0)
            {
                return;
            }

            var methods = GetType().GetRuntimeMethods();
            List<string> overridableArguments = new List<string>();
            foreach (var methodInfo in methods)
            {
                var attribute = methodInfo.GetCustomAttribute<ArgKeyAttribute>();
                if (attribute == null)
                {
                    continue;
                }

                overridableArguments.AddRange(attribute.Keys);
            }

            foreach (var scriptArg in context.ScriptArgs)
            {
                var overridableArgument = overridableArguments.FirstOrDefault(x => x.EndsWith(scriptArg.Key));
                if (overridableArgument == null)
                {
                    continue;
                }

                overridableArgument = LongAndShortKeyLookup(overridableArgument);

                var argumentToOverride = _arguments.FirstOrDefault(x => x.ArgKey == overridableArgument);

                if (argumentToOverride == null)
                {
                    _arguments.Add(new Argument(overridableArgument, scriptArg.Value, false, false));
                }
                else
                {
                    argumentToOverride.ArgValue = scriptArg.Value;
                }
            }
        }

        private string LongAndShortKeyLookup(string overridableArgument)
        {
            foreach (var keys in _longAndShortKeys)
            {
                foreach (var key in keys.Value)
                {
                    if (key == overridableArgument)
                    {
                        overridableArgument = keys.Key;
                        return overridableArgument;
                    }
                }
            }

            return overridableArgument;
        }
    }
}
