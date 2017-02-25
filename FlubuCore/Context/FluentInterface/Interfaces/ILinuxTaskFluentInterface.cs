using FlubuCore.Tasks.Linux;

namespace FlubuCore.Context.FluentInterface.Interfaces
{
    public interface ILinuxTaskFluentInterface
    {
        /// <summary>
        /// Run's system ctl.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="service"></param>
        /// <returns></returns>
        SystemCtlTask SystemCtlTask(string command, string service);
    }
}
