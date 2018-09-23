using System;
using System.Collections.Generic;
using System.Text;
using FlubuCore.Context;
using FlubuCore.Tasks.Process;

namespace FlubuCore.Tasks.Docker
{
    public class DockerStopTask : ExternalProcessTaskBase<DockerStopTask>
    {
        private readonly string[] _containers;
        private string _description;

        public DockerStopTask(List<string> containers)
        {
            if (containers == null || containers.Count == 0)
            {
                throw new TaskValidationException("Atleast one container must be specified in stop command.");
            }

            _containers = containers.ToArray();

            WithArguments("stop");
        }

        protected override string Description
        {
            get
            {
                if (string.IsNullOrEmpty(_description))
                {
                    return $"Executes docker command 'stop' with specified option.";
                }

                return _description;
            }

            set { _description = value; }
        }

        /// <summary>
        /// -t, --time int
        /// Seconds to wait for stop before killing it (default 10)
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public DockerStopTask WaitBeforeStop(int time)
        {
            WithArgumentsValueRequired("--time", time.ToString());
            return this;
        }

        protected override int DoExecute(ITaskContextInternal context)
        {
            ExecutablePath = "docker";
            WithArguments(_containers);
            return base.DoExecute(context);
        }
    }
}
