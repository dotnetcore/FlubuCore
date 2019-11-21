using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using FlubuCore.Context;
using FlubuCore.IO;
using FlubuCore.Tasks.Process;
using FlubuCore.Tasks.Solution.VSSolutionBrowsing;
using Microsoft.DotNet.Cli.Utils;

namespace FlubuCore.Tasks.Testing
{
    /// <inheritdoc />
    /// <summary>
    /// Run NUnit tests with NUnit console runner.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here.")]
    public class NUnitTask : TaskBase<int, NUnitTask>
    {
        private readonly List<string> _nunitCommandLineOptions = new List<string>();

        private readonly List<string> _projectNames;

        private string _nunitConsoleFileName;

        private string _configuration;

        /// <summary>
        /// unit test working directory.
        /// </summary>
        private string _workingDirectory;

        /// <summary>
        ///  test categories that will be included/excluded in tests.
        /// </summary>
        private string _categories;

        /// <summary>
        /// .NET framework NUnit console should run under.
        /// </summary>
        private string _targetFramework;

        private string _description;

        /// <summary>
        /// Initializes a new instance of the <see cref="NUnitTask"/> class.
        /// </summary>
        /// <param name="nunitConsoleFileName">full file path to nunit console runner</param>
        /// <param name="projectNames">Unit test project name.</param>
        public NUnitTask(List<string> projectNames = null, string nunitConsoleFileName = null)
        {
            _nunitConsoleFileName = nunitConsoleFileName;
            _projectNames = projectNames;
        }

        protected override string Description
        {
            get
            {
                if (string.IsNullOrEmpty(_description))
                {
                    return $"Executes NUnit unit tests. Assemblyes:'{string.Join(",", TestAssemblyFileNames)}', Projects:{string.Join(",", _projectNames)}";
                }

                return _description;
            }

            set => _description = value;
        }

        /// <summary>
        /// Gets a list of assemblies to be tested
        /// </summary>
        public List<string> TestAssemblyFileNames { get; private set; } = new List<string>();

        /// <summary>
        /// Initializes NunitTask with default command line options for nunit V2.
        /// </summary>
        /// <param name="projectName">Unit test project name.</param>
        /// <returns>New instance of nunit task</returns>
        public static NUnitTask ForNunitV2(params string[] projectName)
        {
            var task = new NUnitTask(projectName.ToList());
            task.AddNunitCommandLineOption("/nodots")
                .AddNunitCommandLineOption("/labels")
                .AddNunitCommandLineOption("/noshadow");

            return task;
        }

        /// <summary>
        /// Initializes NunitTask with default command line options for nunit V3.
        /// </summary>
        /// <param name="projectName">Unit test project name.</param>
        /// <returns>New instance of nunit task</returns>
        public static NUnitTask ForNunitV3(params string[] projectName)
        {
            var task = new NUnitTask(projectName.ToList());
            task.AddNunitCommandLineOption("/labels=All")
                .AddNunitCommandLineOption("/trace=Verbose");

            return task;
        }

        /// <summary>
        /// Adds verbose options
        /// </summary>
        /// <returns></returns>
        public NUnitTask WithVerbose()
        {
            AddNunitCommandLineOption("/verbose");
            return this;
        }

        public NUnitTask ClearAllOptions()
        {
            _nunitCommandLineOptions.Clear();
            return this;
        }

        /// <summary>
        /// Excludes category from test. Can be ussed multiple times. Supported only in nunit v3 and above. For v2 use <see cref="AddNunitCommandLineOption"/>
        /// </summary>
        /// <param name="category">The Categorie to be excluded</param>
        /// <returns>The NunitTask</returns>
        public NUnitTask ExcludeCategory(string category)
        {
            _categories = string.IsNullOrEmpty(_categories)
                ? $"cat != {category}"
                : $"{_categories} && cat != {category}";

            return this;
        }

        /// <summary>
        /// Include category in test. Can be ussed multiple times. Supported only in nunit v3 and above. For v2 use <see cref="AddNunitCommandLineOption"/>
        /// </summary>
        /// <param name="category">The category to be included</param>
        /// <returns>The NunitTask</returns>
        public NUnitTask IncludeCategory(string category)
        {
            _categories = string.IsNullOrEmpty(_categories)
                ? $"cat == {category}"
                : $"{_categories} || cat == {category}";

            return this;
        }

