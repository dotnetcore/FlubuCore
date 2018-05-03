using System;
using System.Collections.Generic;
using System.Text;

namespace FlubuCore.BuildServers
{
    public class AppVeyor
    {
        public static bool IsRunningOnAppVeyor => !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("APPVEYOR"));

        /// <summary>
        /// Gets the build id.
        /// </summary>
        public static string BuildId => Environment.GetEnvironmentVariable("APPVEYOR_BUILD_ID");

        /// <summary>
        /// Gets the build number.
        /// </summary>
        public static string BuildNumber => Environment.GetEnvironmentVariable("APPVEYOR_BUILD_NUMBER");

        /// <summary>
        /// Get's the build folder.
        /// </summary>
        public static string BuildWorkingFolder => Environment.GetEnvironmentVariable("APPVEYOR_BUILD_FOLDER");

        /// <summary>
        /// Get's the commit id.
        /// </summary>
        public static string CommitId => Environment.GetEnvironmentVariable("APPVEYOR_REPO_COMMIT");

        /// <summary>
        /// Get's the commit author.
        /// </summary>
        public static string CommitAuthor => Environment.GetEnvironmentVariable("APPVEYOR_REPO_COMMIT_AUTHOR");

        /// <summary>
        /// Get's the commit timestamp.
        /// </summary>
        public static string CommitTimeStamp => Environment.GetEnvironmentVariable("APPVEYOR_REPO_COMMIT_TIMESTAMP");

        /// <summary>
        /// Get's the commit message.
        /// </summary>
        public static string CommitMessage => Environment.GetEnvironmentVariable("APPVEYOR_REPO_COMMIT_MESSAGE");

        /// <summary>
        /// Get's the commit message.
        /// </summary>
        public static string CommitMessageExtended => Environment.GetEnvironmentVariable("APPVEYOR_REPO_COMMIT_MESSAGE_EXTENDED");
    }
}