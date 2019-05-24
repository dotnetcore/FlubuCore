using System;
using System.Collections.Generic;
using System.Text;
using FlubuCore.Context;
using FlubuCore.Tasks.Process;

namespace FlubuCore.Tasks.Docker
{
    public class DockerRemoveContainerTask : ExternalProcessTaskBase<int, DockerRemoveContainerTask>
    {
        private readonly string[] _containers;
        private string _description;

        public DockerRemoveContainerTask(List<string> containers)
        {
            if (containers == null || containers.Count == 0)
            {
                throw new TaskValidationException("Atleast one container must be specified in rm command.");
            }

            _containers = containers.ToArray();

            WithArguments("rm");
        }

        protected override string Description
        {
            get
            {
                if (string.IsNullOrEmpty(_description))
                {
                    return $"Executes docker command 'rm' with specified option.";
                }

                return _description;
            }

            set { _description = value; }
        }

        /// <summary>
        /// --force, -f
        /// Force the removal of a running container (uses SIGKILL)
        /// </summary>
        public DockerRemoveContainerTask Force()
        {
            WithArguments("--force");
            return this;
        }

        /// <summary>
        /// --link, -l
        /// default: false
        /// Remove the specified link
        /// </summary>
        public DockerRemoveContainerTask RemoveLink()
        {
            WithArguments("--link");
            return this;
        }

        /// <summary>
        /// --volumes, -v
        /// default: false
        /// Remove the volumes associated with the container
        /// </summary>
        public DockerRemoveContainerTask RemoveVolumes()
        {
            WithArguments("--volume");
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
