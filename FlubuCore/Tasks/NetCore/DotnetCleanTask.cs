using System;
using System.Collections.Generic;
using System.Text;
using FlubuCore.Context;
using FlubuCore.Tasks.FileSystem;

namespace FlubuCore.Tasks.NetCore
{
    public class DotnetCleanTask : ExecuteDotnetTaskBase<DotnetCleanTask>
    {
        private string _description;

        private bool _cleanBuildDir;

        private bool _cleanOutputDir;

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
            Arguments.Insert(0, projectName);
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
            return this;
        }

        /// <summary>
        /// Set the verbosity level of the command.
        /// </summary>
        /// <param name="verbosity"></param>
        /// <returns></returns>
        public DotnetCleanTask Verbosity(VerbosityOptions verbosity)
        {
            WithArguments("--verbosity", verbosity.ToString().ToLower());
            return this;
        }

        /// <summary>
        /// Task deletes added directory
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
        /// If set OutputDir specified in <see cref="BuildProps.OutputDir"/> deleted and recreated.
        /// </summary>
        /// <returns></returns>
        public DotnetCleanTask CleanOutputDir()
        {
            _cleanOutputDir = true;
            return this;
        }

        /// <summary>
        /// If set BuildDir specified in <see cref="BuildProps.BuildDir"/> deleted and recreated.
        /// </summary>
        /// <returns></returns>
        public DotnetCleanTask CleanBuildDir()
        {
            _cleanBuildDir = true;
            return this;
        }

        protected override void BeforeExecute(ITaskContextInternal context)
        {
            if (Arguments.Count == 0 || Arguments[0].StartsWith("-"))
            {
                var solustionFileName = context.Properties.Get<string>(BuildProps.SolutionFileName, null);
                if (solustionFileName != null)
                {
                    Project(solustionFileName);
                }
            }

            if (!Arguments.Exists(x => x == "-c" || x == "--configuration"))
            {
                var configuration = context.Properties.Get<string>(BuildProps.BuildConfiguration, null);
                if (configuration != null)
                {
                    Configuration(configuration);
                }
            }

            string buildDir = context.Properties.Get<string>(BuildProps.BuildDir, null);
            if (!string.IsNullOrEmpty(buildDir) && _cleanBuildDir)
            {
                CreateDirectoryTask createDirectoryTask = new CreateDirectoryTask(buildDir, true);
                createDirectoryTask.Execute(context);
            }

            string outputDir = context.Properties.Get<string>(BuildProps.OutputDir, null);
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
        }
    }
}
