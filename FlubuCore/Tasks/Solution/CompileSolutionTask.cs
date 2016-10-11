using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FlubuCore.Context;
using FlubuCore.Services;
using FlubuCore.Tasks.Process;

namespace FlubuCore.Tasks.Solution
{
    public class CompileSolutionTask : TaskBase
    {
        private readonly IComponentProvider _componentProvider;

        private readonly string _solutionFileName;

        private readonly string _buildConfiguration;

        private string _target;

        private Version _toolsVersion;

        private bool _useSolutionDirAsWorkingDir;

        private int _maxCpuCount = 3;

        public CompileSolutionTask(
           string solutionFileName,
           string buildConfiguration,
           IComponentProvider componentProvider)
        {
           _solutionFileName = solutionFileName;
           _buildConfiguration = buildConfiguration;
           _componentProvider = componentProvider;
        }

        public int MaxCpuCount
        {
            get { return _maxCpuCount; }
            set { _maxCpuCount = value; }
        }

        public string Target
        {
            get { return _target; }
            set { _target = value; }
        }

        public Version ToolsVersion
        {
            get { return _toolsVersion; }
            set { _toolsVersion = value; }
        }

        public bool UseSolutionDirAsWorkingDir
        {
            get { return _useSolutionDirAsWorkingDir; }
            set { _useSolutionDirAsWorkingDir = value; }
        }

        protected override int DoExecute(ITaskContext context)
        {
            string msbuildPath = FindMSBuildPath(context);

            IRunProgramTask task = _componentProvider.CreateRunProgramTask(msbuildPath);
            task
                .WithArguments(_solutionFileName)
                .WithArguments("/p:Configuration={0}", _buildConfiguration)
                .WithArguments("/p:Platform=Any CPU")
                .WithArguments("/consoleloggerparameters:NoSummary")
                .WithArguments("/maxcpucount:{0}", _maxCpuCount.ToString());

            if (_useSolutionDirAsWorkingDir)
                task.WorkingFolder(Path.GetDirectoryName(_solutionFileName));

            if (_target != null)
                task.WithArguments("/t:{0}", _target);

            task.Execute(context);

            return 0;
        }

        private string FindMSBuildPath(ITaskContext context)
        {
            string msbuildPath;
            var flubuEnvironmentService = _componentProvider.CreateFlubuEnviromentService();
            IDictionary<Version, string> msbuilds = flubuEnvironmentService.ListAvailableMSBuildToolsVersions();
            if (msbuilds == null || msbuilds.Count == 0)
                throw new TaskExecutionException("No MSBuild tools found on the system", 0);

            if (_toolsVersion != null)
            {
                if (!msbuilds.TryGetValue(_toolsVersion, out msbuildPath))
                {
                    KeyValuePair<Version, string> higherVersion = msbuilds.FirstOrDefault(x => x.Key > _toolsVersion);
                    if (higherVersion.Equals(default(KeyValuePair<Version, string>)))
                        throw new TaskExecutionException(string.Format("Requested MSBuild tools version {0} not found and there are no higher versions", _toolsVersion), 0);

                    context.WriteMessage(string.Format("Requested MSBuild tools version {0} not found, using a higher version {1}", _toolsVersion, higherVersion.Key));
                    msbuildPath = higherVersion.Value;
                }
            }
            else
            {
                KeyValuePair<Version, string> highestVersion = msbuilds.Last();
                context.WriteMessage(string.Format("Since MSBuild tools version was not explicity specified, using the highest MSBuild tools version found ({0})", highestVersion.Key));
                msbuildPath = highestVersion.Value;
            }

            return Path.Combine(msbuildPath, "MSBuild.exe");
        }
    }
}
