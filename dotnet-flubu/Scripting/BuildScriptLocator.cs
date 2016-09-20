using Microsoft.Extensions.Logging;
using System;
using System.Text;
using System.Threading.Tasks;

namespace flubu.Scripting
{
    public interface IBuildScriptLocator
    {
        Task<IBuildScript> FindBuildScript(CommandArguments args);
    }

    public class BuildScriptLocator : IBuildScriptLocator
    {
        private static readonly string[] defaultScriptLocations =
        {
            "buildscript.cs",
            "deployscript.cs",
            "buildscript\\buildscript.cs",
            "buildscripts\\buildscript.cs"
        };

        private readonly ILogger<BuildScriptLocator> _log;

        private readonly IFileExistsService fileExistsService;

        private readonly IScriptLoader scriptLoader;

        public BuildScriptLocator(IFileExistsService fileExistsService,
            ILogger<BuildScriptLocator> log,
            IScriptLoader scriptLoader)
        {
            this.fileExistsService = fileExistsService;
            this.scriptLoader = scriptLoader;
            _log = log;
        }

        public Task<IBuildScript> FindBuildScript(CommandArguments args)
        {
            string fileName = GetFileName(args);

            if (fileName == null)
                ReportUnspecifiedBuildScript();

            return FindAndCreateBuildScriptInstance(fileName);
        }

        private static void ReportUnspecifiedBuildScript()
        {
            StringBuilder errorMsg = new StringBuilder();
            errorMsg.Append(
                "The build script file was not specified. Please specify it as the first argument or use some of the default paths for script file: ");
            foreach (var defaultScriptLocation in defaultScriptLocations)
                errorMsg.AppendLine(defaultScriptLocation);

            throw new BuildScriptLocatorException(errorMsg.ToString());
        }

        private Task<IBuildScript> FindAndCreateBuildScriptInstance(string fileName)
        {
            return scriptLoader.FindAndCreateBuildScriptInstance(fileName);
        }

        private string GetFileName(CommandArguments args)
        {
            if (!string.IsNullOrEmpty(args.Script))
            {
                return TakeExplicitBuildScriptName(args);
            }

            _log.LogInformation("Build script file name was not explicitly specified, searching the default locations:");

            foreach (string defaultScriptLocation in defaultScriptLocations)
            {
                if (fileExistsService.FileExists(defaultScriptLocation))
                {
                    _log.LogInformation("Found it, using the build script file '{0}'.", defaultScriptLocation);
                    return defaultScriptLocation;
                }
            }

            return null;
        }

        private string TakeExplicitBuildScriptName(CommandArguments args)
        {
            if (fileExistsService.FileExists(args.Script))
                return args.Script;

            throw new BuildScriptLocatorException($"The build script file specified ('{args.Script}') does not exist.");
        }
    }
}