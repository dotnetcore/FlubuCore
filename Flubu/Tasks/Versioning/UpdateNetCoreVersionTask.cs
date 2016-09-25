using System;
using System.Collections.Generic;
using Flubu.Tasks.Text;
using Newtonsoft.Json.Linq;

namespace Flubu.Tasks.Versioning
{
    public class UpdateNetCoreVersionTask : TaskBase
    {
        private readonly List<string> _files = new List<string>();

        private Version _version;

        public UpdateNetCoreVersionTask(string filePath)
        {
            _files.Add(filePath);
        }

        public UpdateNetCoreVersionTask(params string[] files)
        {
            _files.AddRange(files);
        }

        public override string Description => $"Update version to {_version}";

        public UpdateNetCoreVersionTask FixedVersion(Version version)
        {
            _version = version;
            return this;
        }

        protected override int DoExecute(ITaskContext context)
        {
            if (_version == null)
            {
                _version = context.Properties.TryGet<Version>("version");
            }

            if (_version == null)
            {
                throw new TaskExecutionException("Version is not set!");
            }

            string newVersion = _version.ToString(3);

            foreach (string file in _files)
            {
                UpdateJsonFileTask task = new UpdateJsonFileTask(file);
                task
                    .Update("version", newVersion)
                    .Execute(context);
            }

            return 0;
        }
    }
}
