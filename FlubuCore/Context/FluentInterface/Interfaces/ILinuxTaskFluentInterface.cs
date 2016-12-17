using FlubuCore.Tasks.Linux;

namespace FlubuCore.Context.FluentInterface.Interfaces
{
    public interface ILinuxTaskFluentInterface
    {
        SystemCtlTask SystemCtlTask(string command, string service);
    }
}
