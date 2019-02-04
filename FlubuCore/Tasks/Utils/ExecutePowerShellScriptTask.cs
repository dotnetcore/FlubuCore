using System;
using System.Collections.Generic;
using System.Text;
using FlubuCore.Tasks.Process;

namespace FlubuCore.Tasks.Utils
{
    public class ExecutePowerShellScriptTask : ExternalProcessTaskBase<int, ExecutePowerShellScriptTask>
    {
        private readonly string _pathToPowerShellScript;
        private string _description;

        public ExecutePowerShellScriptTask(string pathToPowerShellScript)
        {
            _pathToPowerShellScript = pathToPowerShellScript;
            WithArguments($"&  {pathToPowerShellScript}");
            ExecutablePath = "powershell";
        }

        protected override string Description
        {
            get
            {
                if (string.IsNullOrEmpty(_description))
                {
                    return $"Executes powershell script {_pathToPowerShellScript}";
                }

                return _description;
            }

            set { _description = value; }
        }
    }
}
