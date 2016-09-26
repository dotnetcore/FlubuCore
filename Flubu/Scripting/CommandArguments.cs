using System.Collections.Generic;

namespace Flubu.Scripting
{
    public class CommandArguments
    {
        public string Config { get; set; }

        public bool Help { get; set; }

        public string MainCommand { get; set; }

        public string Output { get; set; }

        public List<string> RemainingCommands { get; set; }

        public string Script { get; set; }

        public string ScriptAssembly { get; set; }
    }
}