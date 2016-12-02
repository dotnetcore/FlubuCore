using FlubuCore.Context.FluentInterface.Interfaces;
using FlubuCore.Tasks.Linux;

namespace FlubuCore.Context.FluentInterface
{
    public class LinuxTaskFluentInterface : ILinuxTaskFluentInterface
    {
        public TaskContext Context { get; set; }

        public SystemCtlTask SystemCtlTask(string command, string service)
        {
            return Context.CreateTask<SystemCtlTask>(command, service);
        }
    }
}
