using System;
using System.Text.RegularExpressions;
using FlubuCore.Context;

namespace FlubuCore.Tasks.Utils
{
    /// <summary>
    /// Query windows service status with sc.exe command.
    /// </summary>
    public class ServiceStatusTask : ServiceControlTaskBase<ServiceControlTask>
    {
        private string _description;
        private Regex _state = new Regex("(STATE\\s*:\\s)(\\d)", RegexOptions.Compiled);

        public ServiceStatusTask(string serviceName)
            : base("queryex", serviceName)
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

        protected override int DoExecute(ITaskContextInternal context)
        {
            Command.MustNotBeNullOrEmpty("sc command must not be empty.");
            ServiceName.MustNotBeNullOrEmpty("Service name must not be empty.");
            CaptureOutput();
            var res = base.DoExecute(context);
            var output = GetOutput();
            var status = GetStatus(output);
            context.Properties.Set($"{ServiceName}.status", status);
            return res;
        }

        private ServiceStatus GetStatus(string output)
        {
            if (string.IsNullOrEmpty(output))
                return ServiceStatus.Unknown;

            var match = _state.Match(output);

            if (!match.Success)
                return ServiceStatus.Unknown;

            var status = match.Groups[2];
            if (!Enum.TryParse<ServiceStatus>(status.Value, true, out var parsed))
                return ServiceStatus.Unknown;

            return parsed;
        }
    }
}
