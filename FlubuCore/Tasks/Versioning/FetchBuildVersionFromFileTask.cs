using System;
using System.IO;
using FlubuCore.Context;
using FlubuCore.Tasks.Solution;

namespace FlubuCore.Tasks.Versioning
{
    public class FetchBuildVersionFromFileTask : TaskBase<Version, FetchBuildVersionFromFileTask>, IFetchBuildVersionTask
    {
        private bool _doNotSaveVersionToSession;
        private string _productRootDir;
        private string _productId;

        /// <summary>
        /// File name where project version will be retrived from. If not set default filane is {SolutionName}.ProjectVersion.txt
        /// </summary>
        private string _projectVersionFileName;

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
        /// File name where project version will be retrived from. If not set default file name is {ProductId from session}.ProjectVersion.txt
        /// </summary>
        public FetchBuildVersionFromFileTask ProjectVersionFileName(string projectVersionFileName)
        {
            _projectVersionFileName = projectVersionFileName;
            return this;
        }

        protected override Version DoExecute(ITaskContextInternal context)
        {
            _productRootDir = context.Properties.Get<string>(BuildProps.ProductRootDir);

            if (string.IsNullOrEmpty(_productRootDir))
                _productRootDir = ".";

            string projectVersionFileName;

            if (!string.IsNullOrEmpty(_projectVersionFileName))
            {
                projectVersionFileName = Path.Combine(_productRootDir, _projectVersionFileName);
            }
            else
            {
                _productId = context.Properties.Get<string>(BuildProps.ProductId);
                projectVersionFileName = Path.Combine(_productRootDir, $"{_productId}.ProjectVersion.txt");
            }

            if (!File.Exists(projectVersionFileName))
                throw new InvalidOperationException($"Project version file '{projectVersionFileName}' is missing.");

            Version buildVersion;
            using (Stream stream = File.Open(projectVersionFileName, FileMode.Open))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    string versionAsString = reader.ReadLine();
                    buildVersion = new Version(versionAsString);
                }
            }

            if (!_doNotSaveVersionToSession)
            {
                context.SetBuildVersion(buildVersion);
            }

            context.LogInfo($"Project build version (from file): {buildVersion}");
            return buildVersion;
        }
    }
}