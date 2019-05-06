using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using FlubuCore.Context;
using FlubuCore.Infrastructure;
using FlubuCore.IO.Wrappers;
using FlubuCore.Scripting;
using FlubuCore.Targeting;
using Microsoft.Extensions.Logging;

namespace FlubuCore.Commanding
{
    public class CommandExecutor : ICommandExecutor
    {
        private readonly CommandArguments _args;

        private readonly IScriptLoader _scriptLoader;
        private readonly ITaskSession _taskSession;
        private readonly IFileWrapper _file;
        private readonly IPathWrapper _path;

        private readonly ILogger<CommandExecutor> _log;

        public CommandExecutor(
            CommandArguments args,
            IScriptLoader scriptLoader,
            ITaskSession taskSession,
            IFileWrapper file,
            IPathWrapper path,
            ILogger<CommandExecutor> log)
        {
            _args = args;
            _scriptLoader = scriptLoader;
            _taskSession = taskSession;
            _file = file;
            _path = path;
            _log = log;
        }

        public string FlubuHelpText { get; set; }

        public async Task<int> ExecuteAsync()
        {
            var version = Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyFileVersionAttribute>().Version;

            if (_args.DisableColoredLogging)
            {
                FlubuConsoleLogger.DisableColloredLogging = true;
            }

            _log.LogInformation($"Flubu v.{version}");

            if (_args.Help) return 1;

            if (_args.MainCommands.Count == 1 && _args.MainCommands.First().Equals("setup", StringComparison.CurrentCultureIgnoreCase))
            {
                SetupFlubu();
                return 0;
            }

            try
            {
                var script = await _scriptLoader.FindAndCreateBuildScriptInstanceAsync(_args);

                _taskSession.FlubuHelpText = FlubuHelpText;
                _taskSession.ScriptArgs = _args.ScriptArguments;
                var result = script.Run(_taskSession);
                return result;
            }
            catch (TaskExecutionException e)
            {
                  if (_args.RethrowOnException)
                    throw;

                 _log.Log(LogLevel.Error, 1, $"EXECUTION FAILED:\r\n{e.ToString()}", null, (t, ex) => t);
                 return StatusCodes.BuildScriptNotFound;
            }
            catch (FlubuException e)
            {
                if (_args.RethrowOnException)
                    throw;

                var str = _args.Debug ? e.ToString() : e.Message;
                _log.Log(LogLevel.Error, 1, $"EXECUTION FAILED:\r\n{str}", null, (t, ex) => t);
                return StatusCodes.BuildScriptNotFound;
            }
            catch (Exception e)
            {
                if (_args.RethrowOnException)
                    throw;

                var str = _args.Debug ? e.ToString() : e.Message;
                _log.Log(LogLevel.Error, 1, $"EXECUTION FAILED:\r\n{str}", null, (t, ex) => t);
                return 3;
            }
        }

        private void SetupFlubu()
        {
            string buildScriptLocation = null;
            string csprojLocation = null;
            if (_file.Exists("./.flubu"))
            {
                var lines = _file.ReadAllLines("./.flubu");

                if (lines.Count >= 1)
                {
                    buildScriptLocation = lines[0];
                }

                if (lines.Count >= 2)
                {
                    csprojLocation = lines[1];
                }
            }

            if (buildScriptLocation != null && _file.Exists(buildScriptLocation))
            {
                Console.WriteLine($"Script '{buildScriptLocation}' found nothing to do.");
                return;
            }

            if (string.IsNullOrEmpty(buildScriptLocation))
            {
                foreach (var defaultScriptLocation in BuildScriptLocator.DefaultScriptLocations)
                {
                    if (_file.Exists(defaultScriptLocation))
                    {
                        buildScriptLocation = defaultScriptLocation;
                    }
                }

                if (!string.IsNullOrEmpty(buildScriptLocation))
                {
                    Console.WriteLine($"Script '{buildScriptLocation}' found nothing to do.");
                    return;
                }
            }

            bool scriptFound = false;
            while (!scriptFound)
            {
                Console.Write("Enter script location (enter to skip): ");
                buildScriptLocation = Console.ReadLine();
                if (string.IsNullOrEmpty(buildScriptLocation) ||
                    (_path.GetExtension(buildScriptLocation) == ".cs" && _file.Exists(buildScriptLocation)))
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
                    (_path.GetExtension(csprojLocation) == ".csproj" && File.Exists(csprojLocation)))
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