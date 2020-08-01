using System;
using System.Collections.Generic;
using System.Text;
using FlubuCore.BuildServers.Configurations;

namespace FlubuCore
{
    public class FlubuConfigurationBuilder : IFlubuConfigurationBuilder
    {
        public TravisOptions TravisOptions { get; set; } = new TravisOptions();

        public AzurePipelineOptions AzurePipelineOptions { get; set; } = new AzurePipelineOptions();

        public AppVeyorOptions AppVeyorOptions { get; set; } = new AppVeyorOptions();

        public GitHubActionsOptions GitHubActionsOptions { get; set; } = new GitHubActionsOptions();

        public JenkinsOptions JenkinsOptions { get; set; } = new JenkinsOptions();

        public FlubuConfigurationBuilder ConfigureTravis(Action<TravisOptions> configuration)
        {
            configuration.Invoke(TravisOptions);
            return this;
        }

        public FlubuConfigurationBuilder ConfigureAzurePipelines(Action<AzurePipelineOptions> configuration)
        {
            configuration.Invoke(AzurePipelineOptions);

            return this;
        }

        public FlubuConfigurationBuilder ConfigureAppVeyor(Action<AppVeyorOptions> configuration)
        {
            configuration.Invoke(AppVeyorOptions);

            return this;
        }

        public FlubuConfigurationBuilder ConfigureGitHubActions(Action<GitHubActionsOptions> configuration)
        {
            configuration.Invoke(GitHubActionsOptions);

            return this;
        }

        public FlubuConfigurationBuilder ConfigureJenkins(Action<JenkinsOptions> configuration)
        {
            configuration.Invoke(JenkinsOptions);

            return this;
        }

        public FlubuConfiguration Build()
        {
            return new FlubuConfiguration(TravisOptions, AzurePipelineOptions, GitHubActionsOptions, AppVeyorOptions, JenkinsOptions);
        }
    }
}
