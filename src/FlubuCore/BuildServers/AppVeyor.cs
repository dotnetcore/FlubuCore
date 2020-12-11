using System;
using FlubuCore.Services;

namespace FlubuCore.BuildServers
{
    public class AppVeyor
    {
        public static bool RunningOnAppVeyor => !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("APPVEYOR"));

        /// <summary>
        /// Indicates whether build is running on AppVeyor build server.
        /// </summary>
        public bool IsRunningOnAppVeyor => RunningOnAppVeyor;

        /// <summary>
        /// Gets the build id.
        /// </summary>
        public string BuildId => Environment.GetEnvironmentVariable("APPVEYOR_BUILD_ID");

        /// <summary>
        /// Gets the build number.
        /// </summary>
        public string BuildNumber => Environment.GetEnvironmentVariable("APPVEYOR_BUILD_NUMBER");

        /// <summary>
        /// Get's the build folder.
        /// </summary>
        public string BuildWorkingFolder => Environment.GetEnvironmentVariable("APPVEYOR_BUILD_FOLDER");

        /// <summary>
        /// Get's the commit id.
        /// </summary>
        public string CommitId => Environment.GetEnvironmentVariable("APPVEYOR_REPO_COMMIT");

        /// <summary>
        /// Get's the commit author.
        /// </summary>
        public string CommitAuthor => Environment.GetEnvironmentVariable("APPVEYOR_REPO_COMMIT_AUTHOR");

        /// <summary>
        /// Get's the commit timestamp.
        /// </summary>
        public string CommitTimeStamp => Environment.GetEnvironmentVariable("APPVEYOR_REPO_COMMIT_TIMESTAMP");

        /// <summary>
        /// Get's the commit message.
        /// </summary>
        public string CommitMessage => Environment.GetEnvironmentVariable("APPVEYOR_REPO_COMMIT_MESSAGE");

        /// <summary>
        /// Get's the commit message.
        /// </summary>
        public string CommitMessageExtended => Environment.GetEnvironmentVariable("APPVEYOR_REPO_COMMIT_MESSAGE_EXTENDED");

        /// <summary>
        /// Get's the current branch name.
        /// </summary>
        public string BranchName => Environment.GetEnvironmentVariable("APPVEYOR_REPO_BRANCH");

        /// <summary>
        ///  If <c>true</c> build was started by pushed tag. Otherwise not.
        /// </summary>
        public bool IsTag => FlubuEnvironment.GetEnvironmentVariable<bool>("APPVEYOR_REPO_TAG");

        /// <summary>
        /// Gets the name of the tag.
        /// </summary>
        public string TagName => FlubuEnvironment.GetEnvironmentVariable("APPVEYOR_REPO_TAG_NAME");
    }
}