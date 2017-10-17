namespace FlubuCore.Tasks.Utils
{
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
