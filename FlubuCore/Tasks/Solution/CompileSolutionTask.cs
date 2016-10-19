using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FlubuCore.Context;
using FlubuCore.Services;
using FlubuCore.Tasks.Process;

namespace FlubuCore.Tasks.Solution
{
    /// <summary>
    /// Task compiles solution with MsBuild.
    /// </summary>
    public class CompileSolutionTask : TaskBase
    {
        private readonly IFlubuEnviromentService _enviromentService;
        private string _solutionFileName;

        private string _buildConfiguration;

        private string _target;

        private Version _toolsVersion;

        private bool _useSolutionDirAsWorkingDir;

        private int _maxCpuCount = 3;

        /// <summary>
        /// Task compiles specified solution with MSBuild.
        /// </summary>
        /// <param name="enviromentService"></param>
        public CompileSolutionTask(IFlubuEnviromentService enviromentService)
        {
            _enviromentService = enviromentService;
        }

        /// <summary>
        /// Task compiles specified solution with MSBuild.
        /// </summary>
        /// <param name="buildConfiguration"></param>
        /// <param name="enviromentService"></param>
        /// <param name="solutionFileName"></param>
        public CompileSolutionTask(
           string solutionFileName,
           string buildConfiguration,
           IFlubuEnviromentService enviromentService)
        {
           _solutionFileName = solutionFileName;
           _buildConfiguration = buildConfiguration;
            _enviromentService = enviromentService;
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
            if (string.IsNullOrEmpty(_solutionFileName))
            {
                _solutionFileName = context.Properties.Get<string>(BuildProps.SolutionFileName, null);
            }

            if (string.IsNullOrEmpty(_buildConfiguration))
            {
                _buildConfiguration = context.Properties.Get<string>(BuildProps.BuildConfiguration, null);
            }

            Validate();

            IRunProgramTask task = context.Tasks().RunProgramTask(msbuildPath);
            task
                .WithArguments(_solutionFileName)
                .WithArguments($"/p:Configuration={_buildConfiguration}")
                .WithArguments("/p:Platform=Any CPU")
                .WithArguments("/consoleloggerparameters:NoSummary")
                .WithArguments($"/maxcpucount:{_maxCpuCount}");

            if (_useSolutionDirAsWorkingDir)
                task.WorkingFolder(Path.GetDirectoryName(_solutionFileName));

            if (_target != null)
                task.WithArguments($"/t:{_target}");

            task.Execute(context);

            return 0;
        }

        private string FindMSBuildPath(ITaskContext context)
        {
            string msbuildPath;
            IDictionary<Version, string> msbuilds = _enviromentService.ListAvailableMSBuildToolsVersions();
            if (msbuilds == null || msbuilds.Count == 0)
                throw new TaskExecutionException("No MSBuild tools found on the system", 0);

            if (_toolsVersion != null)
            {
                if (!msbuilds.TryGetValue(_toolsVersion, out msbuildPath))
                {
                    KeyValuePair<Version, string> higherVersion = msbuilds.FirstOrDefault(x => x.Key > _toolsVersion);
                    if (higherVersion.Equals(default(KeyValuePair<Version, string>)))
                        throw new TaskExecutionException(string.Format("Requested MSBuild tools version {0} not found and there are no higher versions", _toolsVersion), 0);

                    context.LogInfo(string.Format("Requested MSBuild tools version {0} not found, using a higher version {1}", _toolsVersion, higherVersion.Key));
                    msbuildPath = higherVersion.Value;
                }
            }
            else
            {
                KeyValuePair<Version, string> highestVersion = msbuilds.Last();
                context.LogInfo(string.Format("Since MSBuild tools version was not explicity specified, using the highest MSBuild tools version found ({0})", highestVersion.Key));
                msbuildPath = highestVersion.Value;
            }

            return Path.Combine(msbuildPath, "MSBuild.exe");
        }

        private void Validate()
        {
            if (string.IsNullOrEmpty(_solutionFileName))
            {
                throw new TaskExecutionException("Solution file name is not set. Set it through constructor or build properties.", 0);
            }

            if (string.IsNullOrEmpty(_buildConfiguration))
            {
                throw new TaskExecutionException("Build configuraiton is not set. Set it through constructor or build properties.", 0);
            }
        }
    }
}
