namespace FlubuCore.Tasks.Utils
{
    /// <summary>
    /// Creates a service entry in the registry and Service Database.
    /// </summary>
    public class ServiceCreateTask : ServiceControlTaskBase<ServiceCreateTask>
    {
        private readonly string _pathToService;
        private string _description;

        public ServiceCreateTask(string serviceName, string pathToService)
            : base(StandardServiceControlCommands.Create.ToString(), serviceName)
        {
            _pathToService = pathToService;
            WithArguments($"binPath={pathToService}");
        }

        protected override string Description
        {
            get
            {
                if (string.IsNullOrEmpty(_description))
                {
                    return $"Creates new windows service '{ServiceName}' located at '{_pathToService}";
                }

                return _description;
            }

            set { _description = value; }
        }

        /// <summary>
        /// Set start mode of the service.
        /// </summary>
        /// <param name="start"></param>
        /// <returns></returns>
        public ServiceCreateTask StartMode(ServiceStartMode start)
        {
            string arg;
            switch (start)
            {
                case ServiceStartMode.DelayedAuto:
                {
                    arg = "Delayed-Auto";
                    break;
                }

                default:
                {
                    arg = start.ToString();
                    break;
                }
            }

            WithArguments($"start={arg}");
            return this;
        }
    }
}
