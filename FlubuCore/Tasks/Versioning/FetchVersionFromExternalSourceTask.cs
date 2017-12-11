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

        protected override string Description { get; set; }

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
            string buildNumber = "0";
            string revisionNumber = "0";
            foreach (string itm in _buildNumbers)
            {
                string val = Environment.GetEnvironmentVariable(itm);

                if (!string.IsNullOrEmpty(val))
                {
                    buildNumber = val;
                    break;
                }
            }

            foreach (string itm in _revisionNumbers)
            {
                string val = Environment.GetEnvironmentVariable(itm);

                if (!string.IsNullOrEmpty(val))
                {
                    revisionNumber = val;
                    break;
                }
            }

            Version current = context.Properties.GetBuildVersion();
            Version newVer = new Version(current.Major, current.Minor, int.Parse(buildNumber),
                int.Parse(revisionNumber));

            context.SetBuildVersion(newVer);
            context.LogInfo($"Updated version to {newVer.ToString(3)}");
            return newVer;
        }
    }
}
