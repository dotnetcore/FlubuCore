using System;
using System.Collections.Generic;
using FlubuCore.Context;

namespace FlubuCore.Tasks.Versioning
{
    public class FetchVersionFromExternalSourceTask : TaskBase<Version, FetchVersionFromExternalSourceTask>
    {
        private readonly List<string> _buildNumbers = new List<string>
        {
            "APPVEYOR_BUILD_NUMBER",
            "BUILD_NUMBER",
        };

        private readonly List<string> _revisionNumbers = new List<string>();

        private string _description;

        protected override string Description
        {
            get => string.IsNullOrEmpty(_description) ? "Fetches version (build and revision) from environment variables." : _description;
            set => _description = value;
        }

        public FetchVersionFromExternalSourceTask WithBuildNumber(string envName)
        {
            if (_buildNumbers.Contains(envName))
                return this;

            _buildNumbers.Add(envName);

            return this;
        }

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

        protected override Version DoExecute(ITaskContextInternal context)
        {
            int? buildNumber = null, revisionNumber = null;

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
            context.LogInfo($"Updated version to {newVer.ToString(4)}");
            return newVer;
        }
    }
}
