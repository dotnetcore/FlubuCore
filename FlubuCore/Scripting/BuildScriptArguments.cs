using System;
using System.Collections.Generic;
using System.Text;
using FlubuCore.Context;
using FlubuCore.Infrastructure;

namespace FlubuCore.Scripting
{
    public class BuildScriptArguments
    {
        /// <summary>
        /// The list of main commands (targets) to be executed.
        /// </summary>
        public List<string> MainCommands { get; set; }

        /// <summary>
        /// If <c>true</c> no target dependencies are executed.
        /// </summary>
        public bool NoDependencies { get; set; }

        /// <summary>
        /// List of all targets(also dependencies) that must be executed. If they are not script execution fails.
        /// </summary>
        public List<string> TargetsToExecute { get; set; }

        /// <summary>
        /// If <c>true</c> Target's provided in <see cref="MainCommands"/> are executed in parallel.
        /// </summary>
        public bool ExecuteTargetsInParallel { get; set; }

        /// <summary>
        /// Script Argument that can be accessed in build script through <see cref="IBuildPropertiesContext.ScriptArgs"/>
        /// </summary>
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
    }
}
