using System;
using FlubuCore.Services;

namespace FlubuCore.BuildServers
{
    public class GitLab
    {
        public static bool RunningOnGitLabCi => Environment.GetEnvironmentVariable("CI_SERVER")?.Equals("yes", StringComparison.OrdinalIgnoreCase) ?? false;

        /// <summary>
        /// Indicates whether build is running on GitLab Ci.
        /// </summary>
        public bool IsRunningOnGitLabCi => RunningOnGitLabCi;

        /// <summary>
        /// The job identifier for the current job. (example: 50)
        /// </summary>
        public int JobId => FlubuEnvironment.GetEnvironmentVariable<int>("CI_JOB_ID");

        /// <summary>
        /// Commit SHA (example: "1ecfd275763eff1d6b4844ea3168962458c9f27a")
        /// </summary>
        public string CommitSHA => FlubuEnvironment.GetEnvironmentVariable("CI_COMMIT_SHA");

        /// <summary>
        /// Commit Short SHA (example: "1ecfd275")
        /// </summary>
        public string CommitShortSHA => FlubuEnvironment.GetEnvironmentVariable("CI_COMMIT_SHORT_SHA");

        /// <summary>
        /// Defines the branch or tag name for project build. (example: "master")
        /// </summary>
        public string CommitRefName => FlubuEnvironment.GetEnvironmentVariable("CI_COMMIT_REF_NAME");

        /// <summary>
        /// Repository Url (example: "https://gitlab-ci-token:abcde-1234ABCD5678ef@example.com/gitlab-org/gitlab-foss.git")
        /// </summary>
        public string RepositoryUrl => FlubuEnvironment.GetEnvironmentVariable("CI_REPOSITORY_URL");

        /// <summary>
        /// Commit Tag (example: "1.0.0")
        /// </summary>
        public string CommitTag => FlubuEnvironment.GetEnvironmentVariable<string>("CI_COMMIT_TAG");

        /// <summary>
        /// The job name for the current job. (example: "spec:other")
        /// </summary>
        public string JobName => FlubuEnvironment.GetEnvironmentVariable("CI_JOB_NAME");

        /// <summary>
        /// The job stage for the current job. (example: "test")
        /// </summary>
        public string JobStage => FlubuEnvironment.GetEnvironmentVariable("CI_JOB_STAGE");

        /// <summary>
        /// Job triggered by manual action
        /// </summary>
        public bool JobManual => FlubuEnvironment.GetEnvironmentVariable<bool>("CI_JOB_MANUAL");

        /// <summary>
        /// Job triggered by API (example: true)
        /// </summary>
        public bool JobTriggered => FlubuEnvironment.GetEnvironmentVariable<bool>("CI_JOB_TRIGGERED");

        /// <summary>
        /// An authorization token provided to this job (example: "abcde-1234ABCD5678ef")
        /// </summary>
        public string JobToken => FlubuEnvironment.GetEnvironmentVariable("CI_JOB_TOKEN");

        /// <summary>
        /// The pipeline identifier for the current job. (example: 1000)
        /// </summary>
        public int PipelineId => FlubuEnvironment.GetEnvironmentVariable<int>("CI_PIPELINE_ID");

        /// <summary>
        /// The pipeline project identifier for the current job. (example: "10")
        /// </summary>
        public int PipelineIid => FlubuEnvironment.GetEnvironmentVariable<int>("CI_PIPELINE_IID");

        /// <summary>
        /// Pages Domain (example: "gitlab.io")
        /// </summary>
        public string PagesDomain => FlubuEnvironment.GetEnvironmentVariable("CI_PAGES_DOMAIN");

        /// <summary>
        /// Pages Url (example: "https://gitlab-org.gitlab.io/gitlab-foss")
        /// </summary>
        public string PagesUrl => FlubuEnvironment.GetEnvironmentVariable("CI_PAGES_URL");

        /// <summary>
        /// Project Id(example: 34)
        /// </summary>
        public int ProjectId => FlubuEnvironment.GetEnvironmentVariable<int>("CI_PROJECT_ID");

        /// <summary>
        /// Project directory for current job (example: "/builds/gitlab-org/gitlab-foss")
        /// </summary>
        public string ProjectDir => FlubuEnvironment.GetEnvironmentVariable("CI_PROJECT_DIR");

        /// <summary>
        /// Project name (example: "gitlab-foss")
        /// </summary>
        public string ProjectName => FlubuEnvironment.GetEnvironmentVariable("CI_PROJECT_NAME");

        /// <summary>
        /// Project title (example: "GitLab FOSS")
        /// </summary>
        public string ProjectTitle => FlubuEnvironment.GetEnvironmentVariable("CI_PROJECT_TITLE");

