using FlubuCore.Context.FluentInterface.Interfaces;
using FlubuCore.Tasks.Linux;

namespace FlubuCore.Context.FluentInterface
{
    public class LinuxTaskFluentInterface : ILinuxTaskFluentInterface
    {
        public TaskContext Context { get; set; }

        public SshCommandTask SshCommand(string host, string username, string password, string command)
        {
            return Context.CreateTask<SshCommandTask>(host, username, password, command);
        }

        public SshCopyTask SshCopy(string host, string username, string password)
        {
            return Context.CreateTask<SshCopyTask>(host, username, password);
        }

        public SystemCtlTask SystemCtlTask(string command, string service)
        {
            return Context.CreateTask<SystemCtlTask>(command, service);
        }
    }
}
