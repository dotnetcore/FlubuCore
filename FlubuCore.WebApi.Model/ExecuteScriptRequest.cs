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
            return $"TargetToExecute: '{TargetToExecute}' RemainingCommands: '{string.Join(", ", RemainingCommands)}', ScriptFileName '{ScriptFileName}', ScriptArguments:'{string.Join(";", ScriptArguments)}'.";
        }
    }
}
