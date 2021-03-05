using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FlubuCore.Context;
using FlubuCore.Services;
using FlubuCore.Tasks.Process;
using FlubuCore.Tasks.Solution.VSSolutionBrowsing;

namespace FlubuCore.Tasks.Solution
{
    /// <inheritdoc />
    /// <summary>
    /// Task compiles solution with MsBuild.
    /// </summary>
    public class CompileSolutionTask : ExternalProcessTaskBase<int, CompileSolutionTask>
    {
        private readonly List<string> _msbuildPaths = new List<string>();
        private readonly List<string> _loggingOptions = new List<string>();

        private readonly IFlubuEnvironmentService _environmentService;

        private string _solutionFileName;

        private string _buildConfiguration;

        private bool _doNotSetConfiguration;

        private bool _useSolutionDirAsWorkingDir;

        private Version _toolsVersion;
        private string _description;

        /// <summary>
        /// Task compiles specified solution with MSBuild.
        /// </summary>
        /// <param name="environmentService"></param>
        public CompileSolutionTask(IFlubuEnvironmentService environmentService)
        {
            _environmentService = environmentService;
            AddAdditionalOptionPrefix("Compile");
            AddAdditionalOptionPrefix("CompileSolution");
        }

        /// <inheritdoc />
        /// <summary>
        /// Task compiles specified solution with MSBuild.
        /// </summary>
        /// <param name="buildConfiguration"></param>
        /// <param name="environmentService"></param>
        /// <param name="solutionFileName"></param>
        public CompileSolutionTask(
           string solutionFileName,
           string buildConfiguration,
           IFlubuEnvironmentService environmentService)
            : this(environmentService)
        {
            _solutionFileName = solutionFileName;
            _buildConfiguration = buildConfiguration;
            _environmentService = environmentService;
        }

        protected override string Description
        {
            get => string.IsNullOrEmpty(_description) ? "Compiles the solution." : _description;

            set => _description = value;
        }

        /// <summary>
        /// Solution filename to build.
        /// </summary>
        /// <param name="solutionFileName"></param>
        /// <returns></returns>
        public CompileSolutionTask SolutionFileName(string solutionFileName)
        {
            _solutionFileName = solutionFileName;
            return this;
        }

        /// <summary>
        /// The build configuration solution will be build.
        /// </summary>
        /// <param name="buildConfiguration"></param>
        /// <returns></returns>
        public CompileSolutionTask BuildConfiguration(string buildConfiguration)
        {
            _buildConfiguration = buildConfiguration;
            return this;
        }

        /// <summary>
        /// Add's Platform argument to MSBuild. If not set CPUAny is used as default.
        /// </summary>
        /// <param name="platform">The platform.</param>
        /// <returns></returns>
        public CompileSolutionTask Platform(string platform)
        {
            if (string.IsNullOrEmpty(platform))
                throw new ArgumentNullException(nameof(platform));

            WithArguments($"/p:Platform={platform}");
            return this;
        }

        /// <summary>
        /// Add location to msbuild. Full msbuild.exe file location must be specified. If msbuild is found at specified location msbuild wild not be searched at default locations it will use one specified here.
        /// If more than 1 path is specified first msbuild occurrence will be used. Otherwise if it is not found it will search for it in default locations.
        /// </summary>
        public CompileSolutionTask AddMsBuildPath(string pathToMsbuild)
        {
            _msbuildPaths.Add(pathToMsbuild);
            return this;
        }

        /// <summary>
        /// Configuration parameter won't be passed to msbuild command.
        /// </summary>
        /// <returns></returns>
        public CompileSolutionTask DoNotSetConfiguration()
        {
            _doNotSetConfiguration = true;
            return this;
        }

        /// <summary>
        /// Sets the max CPU variable for msbuild
        /// </summary>
        public CompileSolutionTask WithMaxCpuCount(int count)
        {
            WithArguments($"/maxcpucount:{count}");
            return this;
        }

        /// <summary>
        /// Sets the logging level of console logger. See msbuild help for more details.
        /// </summary>
        /// <param name="level">Logging level (quiet, minimal,normal, detailed, and diagnostic)</param>
        /// <returns></returns>
        public CompileSolutionTask LogLevel(string level)
        {
            _loggingOptions.Add($"Verbosity={level}");
            return this;
        }

        /// <summary>
        /// Sets the logging level of console logger. See msbuild help for more details.
        /// </summary>
        /// <param name="level">Logging level (quiet, minimal,normal, detailed, and diagnostic)</param>
        /// <returns></returns>
        public CompileSolutionTask LogLevel(StandardVerbosityLevelParameters level)
        {
            _loggingOptions.Add($"Verbosity={level.ToString().ToLower()}");
            return this;
        }

        /// <summary>
        /// Add console logger option(clp). See msbuild for more details.
        /// </summary>
        /// <param name="option">Option to set to the /clp parameter (NoSummary)</param>
        /// <returns></returns>
        public CompileSolutionTask LogOption(string option)
        {
            _loggingOptions.Add(option);
            return this;
        }

        /// <summary>
        /// Add console logger option(clp). See msbuild for more details.
        /// </summary>
        /// <param name="option">Option to set to the /clp parameter (NoSummary)</param>
        /// <returns></returns>
        public CompileSolutionTask LogOption(StandardConsoleLoggerParameters option)
        {
            _loggingOptions.Add(option.ToString());
            return this;
        }

