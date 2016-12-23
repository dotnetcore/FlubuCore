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
    public class CleanOutputTask : TaskBase<int>
    {
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

            return 0;
        }
    }
}
