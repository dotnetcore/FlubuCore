using FlubuCore.Context;
using FlubuCore.Tasks.Process;

namespace FlubuCore.Tasks.Utils
{
    /// <summary>
    /// Base class for 
    /// </summary>
    /// <typeparam name="TTask"></typeparam>
    public abstract class ServiceControlTaskBase<TTask> : ExternalProcessTaskBase<TTask> where TTask : class, ITask
    {
        /// <inheritdoc />
        public ServiceControlTaskBase(string command, string serviceName)
        {
            WithArguments(command, serviceName);
            ExecutablePath = "sc";
        }

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
            return (TTask) (object) this;
        }
    }
}
