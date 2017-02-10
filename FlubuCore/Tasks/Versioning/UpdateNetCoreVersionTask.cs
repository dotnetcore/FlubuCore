using System;
using System.Collections.Generic;
using System.IO;
using FlubuCore.Context;
using FlubuCore.IO.Wrappers;
using FlubuCore.Tasks.Text;

namespace FlubuCore.Tasks.Versioning
{
    public class UpdateNetCoreVersionTask : TaskBase<int>
    {
        private readonly List<string> _files = new List<string>();

        private readonly IPathWrapper _pathWrapper;

        private readonly IFileWrapper _file;

        private Version _version;

        public UpdateNetCoreVersionTask(IPathWrapper pathWrapper, IFileWrapper filWrapper, string file)
        {
            _file = filWrapper;
            _pathWrapper = pathWrapper;
            _files.Add(file);
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
                if (!_file.Exists(file))
                {
                    context.Fail($"File {file} not found!", 1);
                    return 1;
                }

                if (_pathWrapper.GetExtension(file).Equals(".xproj", StringComparison.CurrentCultureIgnoreCase))
                {
                    UpdateJsonFileTask task = context.Tasks().UpdateJsonFileTask(file);

                    task
                        .FailIfPropertyNotFound(false)
                        .Update("version", newVersion);

                    AdditionalProperties.ForEach(i => task.Update(i, newVersion));

                    task.Execute(context);
                }
                else
                {
                    var task = context.Tasks().UpdateXmlFileTask(file);
                    task.AddOrUpdate("//Version", newVersion);
                    task.Execute(context);
                }
            }

            return res;
        }
    }
}