using System.Collections.Generic;
using FlubuCore.Context;
using FlubuCore.Tasks.Process;

namespace FlubuCore.Tasks.Utils
{
    /// <inheritdoc cref="TaskBase{T}"/>
    /// <summary>
    /// Control windows service with sc.exe command.
    /// </summary>
    public class ServiceControlTask : ServiceControlTaskBase<ServiceControlTask>
    {
        public ServiceControlTask(string command, string serviceName) : base(command, serviceName)
        {
        }
    }
}
