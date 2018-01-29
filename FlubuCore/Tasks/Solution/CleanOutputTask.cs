using System.Collections.Generic;
using System.Globalization;
using System.IO;
using FlubuCore.Context;
using FlubuCore.IO;
using FlubuCore.Tasks.FileSystem;
using FlubuCore.Tasks.Solution.VSSolutionBrowsing;

namespace FlubuCore.Tasks.Solution
{
    public class CleanOutputTask : TaskBase<int, CleanOutputTask>
    {
        private bool _cleanBuildDir;
        private string _description;
        private bool _cleanOutputDir;
        private List<(string path, bool recreate)> _directoriesToClean = new List<(string path, bool recreate)>();

        protected override string Description
        {
            get
            {
                if (string.IsNullOrEmpty(_description))
                {
                    return "Cleans project outputs in solution";
                }

                return _description;
            }

            set => _description = value;
        }

        /// <summary>
        /// Task deletes added directory
        /// </summary>
        /// <param name="directory">The directory do delete</param>
        /// <param name="recreate">If <c>true</c> directory is recreated. Otherwise deleted.</param>
        /// <returns></returns>
        public CleanOutputTask AddDirectoryToClean(string directory, bool recreate)
        {
            _directoriesToClean.Add((directory, recreate));
            return this;
        }

        /// <summary>
        /// If set OutputDir specified in <see cref="BuildProps.OutputDir"/> deleted and recreated.
        /// </summary>
        /// <returns></returns>
        public CleanOutputTask CleanOutputDir()
        {
            _cleanOutputDir = true;
            return this;
        }

        /// <summary>
        /// If set BuildDir specified in <see cref="BuildProps.BuildDir"/> deleted and recreated.
        /// </summary>
        /// <returns></returns>
        public CleanOutputTask CleanBuildDir()
        {
            _cleanBuildDir = true;
            return this;
        }

        protected override int DoExecute(ITaskContextInternal context)
        {
            string buildConfiguration = context.Properties.Get<string>(BuildProps.BuildConfiguration);
            string productRootDir = context.Properties.Get(BuildProps.ProductRootDir, ".");

            VSSolution solution = context.Properties.Get<VSSolution>(BuildProps.Solution);

            solution.ForEachProject(projectInfo =>
                {
                    if (projectInfo is VSProjectWithFileInfo info)
                    {
                        LocalPath projectOutputPath = info.GetProjectOutputPath(buildConfiguration);

                        if (projectOutputPath == null)
                            return;

                        FullPath projectFullOutputPath = info.ProjectDirectoryPath.CombineWith(projectOutputPath);
                        DeleteDirectoryTask.Execute(context, projectFullOutputPath.ToString(), false);

                        string projectObjPath = string.Format(
                            CultureInfo.InvariantCulture,
                            @"{0}\obj\{1}",
                            projectInfo.ProjectName,
                            buildConfiguration);
                        projectObjPath = Path.Combine(productRootDir, projectObjPath);
                        DeleteDirectoryTask.Execute(context, projectObjPath, false);
                    }
                });

            string buildDir = context.Properties.Get<string>(BuildProps.BuildDir);
            if (!string.IsNullOrEmpty(buildDir) && _cleanBuildDir)
            {
                CreateDirectoryTask createDirectoryTask = new CreateDirectoryTask(buildDir, true);
                createDirectoryTask.Execute(context);
            }

            string outputDir = context.Properties.Get<string>(BuildProps.OutputDir);
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

            return 0;
        }
    }
}
