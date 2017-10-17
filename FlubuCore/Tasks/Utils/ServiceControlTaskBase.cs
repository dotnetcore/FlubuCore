using System;
using System.Collections.Generic;
using System.Text;
using FlubuCore.Context;

namespace FlubuCore.Tasks.Utils
{
    public abstract class ServiceControlTaskBase<TTask> : ExternalProcessTaskBase<TTask, int> where TTask : ITask
    {
        /// <inheritdoc />
        public ServiceControlTaskBase(string command, string serviceName)
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
        public TTask UseServer(string server)
        {
            if (!server.StartsWith("\\\\"))
                server = $"\\\\{server.Trim()}";

            Arguments.Insert(0, server);
            return (TTask) (object) this;
        }
    }
}
