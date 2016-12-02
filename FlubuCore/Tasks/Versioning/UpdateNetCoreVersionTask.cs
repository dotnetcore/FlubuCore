using System;
using System.Collections.Generic;
using FlubuCore.Context;
using FlubuCore.Tasks.Text;

namespace FlubuCore.Tasks.Versioning
{
    public class UpdateNetCoreVersionTask : TaskBase<int>
    {
        private readonly List<string> _files = new List<string>();

        private Version _version;

        public UpdateNetCoreVersionTask(string filePath)
        {
            if (!string.IsNullOrEmpty(filePath))
            {
                _files.Add(filePath);
            }
        }

        public UpdateNetCoreVersionTask(params string[] files)
        {
            _files.AddRange(files);
        }

        internal List<string> AdditionalProperties { get; } = new List<string>();

        public UpdateNetCoreVersionTask FixedVersion(Version version)
        {
            _version = version;
            return this;
        }

        public UpdateNetCoreVersionTask AdditionalProp(params string[] args)
        {
            if (args == null || args.Length <= 0)
                return this;

            AdditionalProperties.AddRange(args);
            return this;
        }

        public UpdateNetCoreVersionTask AddFiles(params string[] files)
        {
            _files.AddRange(files);
            return this;
        }

        protected override int DoExecute(ITaskContextInternal context)
        {
            if (_version == null)
            {
                _version = context.Properties.GetBuildVersion();
            }

            if (_version == null)
            {
                throw new TaskExecutionException("Version is not set!", 1);
            }

            context.LogInfo($"Update version to {_version}");

            string newVersion = _version.ToString(3);
            int res = 0;

            foreach (string file in _files)
            {
                UpdateJsonFileTask task = new UpdateJsonFileTask(file);

                task
                    .FailIfPropertyNotFound(false)
                    .Update("version", newVersion);

                AdditionalProperties.ForEach(i => task.Update(i, newVersion));

                task.Execute(context);
            }

            return res;
        }
    }
}