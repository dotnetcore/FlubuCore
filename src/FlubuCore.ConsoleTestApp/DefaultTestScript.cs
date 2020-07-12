using FlubuCore.BuildServers;
using FlubuCore.Context;
using FlubuCore.Context.Attributes;
using FlubuCore.Context.FluentInterface.Interfaces;
using FlubuCore.Scripting;
using Microsoft.Extensions.Logging;

namespace FlubuCore.ConsoleTestApp
{
    public class DefaultTestScript : DefaultBuildScript
    {        
        public string Output => RootDirectory.CombineWith("output222");

        protected override void ConfigureTargets(ITaskContext context)
        {
            var clean = context.CreateTarget("Clean").AddCoreTask(x => x.Clean());

            context.CreateTarget("Build")
                .SetAsDefault()
                .AddCoreTask(x => x.Build())
                .DependsOn(clean);
        }

        public override void Configure(IFlubuConfigurationBuilder configurationBuilder, ILoggerFactory loggerFactory)
        {
            configurationBuilder.ConfigureAzurePipelines(
                config =>
                {
                    config.SetWorkingDirectory("Abc");
                    config.AddCustomScriptStepBeforeTargets(script =>
                    {
                        script.DisplayName = "Custom script step example before target execution";
                        script.Script = "echo before target";
                    });

                    config.AddCustomScriptStepAfterTargets(script =>
                    {
                        script.DisplayName = "Custom script step example after target execution";
                        script.Script = "echo after target";
                    });
                });

            configurationBuilder.ConfigureAppVeyor(app => { app.AddSkipCommits("test.jpg"); });

            configurationBuilder.ConfigureGitHubActions(actions =>
            {
                actions.OnPush().AddBranchesOnPush("Test");
                actions.AddCustomStepBeforeTargets(x =>
                {
                    x.Run = "Abcv";
                    x.Name = "Custom step before";
                });

                actions.AddCustomStepAfterTargets(x =>
                {
                    x.Run = "Lamoid";
                    x.Name = "Custom step before";
                });

                actions.AddEnvironmentVariableToSpecificVmImageJob("Test", "TestValue", GitHubActionsImage.AllLatest);
            });
        }
    }
}