using System.Threading.Tasks;
using FlubuCore.Scripting;
using Microsoft.Extensions.Logging;

namespace DotNet.Cli.Flubu.Scripting
{
    public class BuildScriptLocator : IBuildScriptLocator
    {
        internal static readonly string[] DefaultScriptLocations =
        {
            "buildscript.cs",
            "BuildScript.cs",
            "deployscript.cs",
            "DeployScript.cs",
            "buildscript\\buildscript.cs",
            "buildscripts\\buildscript.cs"
        };

        private readonly ILogger<BuildScriptLocator> _log;

        private readonly IFileExistsService _fileExistsService;

        private readonly IScriptLoader _scriptLoader;

        public BuildScriptLocator(
            IFileExistsService fileExistsService,
            ILogger<BuildScriptLocator> log,
            IScriptLoader scriptLoader)
        {
            _fileExistsService = fileExistsService;
            _scriptLoader = scriptLoader;
            _log = log;
        }

        public Task<IBuildScript> FindBuildScript(CommandArguments args)
        {
            string fileName = GetFileName(args);

            return FindAndCreateBuildScriptInstanceAsync(fileName);
        }

        private Task<IBuildScript> FindAndCreateBuildScriptInstanceAsync(string fileName)
        {
            return _scriptLoader.FindAndCreateBuildScriptInstanceAsync(fileName);
        }

        private string GetFileName(CommandArguments args)
        {
            if (!string.IsNullOrEmpty(args.Script))
            {
                return TakeExplicitBuildScriptName(args);
            }

            _log.LogInformation("Build script file name was not explicitly specified, searching the default locations:");

            foreach (var defaultScriptLocation in DefaultScriptLocations)
            {
                if (_fileExistsService.FileExists(defaultScriptLocation))
                {
                    _log.LogInformation("Found it, using the build script file '{0}'.", defaultScriptLocation);
                    return defaultScriptLocation;
                }
            }

            return null;
        }

        private string TakeExplicitBuildScriptName(CommandArguments args)
        {
            if (_fileExistsService.FileExists(args.Script))
            {
                return args.Script;
            }

            throw new BuildScriptLocatorException($"The build script file specified ('{args.Script}') does not exist.");
        }
    }
}