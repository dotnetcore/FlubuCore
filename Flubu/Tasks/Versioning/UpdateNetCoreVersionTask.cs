using System;
using System.Collections.Generic;
using Flubu.Tasks.Text;

namespace Flubu.Tasks.Versioning
{
    public class UpdateNetCoreVersionTask : TaskBase
    {
        private readonly List<string> _files = new List<string>();
        private readonly List<string> _additionalProperies = new List<string>();

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

        public UpdateNetCoreVersionTask AdditionalProp(params string[] args)
        {
            _additionalProperies.AddRange(args);
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
            int res = 0;

            foreach (string file in _files)
            {
                UpdateJsonFileTask task = new UpdateJsonFileTask(file);

                task
                    .FailIfPropertyNotFound(false)
                    .Update("version", newVersion);

                _additionalProperies.ForEach(i => task.Update(i, newVersion));

                task.Execute(context);
            }

            return res;
        }
    }
}