        /// <summary>
        /// Sets the .NET framework NUnit console should run under. Supported only in nunit v3 and above. For v2 use <see cref="AddNunitCommandLineOption"/>
        /// </summary>
        /// <param name="framework">Targeted .net framework</param>
        /// <returns>The NunitTask</returns>
        public NUnitTask SetTargetFramework(string framework)
        {
            _targetFramework = framework;
            return this;
        }

        public NUnitTask SetWorkingDirectory(string directory)
        {
            _workingDirectory = directory;
            return this;
        }

        /// <summary>
        /// Full file Path to the nunit console runner.
        /// </summary>
        /// <param name="nunitConsoleFilePath"></param>
        /// <returns></returns>
        public NUnitTask SetNunitConsoleFilePath(string nunitConsoleFilePath)
        {
            _nunitConsoleFileName = nunitConsoleFilePath;
            return this;
        }

        /// <summary>
        /// Build configuration the tests fill be run under.
        /// </summary>
        /// <param name="configruation"></param>
        /// <returns></returns>
        public NUnitTask Configuration(string configruation)
        {
            _configuration = configruation;
            return this;
        }

        /// <summary>
        ///  Add nunit command line option. Can be used multiple times.
        /// </summary>
        /// <param name="nunitCmdOption">nunit command line option to be added.</param>
        /// <returns>The NunitTask</returns>
        public NUnitTask AddNunitCommandLineOption(string nunitCmdOption)
        {
            _nunitCommandLineOptions.Add(nunitCmdOption);
            return this;
        }

        /// <summary>
        /// Full file path to nunit console runner. If not set task tries to get path from  context build propertie <see cref="BuildProps.NUnitConsolePath"/>
        /// </summary>
        /// <param name="fullFilePath"></param>
        /// <returns></returns>
        public NUnitTask NunitConsolePath(string fullFilePath)
        {
            _nunitConsoleFileName = fullFilePath;
            return this;
        }

        /// <inheritdoc />
        /// <summary>
        /// Abstract method defining the actual work for a task.
        /// </summary>
        /// <remarks>This method has to be implemented by the inheriting task.</remarks>
        /// <param name="context">The script execution environment.</param>
        protected override int DoExecute(ITaskContextInternal context)
        {
            if (_nunitConsoleFileName == null)
            {
                _nunitConsoleFileName = context.Properties.Get<string>(BuildProps.NUnitConsolePath, null);
            }

            if (string.IsNullOrEmpty(_nunitConsoleFileName))
            {
                throw new TaskExecutionException($"Path to nunit console is not set. Set it through task fluent method  or build property 'BuildProps.{nameof(BuildProps.NUnitConsolePath)}'.",
                    0);
            }

            RunProgramTask task = new RunProgramTask(new CommandFactory(), _nunitConsoleFileName);

            SetAssemblyFileNameAndWorkingDirFromProjectName(context);
            Validate();
            task.WorkingFolder(_workingDirectory);
            foreach (var testAssemblyFileName in TestAssemblyFileNames)
            {
                task.WithArguments(string.Format(testAssemblyFileName));
            }

            foreach (var nunitCommandLineOption in _nunitCommandLineOptions)
            {
                task.WithArguments(nunitCommandLineOption);
            }

            if (!string.IsNullOrEmpty(_targetFramework))
                task.WithArguments($"/framework:{_targetFramework}");

            if (!string.IsNullOrEmpty(_categories))
                task.WithArguments("--where", _categories);

            task.ExecuteVoid(context);

            return 0;
        }

        private void Validate()
        {
            if (string.IsNullOrEmpty(_nunitConsoleFileName))
            {
                throw new TaskExecutionException("Nunit console file name is not set. Set it through constructor or build properties.", 0);
            }

            if (!string.IsNullOrEmpty(_categories))
            {
                if (_nunitCommandLineOptions.Any(nunitCommandLineOption => nunitCommandLineOption.Contains("where")))
                {
                    throw new TaskExecutionException(
                        "Mixing Exclude/Include category with where clause is nunitCommandLineOptions is not supported eiter use exlude/include category or NunitCommandLineOption with where clause.", 0);
                }
            }
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
                        throw new TaskExecutionException("Build configuration must be set. Set it through context property BuildConfiguration or task method Configuration.", 0);
                    }
                }
                else
                {
                    buildConfiguration = _configuration;
                }

                foreach (var projectName in _projectNames)
                {
                    VSProject project = (VSProject)solution.FindProjectByName(projectName);
                    FileFullPath projectTarget = project.ProjectDirectoryPath.CombineWith(project.GetProjectOutputPath(buildConfiguration))
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
