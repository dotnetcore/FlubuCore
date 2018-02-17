using System;
using System.Collections.Generic;

namespace FlubuCore.WebApi.Model
{
    public class ExecuteScriptRequest
    {
        public string TargetToExecute { get; set; }

        public List<string> RemainingCommands { get; set; }

        public string ScriptFileName { get; set; }

        public Dictionary<string, string> ScriptArguments { get; set; }

        public override string ToString()
        {
            string remainingCommans = null;
            if (RemainingCommands != null)
            {
                remainingCommans = $"RemainingCommands: '{string.Join(", ", RemainingCommands)}'";
            }

            string scriptArguments = null;
            if (ScriptArguments != null)
            {
                scriptArguments = $"ScriptArguments:'{string.Join(";", ScriptArguments)}'";
            }

            return $"TargetToExecute: '{TargetToExecute}' , ScriptFileName '{ScriptFileName}', {scriptArguments} {remainingCommans}.";
        }
    }
}
