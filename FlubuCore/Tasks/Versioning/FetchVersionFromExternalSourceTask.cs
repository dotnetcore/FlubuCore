using System;
using System.Collections.Generic;
using FlubuCore.Context;

namespace FlubuCore.Tasks.Versioning
{
    public class FetchVersionFromExternalSourceTask : TaskBase
    {
        private readonly List<string> _environmentVariables = new List<string>
        {
            "APPVEYOR_BUILD_VERSION"
        };

        public override string Description => "Fetch version from external source";

        protected override int DoExecute(ITaskContext context)
        {
            foreach (string itm in _environmentVariables)
            {
                string val = Environment.GetEnvironmentVariable(itm);

                if (!string.IsNullOrEmpty(val))
                {
                    Version current = context.GetBuildVersion();
                    var newVer = new Version(current.Major, current.Minor, int.Parse(val));
                    context.SetBuildVersion(newVer);
                    context.WriteMessage($"Updated version to {newVer.ToString(3)}");
                    break;
                }
            }

            return 0;
        }
    }
}
