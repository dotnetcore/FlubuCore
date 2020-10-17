using System;
using System.Collections.Generic;
using System.Linq;
using FlubuCore.Commanding.Internal;
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
            NoDependencies = arguments.NoDependencies;
            RethrowOnException = arguments.RethrowOnException;
            ScriptArguments = arguments.ScriptArguments;
            TargetsToExecute = arguments.TargetsToExecute;
        }

        public bool Help { get; set; }

        /// <summary>
        /// If <c>true</c> dotnet-flubu cli tool and flubu.runner shows whole excetpion stacktrace. otherwise only message.
        /// </summary>
        public bool Debug { get; set; }

        /// <summary>
        /// If <c>true</c> colored logging is disabled.
        /// </summary>
        public bool DisableColoredLogging { get; set; }

        public bool InteractiveMode { get; set; }

        public bool IsWebApi { get; set; }

        public List<string> AdditionalOptions { get; set; } = new List<string>();

        /// <summary>
        /// Location where <see cref="BuildScriptLocator"/> searches for build script.
        /// </summary>
        public string Script { get; set; }

        public string FlubuFileLocation { get; set; }

        /// <summary>
        /// Location's where <see cref="ScriptLoader"/> looks for assemblies to load.
        /// </summary>
        public List<string> AssemblyDirectories { get; set; } = new List<string>();

        public string FlubuHelpText { get; set; }

        public bool IsInternalCommand()
        {
            if (MainCommands == null || MainCommands.Count == 0)
            {
                return false;
            }

            var firstCommand = MainCommands.First();

            return firstCommand.Equals(InternalFlubuCommands.Setup, StringComparison.OrdinalIgnoreCase) ||
                   firstCommand.Equals(InternalFlubuCommands.New, StringComparison.OrdinalIgnoreCase);
        }
    }
}