using FlubuCore.Context;
using FlubuCore.Context.FluentInterface.Interfaces;
using FlubuCore.Scripting;

namespace Flubu.Tests.Integration
{
    public class SimpleBuildScript : DefaultBuildScript
    {
        protected override void ConfigureBuildProperties(IBuildPropertiesContext context)
        {
        }

        protected override void ConfigureTargets(ITaskContext session)
        {
            ITargetFluentInterface test = session.CreateTarget("test")
                .AddTask(s => s.CopyFileTask("t1", "t2", false));

            ITargetFluentInterface test1 = session.CreateTarget("test1")
                .DependsOn(test);

            session.CreateTarget("extensions")
                .CoreTaskExtensions()
                .DotnetPublish("33")
                .CreateZipPackageFromProjects("aa", "netcoreapp1.1", "fdf");

            var restore = session
                .CreateTarget("restore")
                .AddCoreTask(s => s.Restore());

            var init = session
                .CreateTarget("init")
                .AddTask(s => s.FetchBuildVersionFromFileTask())
                .AddTask(s => s.FetchVersionFromExternalSourceTask())
                .AddCoreTask(s => s.UpdateNetCoreVersionTask("a"));

            var package = session
                .CreateTarget("package")
                .DependsOn(init, restore);

            session.CreateTarget("Linux")
                .AddCoreTask(x => x.LinuxTasks().SystemCtlTask("a", "b"))
                .AddCoreTask(x => x.LinuxTasks().SystemCtlTask("a", "b"));

            session.CreateTarget("IIS")
                .AddTask(x => x.IisTasks().CreateWebsiteTask())
                .AddTask(x => x.IisTasks().DeleteAppPoolTask("test"));

            package
                .CoreTaskExtensions()
                .DotnetPublish("a", "b", "c")
                .CreateZipPackageFromProjects("8d", "netcoreapp1.1", "a", "b", "c", task =>
                {
                    task.AddDirectoryToPackage("configuration", "configuration", true)
                        .AddFileToPackage("DeployScript.cs", string.Empty)
                        .AddFileToPackage("project.json", string.Empty)
                        .AddFileToPackage("NuGet.config", string.Empty);
                });

        }
    }
}
