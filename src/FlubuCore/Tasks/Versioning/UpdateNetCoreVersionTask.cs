using System;
using System.Collections.Generic;
using FlubuCore.Context;
using FlubuCore.IO.Wrappers;
using FlubuCore.Tasks.Text;

namespace FlubuCore.Tasks.Versioning
{
    public class UpdateNetCoreVersionTask : TaskBase<int, UpdateNetCoreVersionTask>
    {
        private readonly List<string> _files = new List<string>();

        private readonly IPathWrapper _pathWrapper;

        private readonly IFileWrapper _file;

        private BuildVersion _version;

        private string _description;

        private bool _addPackageVersion;

        private int? _packageVersionFieldCount;

        private int? _versionFieldCount;

        public UpdateNetCoreVersionTask(IPathWrapper pathWrapper, IFileWrapper filWrapper, string file)
        {
            _file = filWrapper;
            _pathWrapper = pathWrapper;
            _files.Add(file);
        }

        protected override string Description
        {
            get
            {
                if (string.IsNullOrEmpty(_description))
                {
                    return $"Updates version, AssemblyVersion and FileVersion in csproj/project.json file.";
                }

                return _description;
            }

            set => _description = value;
        }

        internal List<string> AdditionalProperties { get; } = new List<string>();

        /// <summary>
        /// Use fixed version instead of fetching one from <see cref="IBuildPropertiesSession"/> build property named: <see cref="BuildProps.BuildVersion"/>
        /// </summary>
        /// <param name="version"></param>
        /// <returns></returns>
        [Obsolete("Use set version instead")]
        public UpdateNetCoreVersionTask FixedVersion(BuildVersion version)
        {
            _version = version;
            return this;
        }

        /// <summary>
        /// Set version field count in csproj Version property. Default is 3
        /// </summary>
        /// <param name="versionFieldCount"></param>
        /// <returns></returns>
        public UpdateNetCoreVersionTask SetVersionFieldCount(int versionFieldCount)
        {
            _versionFieldCount = versionFieldCount;
            return this;
        }

        /// <summary>
        /// If applied PackageVersion property is added/updated in csproj.
        /// </summary>
        /// <param name="versionFieldCount">version field count in csproj PackageVersion property. Default is 3</param>
        /// <returns></returns>
        public UpdateNetCoreVersionTask AddPackageVersion(int? versionFieldCount = null)
        {
            _addPackageVersion = true;
            _packageVersionFieldCount = versionFieldCount;
            return this;
        }

        /// <summary>
        /// Use fixed version instead of fetching one from <see cref="IBuildPropertiesSession"/> build property named: <see cref="BuildProps.BuildVersion"/>
        /// </summary>
        /// <param name="version"></param>
        /// <returns></returns>
        [Obsolete]
        public UpdateNetCoreVersionTask SetVersion(BuildVersion version)
        {
            _version = version;
            return this;
        }

        /// <summary>
        /// Adds additional properties to be updated with the version.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public UpdateNetCoreVersionTask AdditionalProp(params string[] args)
        {
            if (args == null || args.Length <= 0)
                return this;

            AdditionalProperties.AddRange(args);
            return this;
        }

        /// <summary>
        /// Adds Project (json/cproj) files to be updated.
        /// </summary>
        /// <param name="files"></param>
        /// <returns></returns>
        public UpdateNetCoreVersionTask AddFiles(params string[] files)
        {
            _files.AddRange(files);
            return this;
        }

        protected override int DoExecute(ITaskContextInternal context)
        {
            if (_version == null)
            {
                _version = new BuildVersion();
                _version.Version = context.Properties.GetBuildVersion();
                _version.VersionQuality = context.Properties.GetBuildVersionQuality();
            }

            if (_version == null || _version.Version == null)
            {
                throw new TaskExecutionException("Version is not set!", 1);
            }

            DoLogInfo($"Update version to {_version.Version}");

            if (!_versionFieldCount.HasValue)
            {
                _versionFieldCount = context.Properties.TryGet(BuildProps.ProductVersionFieldCount, 3);
            }

            if (!_packageVersionFieldCount.HasValue)
            {
                _packageVersionFieldCount = context.Properties.TryGet(BuildProps.ProductVersionFieldCount, 3);
            }

            string newVersion = _version.Version.ToString(_versionFieldCount.Value);
            string newPackageVersion = _version.Version.ToString(_packageVersionFieldCount.Value);
            string newVersionWithQuality = newVersion;
            string newPackageVersionWithQuality = newPackageVersion;

            if (!string.IsNullOrEmpty(_version.VersionQuality))
            {
                if (!_version.VersionQuality.StartsWith("-"))
                {
                    _version.VersionQuality = _version.VersionQuality.Insert(0, "-");
                    newVersionWithQuality = $"{newVersionWithQuality}{_version.VersionQuality}";
                    newPackageVersionWithQuality = $"{newPackageVersionWithQuality}{_version.VersionQuality}";
                }
            }

            foreach (string file in _files)
            {
                if (string.IsNullOrEmpty(file))
                {
                    continue;
                }

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
                        .Update("version", newVersionWithQuality);

                    AdditionalProperties.ForEach(i => task.Update(i, newVersion));

                    task.Execute(context);
                }
                else
                {
                    var task = context.Tasks().UpdateXmlFileTask(file);
                    task.AddOrUpdate("Project/PropertyGroup/Version", newVersionWithQuality);
                    task.AddOrUpdate("Project/PropertyGroup/AssemblyVersion", _version.Version.ToString());
                    task.AddOrUpdate("Project/PropertyGroup/FileVersion", _version.Version.ToString());
                    if (_addPackageVersion)
                    {
                        task.AddOrUpdate("Project/PropertyGroup/PackageVersion", newPackageVersionWithQuality);
                    }

                    task.Execute(context);
                }
            }

            return 0;
        }
    }
}