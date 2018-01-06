using System.Collections.Generic;

namespace FlubuCore.Scripting
{
    public class CommandArguments
    {
        public string Config { get; set; }

        public bool Help { get; set; }

        public bool Debug { get; set; }

        public List<string> MainCommands { get; set; }

        public string Output { get; set; }

        public List<string> RemainingCommands { get; set; } = new List<string>();

        public string Script { get; set; }

        public List<string> TargetsToExecute { get; set; }

        public bool ExecuteTargetsInParallel { get; set; }

        public DictionaryWithDefault<string, string> ScriptArguments { get; set; } = new DictionaryWithDefault<string, string>();

        /// <summary>
        /// Gets or sets a value indicating whether <c>true</c> specified target is unknown flubu treat this as exception.
        /// Otherwise help target is runned.
        /// </summary>
        public bool TreatUnknownTargetAsException { get; set; }

        /// <summary>
        /// If <c>true</c> flubu rethrows exception when occures. Otherwise status code is returned.
        /// </summary>
        public bool RethrowOnException { get; set; }

        public List<string> AssemblyDirectories { get; set; } = new List<string>();
    }
}