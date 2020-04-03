using FlubuCore.Context;
using FlubuCore.Context.FluentInterface.Interfaces;
using FlubuCore.Scripting;
using Xunit;

namespace FlubuCore.Tests.Integration
{
    public class SimpleBuildScript : DefaultBuildScript
    {
        protected override void ConfigureBuildProperties(IBuildPropertiesContext context)
        {
        }

        protected override void ConfigureTargets(ITaskContext session)
        {
            var test = session.CreateTarget("test")
                .AddTask(s => s.CopyFileTask("t1", "t2", false));

            var test1 = session.CreateTarget("test1")
                .DependsOn(test);

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

            session.CreateTarget("Do").Do(DoExample, "test");
        }

        private void DoExample(ITaskContext context, string parameter)
        {
            Assert.Equal("test", parameter);
        }
    }
}