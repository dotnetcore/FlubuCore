using System;
using System.Collections.Generic;
using FlubuCore.Context;

namespace FlubuCore.Tasks.Versioning
{
    public class FetchVersionFromExternalSourceTask : TaskBase<Version>
    {
        private readonly List<string> _environmentVariables = new List<string>
        {
            "APPVEYOR_BUILD_NUMBER",
            "BUILD_NUMBER"
        };

        protected override Version DoExecute(ITaskContextInternal context)
        {
            Version newVer = null;
            foreach (string itm in _environmentVariables)
            {
                string val = Environment.GetEnvironmentVariable(itm);

                if (!string.IsNullOrEmpty(val))
                {
                    Version current = context.GetBuildVersion();
                    newVer = new Version(current.Major, current.Minor, int.Parse(val));
                    context.SetBuildVersion(newVer);
                    context.LogInfo($"Updated version to {newVer.ToString(3)}");
                    break;
                }
            }

            return newVer;
        }
    }
}
