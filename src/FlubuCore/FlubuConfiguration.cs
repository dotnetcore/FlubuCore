using System;
using System.Collections.Generic;
using System.Text;
using FlubuCore.BuildServers.Configurations;
using FlubuCore.Context;

namespace FlubuCore
{
    public class FlubuConfiguration
    {
        protected internal FlubuConfiguration()
        {
        }

        public FlubuConfiguration(
            TravisOptions travisOptions,
            AzurePipelineOptions azureOptions,
            GitHubActionsOptions gitHubActionsOptions,
            AppVeyorOptions appVeyorOptions)
        {
            TravisOptions = travisOptions;
            AzurePipelineOptions = azureOptions;
            GitHubActionsOptions = gitHubActionsOptions;
            AppVeyorOptions = appVeyorOptions;
        }

        public TravisOptions TravisOptions { get; private set; }

        public AzurePipelineOptions AzurePipelineOptions { get; private set; }

        public GitHubActionsOptions GitHubActionsOptions { get; private set; }

        public AppVeyorOptions AppVeyorOptions { get; private set; }

        public List<BuildServerType> GenerateOnBuild()
        {
            List<BuildServerType> ci = new List<BuildServerType>();

            if (AzurePipelineOptions != null && AzurePipelineOptions.ShouldGenerateOnEachBuild)
            {
                ci.Add(BuildServerType.AzurePipelines);
            }

            if (TravisOptions != null && TravisOptions.ShouldGenerateOnEachBuild)
            {
                ci.Add(BuildServerType.TravisCI);
            }

            if (AppVeyorOptions != null && AppVeyorOptions.ShouldGenerateOnEachBuild)
            {
                ci.Add(BuildServerType.AppVeyor);
            }

            if (GitHubActionsOptions != null && GitHubActionsOptions.ShouldGenerateOnEachBuild)
            {
                ci.Add(BuildServerType.GitHubActions);
            }

            return ci;
        }

        public void CopyFrom(FlubuConfigurationBuilder builder)
        {
            TravisOptions = builder.TravisConfiguration;
            AzurePipelineOptions = builder.AzurePipelineOptions;
            AppVeyorOptions = builder.AppVeyorOptions;
            GitHubActionsOptions = builder.GitHubActionsOptions;
        }
    }
}
