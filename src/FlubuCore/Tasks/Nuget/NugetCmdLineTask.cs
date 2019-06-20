using System;
using System.Globalization;
using System.IO;
using FlubuCore.Context;
using FlubuCore.Tasks.Process;

namespace FlubuCore.Tasks.Nuget
{
    public class NuGetCmdLineTask : ExternalProcessTaskBase<int, NuGetCmdLineTask>
    {
        private const string PackagesDirName = "packages";
        private readonly string _command;
        private string _description;

        public NuGetCmdLineTask(string command, string workingDirectory = null)
        {
            _command = command;
            ExecuteWorkingFolder = workingDirectory;
            AddAdditionalOptionPrefix("Nuget");
        }

        /// <summary>
        /// Verbosity
        /// </summary>
        public enum NuGetVerbosity
        {
            Normal,
            Quiet,
            Detailed,
        }

        protected override string Description
        {
            get
            {
                if (string.IsNullOrEmpty(_description))
                {
                    return $"Execute NuGet command line tool (command='{_command}')'";
                }

                return _description;
            }

            set { _description = value; }
        }

        public NuGetVerbosity? Verbosity { get; set; }

        /// <summary>
        /// The API key for the server
        /// </summary>
        public string ApiKey { get; set; }

        public static NuGetCmdLineTask Create(string command, params string[] parameters)
        {
            return new NuGetCmdLineTask(command)
                .WithArguments(parameters);
        }

        /// <summary>
        /// Path to the nuget.exe
        /// </summary>
        /// <param name="fullFilePath"></param>
        /// <returns></returns>
        public NuGetCmdLineTask NuGetPath(string fullFilePath)
        {
            ExecutablePath = fullFilePath;
            return this;
        }

        /// <inheritdoc />
        protected override int DoExecute(ITaskContextInternal context)
        {
            ExecutablePath = FindNuGetCmdLinePath();

            if (ExecutablePath == null)
            {
                context.Fail(
                    string.Format(
                        "Could not find NuGet.CommandLine package in the {0} directory. You have to download it yourself.",
                        PackagesDirName), -1);
                return 5;
            }

            _command.MustNotBeNullOrEmpty("Nuget command must not be empty.");

            InsertArgument(0, _command);

            if (Verbosity.HasValue)
                WithArguments("-Verbosity", Verbosity.ToString());

            if (ApiKey != null)
                WithArgumentsValueRequired("-ApiKey", ApiKey, true);

            AddPrefixToAdditionalOptionKey(PrefixProcessors.AddSingleDashPrefixToAdditionalOptionKey);
            return base.DoExecute(context);
        }

        private string FindNuGetCmdLinePath()
        {
            if (!string.IsNullOrEmpty(ExecutablePath))
                return ExecutablePath;

            if (!Directory.Exists(PackagesDirName))
                return null;

            const string getCmdLinePackageName = "NuGet.CommandLine";
            int packageNameLen = getCmdLinePackageName.Length;

            string highestVersionDir = null;
            Version highestVersion = null;

            foreach (string directory in Directory.EnumerateDirectories(
                PackagesDirName,
                string.Format(CultureInfo.InvariantCulture, "{0}.*", getCmdLinePackageName)))
            {
                string dirLocalName = Path.GetFileName(directory);

                // ReSharper disable once PossibleNullReferenceException
                string versionStr = dirLocalName.Substring(packageNameLen + 1);

                if (!Version.TryParse(versionStr, out var version))
                    continue;

                if (highestVersion == null || version > highestVersion)
                {
                    highestVersion = version;
                    highestVersionDir = directory;
                }
            }

            if (highestVersionDir != null)
                return Path.Combine(highestVersionDir, "tools/NuGet.exe");

            return null;
        }
    }
}
