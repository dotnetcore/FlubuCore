using System;
using System.IO;
using FlubuCore.Context;

namespace FlubuCore.Tasks.Versioning
{
    public class FetchVersionFromFileTask : TaskBase
    {
        private string _versionFilename = "version.txt";

        public override string Description { get; } = "Fetch version from file";

        public FetchVersionFromFileTask UseFile(string fileName)
        {
            _versionFilename = fileName;
            return this;
        }

        protected override int DoExecute(ITaskContext context)
        {
            string ver = File.ReadAllText(_versionFilename);

            context.SetBuildVersion(Version.Parse(ver));
            return 0;
        }
    }
}
