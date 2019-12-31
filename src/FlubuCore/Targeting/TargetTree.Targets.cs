using System;
using System.Collections.Generic;
#if !NETSTANDARD1_6
using System.Drawing;
#endif
using System.IO;
using FlubuCore.Context;
using FlubuCore.Scripting;
using FlubuCore.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace FlubuCore.Targeting
{
    public partial class TargetTree
    {
        /// <summary>
        ///     The target for displaying help in the command line.
        /// </summary>
        /// <param name="context">The task context.</param>
        public void LogTargetsWithHelp(ITaskContextInternal context)
        {
            if (context != null && !string.IsNullOrEmpty(context.FlubuHelpText))
            {
                context.LogInfo(context.FlubuHelpText);
            }

            LogTargetsHelp(context);
        }

        public void LogTargetsHelp(ITaskContextInternal context)
        {
            context.DecreaseDepth();
            context.LogInfo("Targets:");

            // first sort the targets
            var sortedTargets = new SortedList<string, ITargetInternal>();

            foreach (var target in _targets.Values)
            {
                sortedTargets.Add(target.TargetName, target);
            }

            // now display them in sorted order
            foreach (ITargetInternal target in sortedTargets.Values)
            {
                if (target.IsHidden == false)
                {
                    string help = $"  {target.TargetName}";

                    if (target.Dependencies != null && target.Dependencies.Count != 0)
                    {
                        help = $"{help} ({string.Join(", ", target.Dependencies.Keys)})";
                    }

                    help = $"{help} : {target.Description}";

                    if (DefaultTargets.Contains(target))
                    {
#if !NETSTANDARD1_6
                        context.LogInfo(help, Color.DarkOrange);
#else
                        context.LogInfo(help);
#endif
                    }
                    else
                    {
                        context.LogInfo(help);
                    }
                }
            }

            if (ScriptArgsHelp?.Count > 0)
            {
                context.LogInfo(" ");
                context.LogInfo("Global build script arguments:");
                foreach (var argHelp in ScriptArgsHelp)
                {
                    context.LogInfo($"  {argHelp}");
                }
            }

            context.IncreaseDepth();
        }

        public void LogTasksHelp(ITaskContextInternal context)
        {
            context.LogInfo("Tasks:");

            // first sort the targets
            IEnumerable<ITask> tasks = _provider.GetServices<ITask>();

            // now display them in sorted order
            foreach (ITask task in tasks)
            {
                context.LogInfo($"  {task.GetType().FullName}");
            }
        }

        internal static void SetupFlubu()
        {
            bool scriptFound = false;
            string buildScriptLocation = null;
            string csprojLocation = null;
            if (File.Exists("./.flubu"))
            {
                var lines = File.ReadAllLines("./.flubu");

                if (lines.Length >= 1)
                {
                    buildScriptLocation = lines[0];
                }

                if (lines.Length >= 2)
                {
                    csprojLocation = lines[1];
                }
            }

            if (buildScriptLocation != null && File.Exists(buildScriptLocation))
            {
                Console.WriteLine($"Script '{buildScriptLocation}' found nothing to do.");
                return;
            }

            if (string.IsNullOrEmpty(buildScriptLocation))
            {
                foreach (var defaultScriptLocation in BuildScriptLocator.DefaultScriptLocations)
                {
                    if (File.Exists(defaultScriptLocation))
                    {
                        buildScriptLocation = defaultScriptLocation;
                    }
                }

                if (!string.IsNullOrEmpty(buildScriptLocation))
                {
                    Console.WriteLine($"Script '{buildScriptLocation}' found. No need to enter build script location");
                    scriptFound = true;
                }
            }

            while (!scriptFound)
            {
                Console.Write("Enter script location (enter to skip): ");
                buildScriptLocation = Console.ReadLine();
                if (string.IsNullOrEmpty(buildScriptLocation) ||
                    (Path.GetExtension(buildScriptLocation) == ".cs" && File.Exists(buildScriptLocation)))
                {
                    scriptFound = true;
                }
                else
                {
                    Console.WriteLine("Script file not found.");
                }
            }

            bool csprojFound = false;

            while (!csprojFound)
            {
                Console.Write("Enter script project file(csproj) location (enter to skip): ");
                csprojLocation = Console.ReadLine();
                if (string.IsNullOrEmpty(csprojLocation) ||
                    (Path.GetExtension(csprojLocation) == ".csproj" && File.Exists(csprojLocation)))
                {
                    csprojFound = true;
                }
                else
                {
                    Console.WriteLine("Project file not found.");
                }
            }

            if (!string.IsNullOrEmpty(buildScriptLocation) || !string.IsNullOrEmpty(csprojLocation))
            {
                List<string> textLines = new List<string> { buildScriptLocation, csprojLocation };
                File.WriteAllLines("./.flubu", textLines);
                Console.WriteLine(".flubu file created! You should add it to the source control.");
            }
        }
    }
}
