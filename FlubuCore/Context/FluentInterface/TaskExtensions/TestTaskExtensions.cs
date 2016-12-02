using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FlubuCore.Context.FluentInterface.Interfaces;
using FlubuCore.Targeting;
using FlubuCore.Tasks.NetCore;
using FlubuCore.Tasks.Testing;

namespace FlubuCore.Context.FluentInterface.TaskExtensions
{
    public partial class TaskExtensionsFluentInterface
    {
        public ITaskExtensionsFluentInterface DotnetUnitTest(params string[] projects)
        {
            foreach (string project in projects)
            {
                Target.Target.AddTask(Dotnet.Test(project));
            }

            return this;
        }

        public ITaskExtensionsFluentInterface DotnetCoverage(params string[] projects)
        {
            foreach (string project in projects)
            {
                Target.Target.AddTask(DotnetCoverage(project, null, null, new string[1]));
            }

            return this;
        }

        public ITaskExtensionsFluentInterface DotnetCoverage(string projectPath, string output, params string[] excludeList)
        {
            Target.Target.AddTask(DotnetCoverage(projectPath, output, null, excludeList));
            return this;
        }

        public ITaskExtensionsFluentInterface DotnetCoverage(string projectPath, string[] includeList, string[] excludeList)
        {
            Target.Target.AddTask(DotnetCoverage(projectPath, null, includeList, excludeList));
            return this;
        }

        private OpenCoverTask DotnetCoverage(string projectPath, string output, string[] includeList, string[] excludeList)
        {
            if (string.IsNullOrEmpty(output))
                output = $"{Path.GetFileNameWithoutExtension(projectPath)}cover.xml";

            OpenCoverTask task = Context.Tasks().OpenCoverTask()
                .TestExecutableArgs($"test {projectPath}")
                .Output(output)
                .UseDotNet()
                .AddExclude(excludeList);

            if (includeList == null || includeList.Length <= 0)
                task.IncludeAll();
            else
                task.AddInclude(includeList);

            return task;
        }
    }
}
