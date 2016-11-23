using FlubuCore.Tasks.Linux;

namespace FlubuCore.Context.FluentInterface
{
    public interface ILinuxTaskFluentInterface
    {
        TaskContext Context { get; set; }

        SystemCtlTask SystemCtlTask(string command, string service);
    }
}
