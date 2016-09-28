using System;
using System.IO;
using FlubuCore.Context;

namespace FlubuCore.Tasks.Solution
{
    public class FetchBuildVersionFromFileTask : TaskBase, IFetchBuildVersionTask
    {
        private readonly string _productRootDir;
        private readonly string _productId;
        private Version _buildVersion;

        public FetchBuildVersionFromFileTask(
            string productRootDir,
            string productId)
        {
            _productRootDir = productRootDir;
            _productId = productId;
        }

        public override string Description => "Fetch build version";

        public Version BuildVersion => _buildVersion;

        public string ProjectVersionFileName { get; set; }

        protected override int DoExecute(ITaskContext context)
        {
            string projectVersionFileName = !string.IsNullOrEmpty(ProjectVersionFileName)
                ? Path.Combine(_productRootDir, ProjectVersionFileName)
                : Path.Combine(_productRootDir, $"{_productId}.ProjectVersion.txt");

            if (!File.Exists(projectVersionFileName))
            {
                throw new InvalidOperationException($"Project version file '{projectVersionFileName}' is missing.");
            }

            using (Stream stream = File.Open(projectVersionFileName, FileMode.Open))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    string versionAsString = reader.ReadLine();
                    _buildVersion = new Version(versionAsString);
                }
            }

            context.SetBuildVersion(_buildVersion);
            context.WriteMessage($"Project build version (from file): {_buildVersion}");
            return 0;
        }
    }
}