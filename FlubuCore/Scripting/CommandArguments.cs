using System.Collections.Generic;
using FlubuCore.Context;
using FlubuCore.Infrastructure;

namespace FlubuCore.Scripting
{
    public class CommandArguments : BuildScriptArguments
    {
        public CommandArguments()
        {
        }

        public CommandArguments(BuildScriptArguments arguments)
        {
            MainCommands = arguments.MainCommands;
            ExecuteTargetsInParallel = arguments.ExecuteTargetsInParallel;
            NoDependencies = arguments.ExecuteTargetsInParallel;
            RethrowOnException = arguments.RethrowOnException;
            ScriptArguments = arguments.ScriptArguments;
            TargetsToExecute = arguments.TargetsToExecute;
        }

        public bool Help { get; set; }

        /// <summary>
        /// If <c>true</c> dotnet-flubu cli tool and flubu.runner shows whole excetpion stacktrace. otherwise only message.
        /// </summary>
        public bool Debug { get; set; }

        public List<string> RemainingCommands { get; set; } = new List<string>();

        /// <summary>
        /// Location where <see cref="BuildScriptLocator"/> searches for build script.
        /// </summary>
        public string Script { get; set; }

        /// <summary>
        /// Location's where <see cref="ScriptLoader"/> looks for assemblies to load.
        /// </summary>
        public List<string> AssemblyDirectories { get; set; } = new List<string>();
    }
}