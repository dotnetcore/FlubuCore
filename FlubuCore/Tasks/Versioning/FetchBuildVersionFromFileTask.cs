using System;
using System.Collections.Generic;
using System.IO;
using FlubuCore.Context;
using FlubuCore.Tasks.Solution;
using Microsoft.DotNet.Cli.Utils;

namespace FlubuCore.Tasks.Versioning
{
    public class FetchBuildVersionFromFileTask : TaskBase<Version, FetchBuildVersionFromFileTask>, IFetchBuildVersionTask
    {
        private static List<string> _defaultprojectVersionFiles = new List<string>()
        {
            "Changelog.md",
            "ReleaseNotes.md",
            "ReleaseNotes.txt",
        };

        private bool _doNotSaveVersionToSession;

        private string _productRootDir;

        private string _productId;

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

        protected override Version DoExecute(ITaskContextInternal context)
        {
            _productRootDir = context.Properties.Get<string>(BuildProps.ProductRootDir, ".");
            _productId = context.Properties.Get<string>(BuildProps.ProductId, null);
            if (_productId != null)
            {
                _projectVersionFiles.Add($"{_productId}.ProjectVersion.txt");
                _projectVersionFiles.Add($"{_productId}.ProjectVersion.md");
            }

            _projectVersionFiles.AddRange(_defaultprojectVersionFiles);
            string projectVersionFilePath = null;

            foreach (var projectVersionFile in _projectVersionFiles)
            {
                var filePath = Path.Combine(_productRootDir, projectVersionFile);
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
                    defaultLocations = $"{Path.Combine(_productRootDir, projectVersionFile)}{Environment.NewLine}";
                }

                throw new InvalidOperationException($"Project version file is missing. Set 'ProjectVersionFileName' or use one of the default locations: {Environment.NewLine}{defaultLocations}");
            }

            Version buildVersion = null;
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
                            buildVersion = new Version(line);
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

            if (!_doNotSaveVersionToSession)
            {
                context.SetBuildVersion(buildVersion);
            }

            DoLogInfo($"Project build version (from file): {buildVersion}");
            return buildVersion;
        }
    }
}