using System.Threading.Tasks;
using FlubuCore.IO.Wrappers;
using Microsoft.Extensions.Logging;

namespace FlubuCore.Scripting
{
    public class BuildScriptLocator : IBuildScriptLocator
    {
        public static readonly string[] DefaultScriptLocations =
        {
            "buildscript.cs",
            "BuildScript.cs",
            "deployscript.cs",
            "DeployScript.cs",
            "buildscript\\buildscript.cs",
            "buildscripts\\buildscript.cs",
        };

        private readonly ILogger<BuildScriptLocator> _log;

        private readonly IFileWrapper _file;

        private readonly IScriptLoader _scriptLoader;

        public BuildScriptLocator(
            IFileWrapper file,
            ILogger<BuildScriptLocator> log,
            IScriptLoader scriptLoader)
        {
            _file = file;
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
                if (_file.Exists(defaultScriptLocation))
                {
                    _log.LogInformation("Found it, using the build script file '{0}'.", defaultScriptLocation);
                    return defaultScriptLocation;
                }
            }

            return null;
        }

        private string TakeExplicitBuildScriptName(CommandArguments args)
        {
            if (_file.Exists(args.Script))
            {
                return args.Script;
            }

            throw new BuildScriptLocatorException($"The build script file specified ('{args.Script}') does not exist.");
        }
    }
}