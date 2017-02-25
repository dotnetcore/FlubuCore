using System.Collections.Generic;
using FlubuCore.Context;
using FlubuCore.Tasks.Process;

namespace FlubuCore.Tasks.NetCore
{
    public class ExecuteDotnetTask : TaskBase<int>
    {
        private readonly List<string> _arguments = new List<string>();
        private string _workingFolder;
        private string _dotnetExecutable;

        public ExecuteDotnetTask(string command)
        {
            Command = command;
        }

        public ExecuteDotnetTask(StandardDotnetCommands command)
        {
            Command = command.ToString().ToLowerInvariant();
        }

        /// <summary>
        /// Dotnet command to be executed.
        /// </summary>
        public string Command { get; private set; }

        /// <summary>
        /// Add's Argument to the dotnet see <c>Command</c>
        /// </summary>
        /// <param name="arg">Argument to be added</param>
        /// <returns></returns>
        public ExecuteDotnetTask WithArguments(string arg)
        {
            _arguments.Add(arg);
            return this;
        }

        /// <summary>
        /// Add's Arguments to the dotnet see <c>Command</c>
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public ExecuteDotnetTask WithArguments(params string[] args)
        {
            _arguments.AddRange(args);
            return this;
        }

        /// <summary>
        /// Working folder of the dotnet command
        /// </summary>
        /// <param name="folder"></param>
        /// <returns></returns>
        public ExecuteDotnetTask WorkingFolder(string folder)
        {
            _workingFolder = folder;
            return this;
        }

        /// <summary>
        /// Path to the dotnet executable.
        /// </summary>
        /// <param name="fullPath"></param>
        /// <returns></returns>
        public ExecuteDotnetTask DotnetExecutable(string fullPath)
        {
            _dotnetExecutable = fullPath;
            return this;
        }

        protected override int DoExecute(ITaskContextInternal context)
        {
            string program = _dotnetExecutable;

            if (string.IsNullOrEmpty(program))
            {
                program = context.Properties.GetDotnetExecutable();
            }

            if (string.IsNullOrEmpty(program))
            {
                context.Fail("Dotnet executable not set!", -1);
                return -1;
            }

            IRunProgramTask task = context.Tasks().RunProgramTask(program);

            task
                .WithArguments(Command)
                .WithArguments(_arguments.ToArray())
                .WorkingFolder(_workingFolder)
                .ExecuteVoid(context);

            return 0;
        }
    }
}