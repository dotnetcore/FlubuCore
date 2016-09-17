using System.Collections.Generic;

namespace flubu.Scripting
{
    public class CommandArguments
    {
        public string BuildBasePath { get; set; }
        public string Config { get; set; }
        public bool Help { get; set; }
        public string MainCommand { get; set; }
        public string Output { get; set; }
        public string ProjectPath { get; set; }
        public List<string> RemainingCommands { get; set; }
        public string Script { get; set; }
    }
}
