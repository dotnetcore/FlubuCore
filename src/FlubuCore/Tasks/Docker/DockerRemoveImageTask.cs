using System.Collections.Generic;
using FlubuCore.Context;
using FlubuCore.Tasks.Process;

namespace FlubuCore.Tasks.Docker
{
    public class DockerRemoveImageTask : ExternalProcessTaskBase<int, DockerRemoveImageTask>
    {
        private readonly string[] _images;
        private string _description;

        public DockerRemoveImageTask(List<string> images)
        {
            if (images == null || images.Count == 0)
            {
                throw new TaskValidationException("Atleast one container must be specified in rm command.");
            }

            _images = images.ToArray();

            WithArguments("rmi");
        }

        protected override string Description
        {
            get
            {
                if (string.IsNullOrEmpty(_description))
                {
                    return $"Executes docker command 'rmi' with specified option.";
                }

                return _description;
            }

            set { _description = value; }
        }

        /// <summary>
        /// --force, -f
        /// Force removal of the image
        /// </summary>
        public DockerRemoveImageTask Force()
        {
            WithArguments("--force");
            return this;
        }

        /// <summary>
        /// --no-prune
        /// Do not delete untagged parents
        /// </summary>
        public DockerRemoveImageTask NoPrune()
        {
            WithArguments("--no-prune");
            return this;
        }

        protected override int DoExecute(ITaskContextInternal context)
        {
            ExecutablePath = "docker";
            WithArguments(_images);
            return base.DoExecute(context);
        }
    }
}