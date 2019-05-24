namespace FlubuCore.Tasks.Utils
{
    /// <summary>
    /// Control windows service with sc.exe command.
    /// </summary>
    public class ServiceControlTask : ServiceControlTaskBase<ServiceControlTask>
    {
        private string _description;

        public ServiceControlTask(string command, string serviceName)
            : base(command, serviceName)
        {
        }

        protected override string Description
        {
            get
            {
                if (string.IsNullOrEmpty(_description))
                {
                    return $"{Command} service '{ServiceName}'";
                }

                return _description;
            }

            set { _description = value; }
        }
    }
}
