using System.Collections.Generic;

namespace FlubuCore.Scripting
{
    public class CommandArguments
    {
        public string Config { get; set; }

        public bool Help { get; set; }

        public List<string> MainCommands { get; set; }

        public string Output { get; set; }

        public List<string> RemainingCommands { get; set; }

        public string Script { get; set; }

        public List<string> TargetsToExecute { get; set; }


	    public DictionaryWithDefault<string, string> ScriptArguments;
		
        /// <summary>
        /// If <c>true</c> specified target is unknown flubu treat this as exception. Otherwise help target is runned.
        /// </summary>
        public bool TreatUnknownTargetAsException { get; set; }
        
        /// <summary>
        /// If <c>true</c> flubu rethrows exception when occures. Otherwise status code is returned. 
        /// </summary>
        public bool RethrowOnException { get; set; }
    }
}