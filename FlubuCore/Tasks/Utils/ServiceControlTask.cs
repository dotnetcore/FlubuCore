using System.Collections.Generic;
using FlubuCore.Context;
using FlubuCore.Tasks.Process;

namespace FlubuCore.Tasks.Utils
{
    /// <inheritdoc cref="TaskBase{T}"/>
    /// <summary>
    /// Control windows service with sc.exe command.
    /// </summary>
    public class ServiceControlTask : ExternalProcessTaskBase<ServiceControlTask, int>
    {
        /// <inheritdoc />
        public ServiceControlTask(string command, string serviceName)
        {
            Arguments.Add(command);
            Arguments.Add(serviceName);
        }

        /// <inheritdoc />
        protected override int DoExecute(ITaskContextInternal context)
        {
            var task = DoExecuteExternalProcessBase(context, "sc");

            task.ExecuteVoid(context);

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

            Arguments.Insert(0, server);
            return this;
        }
    }
}
