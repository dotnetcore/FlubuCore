using System;
using System.Collections.Generic;
using System.Text;
using FlubuCore.Context;
using FlubuCore.Tasks.Attributes;
using FlubuCore.Tasks.FileSystem;
using FlubuCore.Tasks.Process;

namespace FlubuCore.Tasks.NetCore
{
    public class DotnetCleanTask : ExecuteDotnetTaskBase<DotnetCleanTask>
    {
        private string _description;

        private bool _cleanBuildDir;

        private bool _cleanOutputDir;

        private string _projectName;

        private List<(string path, bool recreate)> _directoriesToClean = new List<(string path, bool recreate)>();

        public DotnetCleanTask()
            : base(StandardDotnetCommands.Clean)
        {
        }

        protected override string Description
        {
            get
            {
                if (string.IsNullOrEmpty(_description))
                {
                    return $"Executes dotnet command Clean";
                }

                return _description;
            }

            set { _description = value; }
        }

        /// <summary>
        /// The MSBuild project file to publish. If a project file is not specified, MSBuild searches the current working directory for a file that has a file extension that ends in `proj` and uses that file.
        /// </summary>
        /// <param name="projectName"></param>
        /// <returns></returns>
        public DotnetCleanTask Project(string projectName)
        {
            _projectName = projectName;
            return this;
        }

        /// <summary>
        /// Clean a specific framework.
        /// </summary>
        /// <param name="framework"></param>
        /// <returns></returns>
        [ArgKey("--framework", "-f")]
        public DotnetCleanTask Framework(string framework)
        {
            WithArgumentsKeyFromAttribute(framework);
            return this;
        }

        /// <summary>
        /// Clean a specific framework.
        /// </summary>
        /// <param name="framework"></param>
        /// <returns></returns>
        [ArgKey("--framework", "-f")]
        public DotnetCleanTask Framework(Framework framework)
        {
            WithArgumentsKeyFromAttribute(framework.ToString());
            return this;
        }

        /// <summary>
        /// Clean a specific configuration.
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        [ArgKey("--configuration", "-c")]
        public DotnetCleanTask Configuration(string configuration)
        {
            WithArgumentsKeyFromAttribute(configuration);
            return this;
        }

        /// <summary>
        /// Clean a specific configuration.
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        [ArgKey("--configuration", "-c")]
        public DotnetCleanTask Configuration(Configuration configuration)
        {
            WithArgumentsKeyFromAttribute(configuration.ToString());
            return this;
        }

        /// <summary>
        /// Set the verbosity level of the command.
        /// </summary>
        /// <param name="verbosity"></param>
        /// <returns></returns>
        [ArgKey("--verbosity", "-v")]
        public DotnetCleanTask Verbosity(VerbosityOptions verbosity)
        {
            WithArgumentsKeyFromAttribute(verbosity.ToString().ToLower());
            return this;
        }

        /// <summary>
        /// Task deletes added directory.
        /// </summary>
        /// <param name="directory">The directory do delete</param>
        /// <param name="recreate">If <c>true</c> directory is recreated. Otherwise deleted.</param>
        /// <returns></returns>
        public DotnetCleanTask AddDirectoryToClean(string directory, bool recreate)
        {
            _directoriesToClean.Add((directory, recreate));
            return this;
        }

        /// <summary>
        /// If set output directory specified in <see cref="BuildProps.OutputDir"/> is deleted and recreated.
        /// </summary>
        /// <returns></returns>
        public DotnetCleanTask CleanOutputDir()
        {
            _cleanOutputDir = true;
            return this;
        }

        /// <summary>
        /// If set Build directory specified in <see cref="BuildProps.BuildDir"/> is deleted and recreated.
        /// </summary>
        /// <returns></returns>
        public DotnetCleanTask CleanBuildDir()
        {
            _cleanBuildDir = true;
            return this;
        }

        protected override void BeforeExecute(ITaskContextInternal context, IRunProgramTask runProgramTask)
        {
            var args = GetArguments();
            if (string.IsNullOrEmpty(_projectName))
            {
                if (args.Count == 0 || args[0].StartsWith("-"))
                {
                    var solustionFileName = context.Properties.Get<string>(BuildProps.SolutionFileName, null);
                    if (solustionFileName != null)
                    {
                        InsertArgument(0, solustionFileName);
                    }
                }
            }
            else
            {
                InsertArgument(0, _projectName);
            }

            if (!args.Exists(x => x == "-c" || x == "--configuration"))
            {
                var configuration = context.Properties.Get<string>(BuildProps.BuildConfiguration, null);
                if (configuration != null)
                {
                    Configuration(configuration);
                }
            }

            string buildDir = context.Properties.Get<string>(DotNetBuildProps.BuildDir, null);
            if (!string.IsNullOrEmpty(buildDir) && _cleanBuildDir)
            {
                CreateDirectoryTask createDirectoryTask = new CreateDirectoryTask(buildDir, true);
                createDirectoryTask.Execute(context);
            }

            string outputDir = context.Properties.Get<string>(DotNetBuildProps.OutputDir, null);
            if (!string.IsNullOrEmpty(outputDir) && _cleanOutputDir)
            {
                CreateDirectoryTask createDirectoryTask = new CreateDirectoryTask(outputDir, true);
                createDirectoryTask.Execute(context);
            }

            foreach (var dir in _directoriesToClean)
            {
                if (dir.recreate)
                {
                    CreateDirectoryTask createDirectoryTask = new CreateDirectoryTask(dir.path, true);
                    createDirectoryTask.Execute(context);
                }
                else
                {
                    DeleteDirectoryTask task = new DeleteDirectoryTask(dir.path, false);
                    task.Execute(context);
                }
            }

            base.BeforeExecute(context, runProgramTask);
        }
    }
}
