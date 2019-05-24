using FlubuCore.Context;
using FlubuCore.Tasks.Process;

namespace FlubuCore.Tasks.Utils
{
    /// <summary>
    /// Base class for <see cref="ServiceControlTask" />
    /// </summary>
    /// <typeparam name="TTask"></typeparam>
    public abstract class ServiceControlTaskBase<TTask> : ExternalProcessTaskBase<int, TTask>
        where TTask : class, ITask
    {
        /// <inheritdoc />
        protected ServiceControlTaskBase(string command, string serviceName)
        {
            Command = command;
            ServiceName = serviceName;
            WithArguments(command, serviceName);
            ExecutablePath = "sc";
        }

        protected string ServiceName { get; }

        protected string Command { get; }

        /// <summary>
        /// Control services on another machine.
        /// </summary>
        /// <param name="server">Machine to control. It must be in \\ServerName format.</param>
        /// <returns></returns>
        public TTask UseServer(string server)
        {
            if (!server.StartsWith("\\\\", System.StringComparison.OrdinalIgnoreCase))
                server = $"\\\\{server.Trim()}";

            InsertArgument(0, server);
            return (TTask)(object)this;
        }

        protected override int DoExecute(ITaskContextInternal context)
        {
            Command.MustNotBeNullOrEmpty("sc command must not be empty.");
            ServiceName.MustNotBeNullOrEmpty("Service name must not be empty.");

            return base.DoExecute(context);
        }
    }
}
