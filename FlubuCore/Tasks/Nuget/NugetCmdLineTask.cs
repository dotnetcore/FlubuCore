using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using FlubuCore.Context;
using FlubuCore.Tasks.Process;

namespace FlubuCore.Tasks.Nuget
{
    public class NuGetCmdLineTask : TaskBase<int>
    {
        private const string PackagesDirName = "packages";

        private readonly List<string> _args = new List<string>();

        private readonly string _command;

        private readonly string _workingDirectory;

        public NuGetCmdLineTask(string command, string workingDirectory = null)
        {
            _command = command;
            _workingDirectory = workingDirectory;
        }

        /// <summary>
        /// Verbosity
        /// </summary>
        public enum NuGetVerbosity
        {
            Normal,
            Quiet,
            Detailed
        }

        public NuGetVerbosity? Verbosity { get; set; }

        /// <summary>
        /// The API key for the server
        /// </summary>
        public string ApiKey { get; set; }

        /// <summary>
        /// Path to the nuget.exe
        /// </summary>
        public string ExecutablePath { get; private set; }

        public static NuGetCmdLineTask Create(string command, params string[] parameters)
        {
            var t = new NuGetCmdLineTask(command);
            t._args.AddRange(parameters);
            return t;
        }

        /// <summary>
        /// Add's argument to the nuget.exe 
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public NuGetCmdLineTask AddArgument(string arg)
        {
            _args.Add(arg);
            return this;
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
            string nugetCmdLinePath = FindNuGetCmdLinePath();

            if (nugetCmdLinePath == null)
            {
                context.Fail(
                    string.Format(
                        "Could not find NuGet.CommandLine package in the {0} directory. You have to download it yourself.",
                        PackagesDirName), -1);
                return -1;
            }

            IRunProgramTask runProgramTask = context.Tasks().RunProgramTask(nugetCmdLinePath);

            if (_workingDirectory != null)
                runProgramTask.WorkingFolder(_workingDirectory);

            runProgramTask.WithArguments(_command);

            if (Verbosity.HasValue)
                runProgramTask.WithArguments("-Verbosity", Verbosity.ToString());
            if (ApiKey != null)
                runProgramTask.WithArguments("-ApiKey").WithArguments(ApiKey);

            foreach (string arg in _args)
                runProgramTask.WithArguments(arg);

            return runProgramTask.Execute(context);
        }

        private string FindNuGetCmdLinePath()
        {
            if (!string.IsNullOrEmpty(ExecutablePath))
                return ExecutablePath;

            if (!Directory.Exists(PackagesDirName))
                return null;

            const string NuGetCmdLinePackageName = "NuGet.CommandLine";
            int packageNameLen = NuGetCmdLinePackageName.Length;

            string highestVersionDir = null;
            Version highestVersion = null;

            foreach (string directory in Directory.EnumerateDirectories(
                PackagesDirName,
                string.Format(CultureInfo.InvariantCulture, "{0}.*", NuGetCmdLinePackageName)))
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
