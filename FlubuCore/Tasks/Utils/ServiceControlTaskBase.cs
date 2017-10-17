using FlubuCore.Context;

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
            Arguments.Add(command);
            Arguments.Add(serviceName);
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

            Arguments.Insert(0, server);
            return this as TTask;
        }
    }
}
