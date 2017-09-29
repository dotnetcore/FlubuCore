using System.Collections.Generic;
using FlubuCore.Context;
using FlubuCore.Tasks.Process;

namespace FlubuCore.Tasks.Utils
{
    /// <inheritdoc cref="TaskBase{T}"/>
    /// <summary>
    /// Control windows service with sc.exe command.
    /// </summary>
    public class ServiceControlTask : TaskBase<int>, IExternalProcess<ServiceControlTask>
    {
        private readonly List<string> _arguments = new List<string>();
        private string _workingFolder;
        private bool _doNotLogOutput;

        /// <inheritdoc />
        public ServiceControlTask(string command, string serviceName)
        {
            _arguments.Add(command);
            _arguments.Add(serviceName);
        }

        /// <inheritdoc />
        protected override int DoExecute(ITaskContextInternal context)
        {
              IRunProgramTask task = context.Tasks().RunProgramTask("sc");

                if (_doNotLogOutput)
                    task.DoNotLogOutput();

                if (DoNotLog)
                    task.NoLog();

                task
                    .WithArguments(_arguments.ToArray())
                    .CaptureErrorOutput()
                    .CaptureOutput()
                    .WorkingFolder(_workingFolder)
                    .ExecuteVoid(context);

            return 0;
        }

        /// <summary>
        /// Control services on another machine.
        /// </summary>
        /// <param name="server">Machine to control. It must be in \\ServerName format.</param>
        /// <returns></returns>
        public ServiceControlTask UseServer(string server)
        {
            if (!server.StartsWith("\\\\"))
                server = $"\\\\{server.Trim()}";

            _arguments.Insert(0, server);
            return this;
        }

        /// <inheritdoc />
        public ServiceControlTask WithArguments(string arg)
        {
            _arguments.Add(arg);
            return this;
        }

        /// <inheritdoc />
        public ServiceControlTask WithArguments(params string[] args)
        {
            _arguments.AddRange(args);
            return this;
        }

        /// <inheritdoc />
        public ServiceControlTask WorkingFolder(string folder)
        {
            _workingFolder = folder;
            return this;
        }

        /// <inheritdoc />
        public ServiceControlTask DoNotLogOutput()
        {
            _doNotLogOutput = true;
            return this;
        }
    }
}
