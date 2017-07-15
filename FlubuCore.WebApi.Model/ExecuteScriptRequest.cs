using System.Collections.Generic;

namespace FlubuCore.WebApi.Model
{
    public class ExecuteScriptRequest
    {
        public string TargetToExecute { get; set; }

        public List<string> RemainingCommands { get; set; }

        public string ScriptFilePathLocation { get; set; }
    }
}
