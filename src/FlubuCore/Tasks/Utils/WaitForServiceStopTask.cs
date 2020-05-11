using System;
using System.Text.RegularExpressions;
using System.Threading;
using FlubuCore.Context;

namespace FlubuCore.Tasks.Utils
{
    /// <summary>
    /// Query windows service status with sc.exe command.
    /// </summary>
    public class WaitForServiceStopTask : ServiceStatusTask
    {
        private string _description;
        private int _timeout = 5;

        public WaitForServiceStopTask(string serviceName)
            : base(serviceName)
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

        public WaitForServiceStopTask Timeout(int timeoutInSec)
        {
            _timeout = timeoutInSec;
            return this;
        }

        protected override int DoExecute(ITaskContextInternal context)
        {
            int time = 0;

            while (true)
            {
                base.DoExecute(context);
                var status = context.Properties.Get<ServiceStatus>($"{ServiceName}.status");
                if (status == ServiceStatus.Stopped)
                {
                    return 0;
                }

                if (time >= _timeout)
                {
                    return 1;
                }

                Thread.Sleep(TimeSpan.FromSeconds(2));
                time += 2;
            }
        }
    }
}