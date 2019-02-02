using System;
using System.Collections.Generic;
using FlubuCore.Context;

namespace FlubuCore.Tasks.Versioning
{
    public class FetchVersionFromExternalSourceTask : TaskBase<Version, FetchVersionFromExternalSourceTask>
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

        protected override Version DoExecute(ITaskContextInternal context)
        {
            int? buildNumber = null, revisionNumber = null;
            if (!_disableDefaultBuildSystems)
            {
                switch (context.BuildSystems().RunningOn)
                {
                    case BuildSystemType.AppVeyor:
                        buildNumber = ParseBuildNumber(context.BuildSystems().AppVeyor().BuildNumber);
                        break;

                    case BuildSystemType.Bamboo:
                        buildNumber = ParseBuildNumber(context.BuildSystems().Bamboo().BuildNumber);
                        break;

                    case BuildSystemType.Bitrise:
                        buildNumber = ParseBuildNumber(context.BuildSystems().BitRise().BuildNumber);
                        break;

                    case BuildSystemType.ContinousCl:
                        buildNumber = ParseBuildNumber(context.BuildSystems().ContinuaCl().BuildNumber);
                        break;

                    case BuildSystemType.Jenkins:
                    {
                        buildNumber = ParseBuildNumber(context.BuildSystems().Jenkins().BuildNumber);
                        revisionNumber = ParseBuildNumber(context.BuildSystems().Jenkins().SvnRevisionId);
                        break;
                    }

                    case BuildSystemType.TFS:
                        buildNumber = ParseBuildNumber(context.BuildSystems().TeamFoundationServer().BuildNumber);
                        break;

                    case BuildSystemType.TeamCity:
                        buildNumber = ParseBuildNumber(context.BuildSystems().TeamCity().BuildNumber);
                        break;

                    case BuildSystemType.TravisCI:
                        buildNumber = ParseBuildNumber(context.BuildSystems().Travis().BuildNumber);
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

            Version current = context.Properties.GetBuildVersion();

            if (buildNumber == null && revisionNumber == null) return current;

            Version newVer = new Version(current.Major, current.Minor, buildNumber ?? current.Build,
                revisionNumber ?? current.Revision);

            context.SetBuildVersion(newVer);
            DoLogInfo($"Updated version to {newVer.ToString(4)}");
            return newVer;
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
