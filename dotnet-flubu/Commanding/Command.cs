using NuGet.Frameworks;
using System.Collections.Generic;

namespace flubu.Commanding
{
    public class Command
    {
        public string BuildBasePath { get; set; }
        public string MainCommand { get; set; }

        public string Config { get; set; }
        public NuGetFramework Framework { get; set; }
        public bool Help { get; set; }
        public string Output { get; set; }
        public int? ParentProcessId { get; set; }
        public string ProjectPath { get; set; }
        public List<string> RemainingCommands { get; set; }
    }
}
