using FlubuCore.Tasks.Linux;

namespace FlubuCore.Context.FluentInterface.Interfaces
{
    public interface ILinuxTaskFluentInterface
    {
        TaskContext Context { get; set; }

        SystemCtlTask SystemCtlTask(string command, string service);
    }
}
