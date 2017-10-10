using System;
using System.Collections.Generic;
using System.Text;
using FlubuCore.Context;

namespace FlubuCore.Tasks.NetCore
{
    public class DotnetCleanTask : ExecuteDotnetTaskBase<DotnetCleanTask>
    {
        protected bool _projectNameIsSet;

        protected bool _configurationIsSet;

        public DotnetCleanTask() : base(StandardDotnetCommands.Clean)
        {
        }

        /// <summary>
        /// The MSBuild project file to publish. If a project file is not specified, MSBuild searches the current working directory for a file that has a file extension that ends in `proj` and uses that file.
        /// </summary>
        /// <param name="projectName"></param>
        /// <returns></returns>
        public DotnetCleanTask Project(string projectName)
        {
            WithArguments(projectName);
            _projectNameIsSet = true;
            return this;
        }

        /// <summary>
        /// Clean a specific framework.
        /// </summary>
        /// <param name="framework"></param>
        /// <returns></returns>
        public DotnetCleanTask Framework(string framework)
        {
            WithArguments("-f", framework);
            return this;
        }

        /// <summary>
        /// Clean a specific configuration.
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public DotnetCleanTask Configuration(string configuration)
        {
            WithArguments("-c", configuration);
            _configurationIsSet = true;
            return this;
        }

        protected override void BeforeExecute(ITaskContextInternal context)
        {
            if (!_projectNameIsSet)
            {
                var solustionFileName = context.Properties.Get<string>(BuildProps.SolutionFileName, null);
                if (solustionFileName != null)
                {
                    Project(solustionFileName);
                }
            }

            if (!_configurationIsSet)
            {
                var configuration = context.Properties.Get<string>(BuildProps.BuildConfiguration, null);
                if (configuration != null)
                {
                    Configuration(configuration);
                }
            }
        }
    }
}
