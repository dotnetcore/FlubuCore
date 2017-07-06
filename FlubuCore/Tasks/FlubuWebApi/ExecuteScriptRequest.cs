using System;
using System.Collections.Generic;
using System.Text;

namespace FlubuCore.Tasks.FlubuWebApi
{
    public class ExecuteScriptRequest
    {
        public string MainCommand { get; set; }

        public List<string> RemainingCommands { get; set; }

        public string ScriptFilePathLocation { get; set; }
    }
}
