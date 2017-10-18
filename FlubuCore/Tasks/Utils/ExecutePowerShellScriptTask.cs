using System;
using System.Collections.Generic;
using System.Text;
using FlubuCore.Tasks.Process;

namespace FlubuCore.Tasks.Utils
{
    public class ExecutePowerShellScriptTask : ExternalProcessTaskBase<ExecutePowerShellScriptTask>
    {
        public ExecutePowerShellScriptTask(string pathToPowerShellScript)
        {
                WithArguments($"&  {pathToPowerShellScript}");
                ExecutablePath = "powershell";
        }
    }
}
