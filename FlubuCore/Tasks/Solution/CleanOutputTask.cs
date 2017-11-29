using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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

        public CleanOutputTask CleanBuildDir()
        {
            this._cleanBuildDir = true;
            return this;
        }

        protected override string Description
        {
            get
            {
                if (string.IsNullOrEmpty(_description))
                {
                    return $"Cleans project outputs in solution";
                }
                return _description;
            }
            set { _description = value; }
        }

        protected override int DoExecute(ITaskContextInternal context)
        {
            string buildConfiguration = context.Properties.Get<string>(BuildProps.BuildConfiguration);
            string productRootDir = context.Properties.Get(BuildProps.ProductRootDir, ".");

            VSSolution solution = context.Properties.Get<VSSolution>(BuildProps.Solution);

            solution.ForEachProject(projectInfo =>
                {
                    if (projectInfo is VSProjectWithFileInfo)
                    {
                        VSProjectWithFileInfo info = (VSProjectWithFileInfo)projectInfo;

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

            return 0;
        }
    }
}
