using System.Collections.Generic;
using System.IO;
using FlubuCore.Context;
using FlubuCore.IO;
using FlubuCore.Tasks.Process;
using FlubuCore.Tasks.Solution.VSSolutionBrowsing;

namespace FlubuCore.Tasks.Testing.Xunit
{
    public class XunitTask : TaskBase<int, XunitTask>
    {
        private string _description;

        private string _configuration;

        private string _xunitConsoleFileName;

        /// <summary>
        /// unit test working directory.
        /// </summary>
        private string _workingDirectory;

        private List<string> _projectNames;

        private ReportMode _reportMode = Xunit.ReportMode.None;

        private List<string> _xunitCommandLineOptions = new List<string>();

        public XunitTask(List<string> projectNames = null, List<string> assemblyNames = null)
        {
            _projectNames = projectNames;
            TestAssemblyFileNames = assemblyNames;
        }

        /// <summary>
        /// Gets a list of assemblies to be tested
        /// </summary>
        public List<string> TestAssemblyFileNames { get; private set; } = new List<string>();

        protected override string Description
        {
            get
            {
                if (string.IsNullOrEmpty(_description))
                {
                    return $"Executes Xunit unit tests.";
                }

                return _description;
            }

            set => _description = value;
        }

        /// <summary>
        /// Build configuration the tests fill be run under.
        /// </summary>
        /// <param name="configruation"></param>
        /// <returns></returns>
        public XunitTask Configuration(string configruation)
        {
            _configuration = configruation;
            return this;
        }

        /// <summary>
        /// Full file path to the xunit console runner. If not set task tries to get path from context build propertie <see cref="BuildProps.XUnitConsolePath"/>
        /// </summary>
        /// <param name="fullFilePath"></param>
        /// <returns></returns>
        public XunitTask XUnitConsolePath(string fullFilePath)
        {
            _xunitConsoleFileName = fullFilePath;
            return this;
        }

        public XunitTask SetWorkingDirectory(string directory)
        {
            _workingDirectory = directory;
            return this;
        }

        public XunitTask ReportMode(ReportMode reportMode)
        {
            _reportMode = reportMode;
            return this;
        }

        /// <summary>
        /// convert skipped tests into failures
        /// </summary>
        /// <returns></returns>
        public XunitTask FailSkips()
        {
            _xunitCommandLineOptions.Add("-failskips");
            return this;
        }

        /// <summary>
        /// stop on first test failure
        /// </summary>
        /// <returns></returns>
        public XunitTask StopOnFail()
        {
            _xunitCommandLineOptions.Add("-stoponfail");
            return this;
        }

        /// <summary>
        /// only run tests with matching name/value traits
        /// if specified more than once, acts as an OR operation
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public XunitTask Trait(string name, string value)
        {
            _xunitCommandLineOptions.Add($"-trait {name}={value}");
            return this;
        }

        /// <summary>
        /// do not run tests with matching name/value traits
        /// if specified more than once, acts as an AND operation
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public XunitTask NoTrait(string name, string value)
        {
            _xunitCommandLineOptions.Add($"-notrait {name}={value}");
            return this;
        }

        /// <summary>
        /// Result formats: (optional, choose one or more)
        /// </summary>
        /// <param name="format"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public XunitTask AddResultFormat(ResultFormat format, string fileName)
        {
            switch (format)
            {
                case ResultFormat.Xml:
                    _xunitCommandLineOptions.Add($"-xml {fileName}");
                    break;
                case ResultFormat.XmlV1:
                    _xunitCommandLineOptions.Add($"-xmlv1 {fileName}");
                    break;
                case ResultFormat.Html:
                    _xunitCommandLineOptions.Add($"-html {fileName}");
                    break;
                case ResultFormat.Nunit:
                    _xunitCommandLineOptions.Add($"-nunit {fileName}");
                    break;
            }

            return this;
        }

        /// <summary>
        /// Add custom xunit option. For example '-noshadow'. See xunit console runner for available options.
        /// </summary>
        /// <param name="option"></param>
        /// <returns></returns>
        public XunitTask AddXUnitOption(string option)
        {
            _xunitCommandLineOptions.Add(option);
            return this;
        }

        protected override int DoExecute(ITaskContextInternal context)
        {
            if (_xunitConsoleFileName == null)
            {
                _xunitConsoleFileName = context.Properties.TryGet<string>(BuildProps.XUnitConsolePath, null);

                if (_xunitConsoleFileName == null)
                {
                    throw new TaskExecutionException(
                        $"Build configuration must be set. Set it through context property {nameof(BuildProps.XUnitConsolePath)} or task method Configuration.",
                        0);
                }
            }

            IRunProgramTask task = context.Tasks().RunProgramTask(_xunitConsoleFileName);
            SetAssemblyFileNameAndWorkingDirFromProjectName(context);

            task.WorkingFolder(_workingDirectory);

            foreach (var testAssemblyFileName in TestAssemblyFileNames)
            {
                task.WithArguments(testAssemblyFileName);
            }

            switch (_reportMode)
            {
                case Xunit.ReportMode.AppVeyor:
                    task.WithArguments("-appveyor");
                    break;
                case Xunit.ReportMode.Json:
                    task.WithArguments("-json");
                    break;
                case Xunit.ReportMode.Quiet:
                    task.WithArguments("-quiet");
                    break;
                case Xunit.ReportMode.TeamCity:
                    task.WithArguments("-teamcity");
                    break;
                case Xunit.ReportMode.Verbose:
                    task.WithArguments("-verbose");
                    break;
            }

            foreach (var xunitCommandLineOption in _xunitCommandLineOptions)
            {
                task.WithArguments(xunitCommandLineOption);
            }

            task.AddPrefixToAdditionalOptionKey(PrefixProcessors.AddSingleDashPrefixToAdditionalOptionKey);
            task.AddNewAdditionalOptionPrefix("Xunit");
            return task.Execute(context);
        }

        private void SetAssemblyFileNameAndWorkingDirFromProjectName(ITaskContextInternal context)
        {
            if (_projectNames != null)
            {
                bool setWorkingDir = false;
                if (TestAssemblyFileNames == null)
                {
                    TestAssemblyFileNames = new List<string>();
                    if (_projectNames.Count == 1 && string.IsNullOrEmpty(_workingDirectory))
                    {
                        setWorkingDir = true;
                    }
                }

                VSSolution solution = GetRequiredVSSolution();

                string buildConfiguration;
                if (string.IsNullOrEmpty(_configuration))
                {
                    buildConfiguration = context.Properties.TryGet<string>(BuildProps.BuildConfiguration);
                    if (buildConfiguration == null)
                    {
                        throw new TaskExecutionException(
                            "Build configuration must be set. Set it through context property BuildConfiguration or task method Configuration.",
                            0);
                    }
                }
                else
                {
                    buildConfiguration = _configuration;
                }

                foreach (var projectName in _projectNames)
                {
                    VSProject project = (VSProject)solution.FindProjectByName(projectName);
                    FileFullPath projectTarget = project.ProjectDirectoryPath
                        .CombineWith(project.GetProjectOutputPath(buildConfiguration))
                        .AddFileName("{0}.dll", project.ProjectName);

                    TestAssemblyFileNames.Add(projectTarget.ToString());
                    if (setWorkingDir)
                    {
                        _workingDirectory = Path.GetDirectoryName(projectTarget.ToString());
                    }
                }
            }
        }
    }
}
