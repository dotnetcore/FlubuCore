using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using FlubuCore.Context;
using FlubuCore.IO;
using FlubuCore.Tasks.Process;
using FlubuCore.Tasks.Solution.VSSolutionBrowsing;
using Microsoft.DotNet.Cli.Utils;

namespace FlubuCore.Tasks.Testing
{
    /// <summary>
    /// Run NUnit tests with NUnit console runner.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here.")]
    public class NUnitTask : TaskBase<int>
    {
        private readonly List<string> _nunitCommandLineOptions = new List<string>();

        private readonly string _projectName;

        private string _nunitConsoleFileName;

        /// <summary>
        /// unit test working directory.
        /// </summary>
        private string _workingDirectory;

        /// <summary>
        ///  assembly to test.
        /// </summary>
        private string _testAssemblyFileName;

        /// <summary>
        ///  test categories that will be included/excluded in tests.
        /// </summary>
        private string _categories;

        /// <summary>
        /// .NET framework NUnit console should run under.
        /// </summary>
        private string _targetFramework;

        /////// <summary>
        /////// Initializes a new instance of the<see cref="NUnitTask"/> class.
        /////// </summary>
        /////// <param name = "testAssemblyFileName" > File name of the assembly containing the test code.</param>
        /////// <param name = "nunitConsoleFileName" > Path to the NUnit-console.exe</param>
        /////// <param name = "workingDirectory" > Working directory to use.</param>
        ////public NUnitTask(
        ////    string testAssemblyFileName,
        ////    string nunitConsoleFileName,
        ////    string workingDirectory)
        ////{
        ////    _nunitConsoleFileName = nunitConsoleFileName;
        ////    _testAssemblyFileName = testAssemblyFileName;
        ////    _workingDirectory = workingDirectory;
        ////}

        /// <summary>
        /// Initializes a new instance of the <see cref="NUnitTask"/> class.
        /// </summary>
        /// <param name="nunitConsoleFileName">full file path to nunit console</param>
        /// <param name="projectName">Unit test project name.</param>
        public NUnitTask(string projectName = null, string nunitConsoleFileName = null)
        {
            _nunitConsoleFileName = nunitConsoleFileName;
            _projectName = projectName;
        }

        public string TestAssemblyFileName
        {
            get { return _testAssemblyFileName; }
            set { _testAssemblyFileName = value; }
        }

        /// <summary>
        /// Initializes NunitTask with default command line options for nunit V2.
        /// </summary>
        /// <param name="projectName">Unit test project name.</param>
        /// <returns>New instance of nunit task</returns>
        public static NUnitTask ForNunitV2(string projectName)
        {
            var task = new NUnitTask(projectName);
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
        public static NUnitTask ForNunitV3(string projectName)
        {
            var task = new NUnitTask(projectName);
            task.AddNunitCommandLineOption("/labels=All")
                .AddNunitCommandLineOption("/trace=Verbose")
                .AddNunitCommandLineOption("/verbose");

            return task;
        }

        /// <summary>
        /// Excludes category from test. Can be ussed multiple times. Supported only in nunit v3 and above. For v2 use <see cref="AddNunitCommandLineOption"/>
        /// </summary>
        /// <param name="category">The Categorie to be excluded</param>
        /// <returns>The NunitTask</returns>
        public NUnitTask ExcludeCategory(string category)
        {
            if (string.IsNullOrEmpty(_categories))
            {
                _categories = string.Format(CultureInfo.InvariantCulture, "cat != {0}", category);
            }
            else
            {
                _categories = string.Format(CultureInfo.InvariantCulture, "{0} && cat != {1}", _categories, category);
            }

            return this;
        }

        /// <summary>
        /// Include category in test. Can be ussed multiple times. Supported only in nunit v3 and above. For v2 use <see cref="AddNunitCommandLineOption"/>
        /// </summary>
        /// <param name="category">The category to be included</param>
        /// <returns>The NunitTask</returns>
        public NUnitTask IncludeCategory(string category)
        {
            if (string.IsNullOrEmpty(_categories))
            {
                _categories = string.Format(CultureInfo.InvariantCulture, "cat == {0}", category);
            }
            else
            {
                _categories = string.Format(CultureInfo.InvariantCulture, "{0} || cat == {1}", _categories, category);
            }

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
        ///  Add nunit command line option. Can be used multiple times.
        /// </summary>
        /// <param name="nunitCmdOption">nunit command line option to be added.</param>
        /// <returns>The NunitTask</returns>
        public NUnitTask AddNunitCommandLineOption(string nunitCmdOption)
        {
            _nunitCommandLineOptions.Add(nunitCmdOption);
            return this;
        }

        public NUnitTask NunitConsolePath(string fullFilePath)
        {
            _nunitConsoleFileName = fullFilePath;
            return this;
        }

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

            RunProgramTask task = new RunProgramTask(new CommandFactory(), _nunitConsoleFileName);

            SetAssemblyFileNameAndWorkingDirFromProjectName(context);
            Validate();

            task
                .WorkingFolder(_workingDirectory)
                .WithArguments(string.Format(_testAssemblyFileName));

            foreach (var nunitCommandLineOption in _nunitCommandLineOptions)
            {
                task.WithArguments(nunitCommandLineOption);
            }

            if (!string.IsNullOrEmpty(_targetFramework))
                task.WithArguments($"/framework:{_targetFramework}");

            if (!string.IsNullOrEmpty(_categories))
                task.WithArguments($"--where \"{_categories}\"");

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
            if (_projectName != null)
            {
                VSSolution solution = context.Properties.Get<VSSolution>(BuildProps.Solution);
                string buildConfiguration = context.Properties.Get<string>(BuildProps.BuildConfiguration);

                VSProjectWithFileInfo project =
                    (VSProjectWithFileInfo)solution.FindProjectByName(_projectName);
                FileFullPath projectTarget = project.ProjectDirectoryPath.CombineWith(project.GetProjectOutputPath(buildConfiguration))
                    .AddFileName("{0}.dll", project.ProjectName);

                _testAssemblyFileName = projectTarget.ToString();
                _workingDirectory = Path.GetDirectoryName(projectTarget.ToString());
            }
        }
    }
}
