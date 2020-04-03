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
        private static readonly string _flubuFile = Path.Combine(".", ".flubu");

        /// <summary>
        ///     The target for displaying help in the command line.
        /// </summary>
        /// <param name="context">The task context.</param>
        public virtual void LogTargetsWithHelp(ITaskContextInternal context)
        {
            if (context != null && !string.IsNullOrEmpty(context.Args.FlubuHelpText))
            {
                context.LogInfo(context.Args.FlubuHelpText);
            }

            LogTargetsHelp(context);
        }

        public virtual void LogTargetsHelp(ITaskContextInternal context)
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

        public virtual void LogTasksHelp(ITaskContextInternal context)
        {
            context.LogInfo("Tasks:");

            IEnumerable<ITask> tasks = _provider.GetServices<ITask>();

            foreach (ITask task in tasks)
            {
                context.LogInfo($"  {task.GetType().FullName}");
            }
        }

        internal static void SetupFlubu()
        {
            string buildScriptLocation = null;
            string csprojLocation = null;
            string flubuSettingsLocation = string.Empty;
            bool setupFileExists = File.Exists(_flubuFile);
            if (setupFileExists)
            {
                var lines = File.ReadAllLines(_flubuFile);

                if (lines.Length >= 1)
                {
                    buildScriptLocation = lines[0];
                }

                if (lines.Length >= 2)
                {
                    csprojLocation = lines[1];
                }

                if (lines.Length >= 3)
                {
                    flubuSettingsLocation = lines[2];
                }
            }
            else
            {
               buildScriptLocation = ReadBuildScriptLocation();
               csprojLocation = ReadCsprojLocation();
            }

            bool exit = false;
            bool showHelp = true;
            string key;
            do
            {
                if (showHelp)
                {
                    Console.WriteLine(string.Empty);
                    Console.WriteLine("1 - Change script file (.cs) location");
                    Console.WriteLine("2 - Change csproj file (.csproj) location");
                    Console.WriteLine("3 - Change flubu settings file (.json) location");
                    Console.WriteLine("0 - Exit");
                    Console.WriteLine(string.Empty);
                }

                Console.Write("Choose: ");
                key = Console.ReadLine();

                switch (key)
                {
                    case "1":
                        buildScriptLocation = ReadBuildScriptLocation();
                        showHelp = true;
                        break;
                    case "2":
                        csprojLocation = ReadCsprojLocation();
                        showHelp = true;
                        break;
                    case "3":
                        flubuSettingsLocation = ReadFlubuSettingsLocation();
                        showHelp = true;
                        break;
                    case "0":
                        exit = true;
                        break;
                    default:
                        showHelp = false;
                        break;
                }
            }
            while (!exit);

            if (!string.IsNullOrEmpty(buildScriptLocation) || !string.IsNullOrEmpty(csprojLocation) || !string.IsNullOrEmpty(flubuSettingsLocation))
            {
                List<string> textLines = new List<string> { buildScriptLocation, csprojLocation };
                File.WriteAllLines(_flubuFile, textLines);
                Console.WriteLine(setupFileExists
                    ? ".flubu file modified!"
                    : ".flubu file created! You should add it to the source control.");
            }
        }

        private static string ReadBuildScriptLocation()
        {
            bool scriptFound = false;
            string buildScriptLocation = null;
            while (!scriptFound)
            {
                Console.Write("Enter path to script file (.cs) (enter to skip): ");
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

            return buildScriptLocation;
        }

        private static string ReadCsprojLocation()
        {
            bool csprojFound = false;
            string csprojLocation = null;
            while (!csprojFound)
            {
                Console.Write("Enter path to script project file(.csproj) location (enter to skip): ");
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

            return csprojLocation;
        }

        private static string ReadFlubuSettingsLocation()
        {
            bool csprojFound = false;
            string csprojLocation = null;
            while (!csprojFound)
            {
                Console.Write("Enter flubu settings file location (enter to skip): ");
                csprojLocation = Console.ReadLine();
                if (string.IsNullOrEmpty(csprojLocation) ||
                    (Path.GetExtension(csprojLocation) == ".json" && File.Exists(csprojLocation)))
                {
                    csprojFound = true;
                }
                else
                {
                    Console.WriteLine("flubu settings file not found.");
                }
            }

            return csprojLocation;
        }
    }
}
