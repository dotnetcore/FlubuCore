using System;
using System.Collections.Generic;

namespace FlubuCore.WebApi.Model
{
    public class ExecuteScriptRequest
    {
        /// <summary>
        /// Target to execute.
        /// ErrorCodes: notempty_error.
        /// </summary>
        public string TargetToExecute { get; set; }

        /// <summary>
        /// Filename of the script to be executed.
        /// </summary>
        public string ScriptFileName { get; set; }

        /// <summary>
        /// The script arguments.
        /// </summary>
        public Dictionary<string, string> ScriptArguments { get; set; }

        public override string ToString()
        {
            string scriptArguments = null;
            if (ScriptArguments != null)
            {
                scriptArguments = $"ScriptArguments:'{string.Join(";", ScriptArguments)}'";
            }

            return $"TargetToExecute: '{TargetToExecute}' , ScriptFileName '{ScriptFileName}', {scriptArguments}.";
        }
    }
}
