using System;
using System.IO;
using FlubuCore.Context;
using FlubuCore.Tasks.Solution;

namespace FlubuCore.Tasks.Versioning
{
    public class FetchBuildVersionFromFileTask : TaskBase<Version>, IFetchBuildVersionTask
    {
        private string _productRootDir;
        private string _productId;

        public string ProjectVersionFileName { get; set; }

        protected override Version DoExecute(ITaskContextInternal context)
        {
            _productRootDir = context.Properties.Get<string>(BuildProps.ProductRootDir);

            if (string.IsNullOrEmpty(_productRootDir))
                _productRootDir = ".";

            string projectVersionFileName;

            if (!string.IsNullOrEmpty(ProjectVersionFileName))
            {
                projectVersionFileName = Path.Combine(_productRootDir, ProjectVersionFileName);
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

            context.SetBuildVersion(buildVersion);
            context.LogInfo($"Project build version (from file): {buildVersion}");
            return buildVersion;
        }
    }
}