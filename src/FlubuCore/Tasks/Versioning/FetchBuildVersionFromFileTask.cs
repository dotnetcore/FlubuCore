using System;
using System.Collections.Generic;
using System.IO;
using FlubuCore.Context;
using FlubuCore.Tasks.Solution;
using Microsoft.DotNet.Cli.Utils;

namespace FlubuCore.Tasks.Versioning
{
    public class FetchBuildVersionFromFileTask : TaskBase<BuildVersion, FetchBuildVersionFromFileTask>, IFetchBuildVersionTask
    {
        private static List<string> _defaultprojectVersionFiles = new List<string>()
        {
            "Changelog.md",
            "ReleaseNotes.md",
            "ReleaseNotes.txt",
        };

        private bool _doNotSaveVersionToSession;

        private string _prefixToRemove;

        private bool _allowSuffix;

        private List<string> _projectVersionFiles = new List<string>();

        private string _description;

        protected override string Description
        {
            get
            {
                if (string.IsNullOrEmpty(_description))
                {
                    return $"Fetches build version from file.";
                }

                return _description;
            }

            set { _description = value; }
        }

        /// <summary>
        /// Removes prefix from version.
        /// For example if u write version in file like so: ## 1.0.0.0 specify '##' as prefix to remove.
        /// </summary>
        /// <param name="prefix"></param>
        /// <returns></returns>
        public FetchBuildVersionFromFileTask RemovePrefix(string prefix)
        {
            _prefixToRemove = prefix;
            return this;
        }

        /// <summary>
        /// When specified version allows to have suffix. Suffix must contain whitespace.
        /// For example 1.0.0.0 (28.1.2019)
        /// </summary>
        /// <returns></returns>
        public FetchBuildVersionFromFileTask AllowSuffix()
        {
            _allowSuffix = true;
            return this;
        }

        /// <summary>
        /// When set task does not store Version information to session <see cref="BuildProps.BuildVersion"/>
        /// </summary>
        /// <returns></returns>
        public FetchBuildVersionFromFileTask DoNotSaveVersionToSession()
        {
            _doNotSaveVersionToSession = true;
            return this;
        }

        /// <summary>
        /// File name where project version will be retrieved from. If not set one of the default file names is used.
        /// Defaults: Changelog.md, ReleaseNotes.md, ReleaseNotes.txt, {ProductId from session}.ProjectVersion.md, {ProductId from session}.ProjectVersion.txt
        /// </summary>
        public FetchBuildVersionFromFileTask ProjectVersionFileName(string projectVersionFileName)
        {
            _projectVersionFiles.Add(projectVersionFileName);
            return this;
        }

        protected override BuildVersion DoExecute(ITaskContextInternal context)
        {
            string productRootDir = context.Properties.Get<string>(BuildProps.ProductRootDir, ".");
            string productId = context.Properties.Get<string>(BuildProps.ProductId, null);
            if (productId != null)
            {
                _projectVersionFiles.Add($"{productId}.ProjectVersion.txt");
                _projectVersionFiles.Add($"{productId}.ProjectVersion.md");
            }

            _projectVersionFiles.AddRange(_defaultprojectVersionFiles);
            string projectVersionFilePath = null;

            foreach (var projectVersionFile in _projectVersionFiles)
            {
                var filePath = Path.Combine(productRootDir, projectVersionFile);
                if (File.Exists(filePath))
                {
                    projectVersionFilePath = filePath;
                    break;
                }
            }

            if (projectVersionFilePath == null)
            {
                string defaultLocations = string.Empty;
                foreach (var projectVersionFile in _projectVersionFiles)
                {
                    defaultLocations = $"{defaultLocations}{Path.Combine(productRootDir, projectVersionFile)}{Environment.NewLine}";
                }

                throw new InvalidOperationException($"Project version file is missing. Set 'ProjectVersionFileName' or use one of the default locations: {Environment.NewLine}{defaultLocations}");
            }

            string versionQuality = null;
            Version version = null;
            context.LogInfo($"Fetching version from file: {projectVersionFilePath}");
            using (Stream stream = File.Open(projectVersionFilePath, FileMode.Open))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    string line;
                    bool versionFound = false;

                    while ((line = reader.ReadLine()) != null)
                    {
                        try
                        {
                            if (_prefixToRemove != null && line.StartsWith(_prefixToRemove))
                            {
                                line = line.Substring(_prefixToRemove.Length);
                            }

                            line = line.Trim();

                            if (_allowSuffix)
                            {
                               var index = line.IndexOf(' ');
                               if (index > 0)
                               {
                                   line = line.Remove(index);
                               }
                            }

                            if (line.Contains("-"))
                            {
                               var splitedVersion = line.Split('-');
                               if (splitedVersion.Length > 2)
                               {
                                   throw new TaskExecutionException("Only one dash is allowed for version quality.", 6);
                               }

                               version = new Version(splitedVersion[0].Trim());
                               versionQuality = splitedVersion[1].Trim();
                            }
                            else
                            {
                                version = new Version(line);
                            }

                            versionFound = true;

                            break;
                        }
                        catch (Exception)
                        {
                        }
                    }

                    if (!versionFound)
                    {
                        throw new TaskExecutionException($"Version information not found in file '{projectVersionFilePath}' File should contaion line with version e.g. '1.0.0.0'", -53);
                    }
                }
            }

            var buildVersion = new BuildVersion()
            {
                Version = version,
                VersionQuality = versionQuality
            };

            if (!_doNotSaveVersionToSession)
            {
                context.SetBuildVersion(buildVersion);
            }

            DoLogInfo($"Project version fetched: {buildVersion.Version}");
            return buildVersion;
        }
    }
}