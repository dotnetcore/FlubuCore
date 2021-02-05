using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using FlubuCore.Infrastructure.Terminal;

namespace FlubuCore.Commanding.Internal
{
    public partial class InternalCommandExecutor
    {
        private static readonly string _flubuFile = Path.Combine(".", ".flubu");

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
                var key = Console.ReadLine();

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
                var flubuConsole = new FlubuConsole(null, new List<Hint>(), options: o =>
                {
                    o.OnlyDirectoriesSuggestions = true;
                    o.IncludeFileSuggestions = true;
                    o.FileSuggestionsSearchPattern = "*.cs";
                    o.InitialText = "Enter path to script file (.cs) (enter to skip):";
                    o.WritePrompt = false;
                });
                buildScriptLocation = flubuConsole.ReadLine().Trim();

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
                var flubuConsole = new FlubuConsole(null, new List<Hint>(), options: o =>
                {
                    o.OnlyDirectoriesSuggestions = true;
                    o.IncludeFileSuggestions = true;
                    o.FileSuggestionsSearchPattern = "*.csproj";
                    o.WritePrompt = false;
                    o.InitialText = "Enter path to script project file(.csproj) location (enter to skip):";
                });
                csprojLocation = flubuConsole.ReadLine().Trim();
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
                var flubuConsole = new FlubuConsole(null, new List<Hint>(), options: o =>
                {
                    o.OnlyDirectoriesSuggestions = true;
                    o.IncludeFileSuggestions = true;
                    o.FileSuggestionsSearchPattern = "*.json";
                    o.WritePrompt = false;
                    o.InitialText = "Enter flubu settings file location (enter to skip):";
                });
                csprojLocation = flubuConsole.ReadLine().Trim();
                if (string.IsNullOrEmpty(csprojLocation) || (Path.GetExtension(csprojLocation) == ".json" && File.Exists(csprojLocation)))
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
