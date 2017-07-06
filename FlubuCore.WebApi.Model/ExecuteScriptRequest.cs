using System.Collections.Generic;

namespace FlubuCore.WebApi.Model
{
    public class ExecuteScriptRequest
    {
        public string MainCommand { get; set; }

        public List<string> RemainingCommands { get; set; }

        public string ScriptFilePathLocation { get; set; }
    }
}
