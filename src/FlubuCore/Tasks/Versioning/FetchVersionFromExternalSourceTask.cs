using System;
using System.Collections.Generic;
using FlubuCore.Context;

namespace FlubuCore.Tasks.Versioning
{
    public class FetchVersionFromExternalSourceTask : TaskBase<BuildVersion, FetchVersionFromExternalSourceTask>
    {
        private readonly List<string> _buildNumbers = new List<string>();

        private readonly List<string> _revisionNumbers = new List<string>();

        private bool _disableDefaultBuildSystems;

        private string _description;

        protected override string Description
        {
            get => string.IsNullOrEmpty(_description) ? "Fetches version (build and revision) from build system environment variables." : _description;
            set => _description = value;
        }

        public FetchVersionFromExternalSourceTask WithBuildNumber(string envName)
        {
            if (_buildNumbers.Contains(envName))
                return this;

            _buildNumbers.Add(envName);

            return this;
        }

        /// <summary>
        /// Fetches revision number from given enviroment variable if it exists
        /// </summary>
        /// <param name="envName">Name of the enviroment variable</param>
        /// <returns></returns>
        public FetchVersionFromExternalSourceTask WithRevisionNumber(string envName)
        {
            if (_revisionNumbers.Contains(envName))
                return this;

            _revisionNumbers.Add(envName);

            return this;
        }

        public FetchVersionFromExternalSourceTask ClearBuildNumbers()
        {
            _buildNumbers.Clear();
            return this;
        }

        public FetchVersionFromExternalSourceTask ClearRevisionNumbers()
        {
            _revisionNumbers.Clear();
            return this;
        }

        /// <summary>
        /// Ignores fetching of build number and revision number from build systems that this task supports by default.
        /// </summary>
        /// <returns></returns>
        public FetchVersionFromExternalSourceTask IgnoreDefaultBuildSystems()
        {
            _disableDefaultBuildSystems = true;
            return this;
        }

        protected override BuildVersion DoExecute(ITaskContextInternal context)
        {
            int? buildNumber = null, revisionNumber = null;
            if (!_disableDefaultBuildSystems)
            {
                switch (context.BuildServers().RunningOn)
                {
                    case BuildServerType.AppVeyor:
                        buildNumber = ParseBuildNumber(context.BuildServers().AppVeyor().BuildNumber);
                        break;

                    case BuildServerType.Bamboo:
                        buildNumber = ParseBuildNumber(context.BuildServers().Bamboo().BuildNumber);
                        break;

                    case BuildServerType.Bitrise:
                        buildNumber = ParseBuildNumber(context.BuildServers().BitRise().BuildNumber);
                        break;

                    case BuildServerType.ContinousCl:
                        buildNumber = ParseBuildNumber(context.BuildServers().ContinuaCl().BuildNumber);
                        break;

                    case BuildServerType.Jenkins:
                    {
                        buildNumber = ParseBuildNumber(context.BuildServers().Jenkins().BuildNumber);
                        revisionNumber = ParseBuildNumber(context.BuildServers().Jenkins().SvnRevisionId);
                        break;
                    }

                    case BuildServerType.TFS:
                        buildNumber = ParseBuildNumber(context.BuildServers().TeamFoundationServer().BuildNumber);
                        break;

                    case BuildServerType.TeamCity:
                        buildNumber = ParseBuildNumber(context.BuildServers().TeamCity().BuildNumber);
                        break;

                    case BuildServerType.TravisCI:
                        buildNumber = ParseBuildNumber(context.BuildServers().Travis().BuildNumber);
                        break;
                }
            }

            foreach (string itm in _buildNumbers)
            {
                string val = Environment.GetEnvironmentVariable(itm);

                if (!string.IsNullOrEmpty(val))
                {
                    buildNumber = int.Parse(val);
                    break;
                }
            }

            foreach (string itm in _revisionNumbers)
            {
                string val = Environment.GetEnvironmentVariable(itm);

                if (!string.IsNullOrEmpty(val))
                {
                    revisionNumber = int.Parse(val);
                    break;
                }
            }

            BuildVersion current = context.Properties.GetBuildVersion();

            if (buildNumber == null && revisionNumber == null) return current;

            current.Version = new Version(current.Version.Major, current.Version.Minor, buildNumber ?? current.Version.Build,
                revisionNumber ?? current.Version.Revision);

            context.SetBuildVersion(current);
            DoLogInfo($"Updated version to {current.Version.ToString(4)}");
            return current;
        }

        private static int? ParseBuildNumber(string value)
        {
            int? buildNumber = null;
            if (!string.IsNullOrEmpty(value))
            {
                buildNumber = int.Parse(value);
            }

            return buildNumber;
        }
    }
}
