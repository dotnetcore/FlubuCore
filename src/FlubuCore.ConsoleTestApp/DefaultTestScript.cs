using FlubuCore.BuildServers;
using FlubuCore.Context;
using FlubuCore.Context.Attributes;
using FlubuCore.Context.FluentInterface.Interfaces;
using FlubuCore.Scripting;
using Microsoft.Extensions.Logging;

namespace FlubuCore.ConsoleTestApp
{
    public class MyScript : DefaultBuildScript
    {        
        public string Output => RootDirectory.CombineWith("output222");

        protected override void ConfigureTargets(ITaskContext context)
        {
            context.CreateTarget("MyTarget")
                .AddTask(x => x.RunProgramTask("export")
                    .WithArguments("https_proxy=http://150.150.150.1:8080"))
                .AddTask(x => x.RunProgramTask("echo")
                    .WithArguments("$http_proxy"))
                .AddTask(x => x.RunProgramTask("scl")
                    .WithArguments("enable", "rh-dotnet31", "bash"))
                .AddCoreTask(x => x.Restore())
                .AddCoreTask(x => x.Build())
                .AddCoreTask(x => x.Publish()
                    .Framework("netcoreapp3.1")
                    .Configuration("Release"));
            ////And so on
        }
    }
}