        /// <summary>
        /// Add'sTarget argument to MSBuild.
        /// </summary>
        public CompileSolutionTask WithTarget(string target)
        {
            WithArguments($"/t:{target}");
            return this;
        }

        /// <summary>
        /// Msbuild version to be used for build.
        /// </summary>
        public CompileSolutionTask SetToolsVersion(Version version)
        {
            _toolsVersion = version;
            return this;
        }

        /// <summary>
        /// Solution directory is used for working directory.
        /// </summary>
        /// <returns></returns>
        public CompileSolutionTask UseSolutionDirectoryAsWorkingDir()
        {
            _useSolutionDirAsWorkingDir = true;
            return this;
        }

        /// <inheritdoc />
        protected override int DoExecute(ITaskContextInternal context)
        {
            ExecutablePath = FindMsBuildPath(context);
            if (string.IsNullOrEmpty(_solutionFileName))
            {
                _solutionFileName = context.Properties.Get<string>(BuildProps.SolutionFileName, null);
            }

            if (string.IsNullOrEmpty(_buildConfiguration))
            {
                _buildConfiguration = context.Properties.Get<string>(BuildProps.BuildConfiguration, null);
            }

            Validate();

            if (!_doNotSetConfiguration)
                WithArguments($"/p:Configuration={_buildConfiguration}");

            ExecuteWorkingFolder = _useSolutionDirAsWorkingDir && string.IsNullOrEmpty(ExecuteWorkingFolder)
                ? Path.GetDirectoryName(_solutionFileName)
                : ExecuteWorkingFolder ?? ".";

            InsertArgument(0, _solutionFileName);

            if (_loggingOptions.Count <= 0)
                _loggingOptions.Add("NoSummary");

            WithArguments($"/clp:{string.Join(";", _loggingOptions.ToArray())}");

            if (OutputLogLevel == Tasks.LogLevel.None)
                WithArguments("/noconlog");

            AddPrefixToAdditionalOptionKey(PrefixProcessors.AddSlashPrefixToAdditionalOptionKey);
            return base.DoExecute(context);
        }

        private string FindMsBuildPath(ITaskContextInternal context)
        {
            string msbuildPath;
            foreach (var path in _msbuildPaths)
            {
                if (File.Exists(path))
                {
                    return path;
                }
            }

            ////if (_toolsVersion != null)
            ////{
            ////    msbuildPath = ToolLocationHelper.GetPathToBuildToolsFile("msbuild.exe", _toolsVersion.ToString());
            ////    if (msbuildPath != null)
            ////    {
            ////        return msbuildPath;
            ////    }

            ////    var highestToolVersion = ToolLocationHelper.CurrentToolsVersion;
            ////    context.LogInfo(string.Format("Requested MSBuild tools version {0} not found, using a higher version {1}", _toolsVersion, highestToolVersion));
            ////    msbuildPath = ToolLocationHelper.GetPathToBuildToolsFile("msbuild.exe", highestToolVersion);
            ////    if (msbuildPath != null)
            ////    {
            ////        return msbuildPath;
            ////    }
            ////}
            ////else
            ////{
            ////    var highestToolVersion = ToolLocationHelper.CurrentToolsVersion;
            ////    context.LogInfo(string.Format("Since MSBuild tools version was not explicity specified, using the highest MSBuild tools version found ({0})", highestToolVersion));
            ////    msbuildPath = ToolLocationHelper.GetPathToBuildToolsFile("msbuild.exe", highestToolVersion);
            ////    if (msbuildPath != null)
            ////    {
            ////        return msbuildPath;
            ////    }
            ////}

            ////context.LogInfo("MsBuild not found with tool location helper. Falling back to registry locator.");

            IDictionary<Version, string> msbuilds = _environmentService.ListAvailableMSBuildToolsVersions();

            if (msbuilds == null || msbuilds.Count == 0)
                throw new TaskExecutionException("No MSBuild tools found on the system", 0);

            if (_toolsVersion != null)
            {
                if (msbuilds.TryGetValue(_toolsVersion, out msbuildPath))
                    return Path.Combine(msbuildPath, "MSBuild.exe");

                KeyValuePair<Version, string> higherVersion = msbuilds.FirstOrDefault(x => x.Key > _toolsVersion);
                if (higherVersion.Equals(default(KeyValuePair<Version, string>)))
                    throw new TaskExecutionException(string.Format("Requested MSBuild tools version {0} not found and there are no higher versions", _toolsVersion), 0);
                DoLogInfo(
                    $"Requested MSBuild tools version {_toolsVersion} not found, using a higher version {higherVersion.Key}");
                msbuildPath = higherVersion.Value;
            }
            else
            {
                KeyValuePair<Version, string> highestVersion = msbuilds.Last();
                DoLogInfo(
                    $"Since MSBuild tools version was not explicity specified, using the highest MSBuild tools version found ({highestVersion.Key})");
                msbuildPath = highestVersion.Value;
            }

            return Path.Combine(msbuildPath, "MSBuild.exe");
        }

        private void Validate()
        {
            if (string.IsNullOrEmpty(_solutionFileName))
            {
                throw new TaskExecutionException($"Solution file name is not set. Set it through constructor  or build property 'BuildProps.{nameof(BuildProps.SolutionFileName)}'.", 0);
            }

            if (string.IsNullOrEmpty(_buildConfiguration) && !_doNotSetConfiguration)
            {
                throw new TaskExecutionException($"Build configuraiton is not set. Set it through constructor or build property 'BuildProps.{nameof(BuildProps.BuildConfiguration)}'", 0);
            }
        }
    }
}