        /// <summary>
        /// Project namespace (example: "gitlab-org")
        /// </summary>
        public string ProjectNamespace => FlubuEnvironment.GetEnvironmentVariable("CI_PROJECT_NAMESPACE");

        /// <summary>
        /// Project root namespace (example: "gitlab-org")
        /// </summary>
        public string ProjectRootNamespace => FlubuEnvironment.GetEnvironmentVariable("CI_PROJECT_ROOT_NAMESPACE");

        /// <summary>
        /// Project path (example: "gitlab-org/gitlab-foss")
        /// </summary>
        public string ProjectPath => FlubuEnvironment.GetEnvironmentVariable("CI_PROJECT_PATH");

        /// <summary>
        /// Project url (example: "https://example.com/gitlab-org/gitlab-foss")
        /// </summary>
        public string ProjectUrl => FlubuEnvironment.GetEnvironmentVariable("CI_PROJECT_URL");

        /// <summary>
        /// Container images registry url (example: "registry.example.com")
        /// </summary>
        public string Registry => FlubuEnvironment.GetEnvironmentVariable("CI_REGISTRY");

        /// <summary>
        /// Container image url (example: "registry.example.com/gitlab-org/gitlab-foss")
        /// </summary>
        public string RegistryImage => FlubuEnvironment.GetEnvironmentVariable("CI_REGISTRY_IMAGE");

        /// <summary>
        /// Container registry user (example: "gitlab-ci-token")
        /// </summary>
        public string RegistryUser => FlubuEnvironment.GetEnvironmentVariable("CI_REGISTRY_USER");

        /// <summary>
        /// Container registry password (example: "longalfanumstring")
        /// </summary>
        public string RegistryPassword => FlubuEnvironment.GetEnvironmentVariable("CI_REGISTRY_PASSWORD");

        /// <summary>
        /// Runner Id (example: 10)
        /// </summary>
        public int RunnerId => FlubuEnvironment.GetEnvironmentVariable<int>("CI_RUNNER_ID");

        /// <summary>
        /// Runner Description (example: "my runner")
        /// </summary>
        public string RunnerDescription => FlubuEnvironment.GetEnvironmentVariable("CI_RUNNER_DESCRIPTION");

        /// <summary>
        /// Runner Tags (example: "docker, linux")
        /// </summary>
        public string RunnerTags => FlubuEnvironment.GetEnvironmentVariable("CI_RUNNER_TAGS");

        /// <summary>
        /// Server Url (example: "https://example.com")
        /// </summary>
        public string ServerUrl => FlubuEnvironment.GetEnvironmentVariable("CI_SERVER_URL");

        /// <summary>
        /// Server Host (example: "example.com")
        /// </summary>
        public string ServerHost => FlubuEnvironment.GetEnvironmentVariable("CI_SERVER_HOST");

        /// <summary>
        /// Server Port (example: 443)
        /// </summary>
        public int ServerPort => FlubuEnvironment.GetEnvironmentVariable<int>("CI_SERVER_PORT");

        /// <summary>
        /// Server Protocol (example: "https")
        /// </summary>
        public string ServerProtocol => FlubuEnvironment.GetEnvironmentVariable("CI_SERVER_PROTOCOL");

        /// <summary>
        /// Server Name (example: "GitLab")
        /// </summary>
        public string ServerName => FlubuEnvironment.GetEnvironmentVariable("CI_SERVER_NAME");

        /// <summary>
        /// Server Revision (example: "70606bf")
        /// </summary>
        public string ServerRevision => FlubuEnvironment.GetEnvironmentVariable("CI_SERVER_REVISION");

        /// <summary>
        /// Server Version (example: "8.9.0")
        /// </summary>
        public string ServerVersion => FlubuEnvironment.GetEnvironmentVariable("CI_SERVER_VERSION");

        /// <summary>
        /// Server Version Major (example: "8")
        /// </summary>
        public string ServerVersionMajor => FlubuEnvironment.GetEnvironmentVariable("CI_SERVER_VERSION_MAJOR");

        /// <summary>
        /// Server Version Minor (example: "9")
        /// </summary>
        public string ServerVersionMinor => FlubuEnvironment.GetEnvironmentVariable("CI_SERVER_VERSION_MINOR");

        /// <summary>
        /// Server Version Patch (example: "0")
        /// </summary>
        public string ServerVersionPatch => FlubuEnvironment.GetEnvironmentVariable("CI_SERVER_VERSION_PATCH");

        /// <summary>
        /// Gitlab User Email (example: "user@example.com")
        /// </summary>
        public string GitlabUserEmail => FlubuEnvironment.GetEnvironmentVariable("GITLAB_USER_EMAIL");

        /// <summary>
        /// Gitlab User Id (example: 42)
        /// </summary>
        public int GitlabUserId => FlubuEnvironment.GetEnvironmentVariable<int>("GITLAB_USER_ID");
    }
}
