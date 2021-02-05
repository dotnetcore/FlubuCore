using System;

namespace FlubuCore.BuildServers
{
    public class AzurePipelines
    {
        public static bool RunningOnAzurePipelines => !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("TF_BUILD")) && !IsHostedAgent;

        public static bool RunningOnAzurePipelinesHosted => !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("TF_BUILD")) && IsHostedAgent;

        /// <summary>
        /// Indicates whether build is running on Team foundation server.
        /// </summary>
        public bool IsRunningOnAzurePipelines => RunningOnAzurePipelines;

        public bool IsRunningOnAzurePipelinesHosted => RunningOnAzurePipelinesHosted;

        /// <summary>
        /// Gets the build number.
        /// </summary>
        public string BuildNumber => Environment.GetEnvironmentVariable("BUILD_BUILDNUMBER");

        public string AgentMachineName => Environment.GetEnvironmentVariable("AGENT_MACHINENAME");

        public string AgentName => Environment.GetEnvironmentVariable("AGENT_NAME");

        public string AgentJobStatus => Environment.GetEnvironmentVariable("AGENT_JOBSTATUS");

        public string BuildRepositoryName => Environment.GetEnvironmentVariable("BUILD_REPOSITORY_NAME");

        public string BuildReason => Environment.GetEnvironmentVariable("BUILD_REASON");

        public string BuildSourceBranchName => Environment.GetEnvironmentVariable("BUILD_SOURCEBRANCHNAME");

        public string BuildSourceBranch => Environment.GetEnvironmentVariable("BUILD_SOURCEBRANCH");

        private static bool IsHostedAgent
        {
            get
            {
                var agentName = Environment.GetEnvironmentVariable("AGENT_NAME");
                return !string.IsNullOrWhiteSpace(agentName) &&
                       (agentName.StartsWith("Hosted") || agentName.StartsWith("Azure Pipelines"));
            }
        }
    }
}
