using System;
using System.Collections.Generic;
using System.Text;
using FlubuCore.Context;
using FlubuCore.Tasks.Process;

namespace FlubuCore.Tasks.Docker
{
    public class DockerBuildTask : ExternalProcessTaskBase<DockerBuildTask>
    {
        private readonly string _pathOrUrl;

        public DockerBuildTask(string pathOrUrl)
        {
            WithArguments("build");
            _pathOrUrl = pathOrUrl;
        }

        protected override string Description { get; set; }

        /// <summary>
        /// Image name and optionally a tag in the 'name:tag' format.
        /// </summary>
        /// <param name="nameAndTag"></param>
        /// <returns></returns>
        public DockerBuildTask ImageNameAndTag(string nameAndTag)
        {
            WithArgumentsValueRequired("--tag", nameAndTag);
            return this;
        }

        public DockerBuildTask UseDockerFile(string pathToDockerFile)
        {
            WithArgumentsValueRequired("--file", pathToDockerFile);
            return this;
        }

        protected override int DoExecute(ITaskContextInternal context)
        {
            _pathOrUrl.MustNotBeNullOrEmpty("PATH | URL must not be empty. run 'docker build --help' or see docker documentation online for help.");
            WithArguments(_pathOrUrl);
            return base.DoExecute(context);
        }
    }
}
