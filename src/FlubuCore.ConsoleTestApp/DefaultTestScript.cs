using FlubuCore.BuildServers;
using FlubuCore.BuildServers.Configurations.Models.Jenkins;
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

            configurationBuilder.ConfigureJenkins(x =>
            {
                x.ConfigureOptions(o => o.Retry = 10);
                x.ConfigurePostSteps(o => o.AddSendEmailPostStep(JenkinsPostConditions.Changed, "projectName", "markoz@comtrade.com"));
                x.AddCustomStageBeforeTargets(o =>
                {
                    o.Name = "Before";
                    o.AddStep("Kekec").AddStep("Lamoid");

                });
                x.AddCustomStageAfterTargets(o =>
                {
                    o.Name = "After";
                    o.AddStep("Kekec").AddStep("Lamoid");

                });
            });
        }
    }
}