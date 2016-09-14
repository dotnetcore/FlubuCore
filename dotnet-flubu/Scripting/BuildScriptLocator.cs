using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using flubu.Scripting;
using Microsoft.Extensions.Logging;
using flubu.core.Infrastructure;

namespace flubu.Console
{
    public class BuildScriptLocator
    {
        public BuildScriptLocator(
            IFileExistsService fileExistsService, 
            ILogger<BuildScriptLocator> log,
            IScriptLoader scriptLoader)
        {
            this.fileExistsService = fileExistsService;
            this.scriptLoader = scriptLoader;
            _log = log;
        }

        public IBuildScript FindBuildScript(IList<string> args)
        {
            string fileName = GetFileName(args);

            if (fileName == null)
                ReportUnspecifiedBuildScript();

            return FindAndCreateBuildScriptInstance(fileName);
        }

        private string GetFileName(IList<string> args)
        {
            if (args.Count > 0)
            {
                if (args[0].EndsWith(".cs", StringComparison.OrdinalIgnoreCase))
                    return TakeExplicitBuildScriptName(args);
            }

            _log.Log("Build script file name was not explicitly specified, searching the default locations:");

            foreach (string defaultScriptLocation in defaultScriptLocations)
            {
                _log.Log("Looking for a build script file '{0}'.", defaultScriptLocation);

                if (fileExistsService.FileExists(defaultScriptLocation))
                {
                    _log.Log("Found it, using the build script file '{0}'.", defaultScriptLocation);
                    return defaultScriptLocation;
                }
            }

            return null;
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

        private IBuildScript FindAndCreateBuildScriptInstance(string fileName)
        {
            return scriptLoader.FindAndCreateBuildScriptInstance(fileName);
        }

        private string TakeExplicitBuildScriptName(IList<string> args)
        {
            string buildScriptName = args[0];

            if (!fileExistsService.FileExists(buildScriptName))
            {
                string message = string.Format(
                    CultureInfo.InvariantCulture,
                    "The build script file specified ('{0}') does not exist.",
                    buildScriptName);
                throw new BuildScriptLocatorException(message);
            }

            args.RemoveAt(0);
            
            return buildScriptName;
        }

        private static readonly string[] defaultScriptLocations =
        {
            "buildscript.cs",
            "deployscript.cs",
            "buildscript\\buildscript.cs",
            "buildscripts\\buildscript.cs"
        };

        private readonly IFileExistsService fileExistsService;
        private readonly IScriptLoader scriptLoader;
        private readonly ILogger<BuildScriptLocator> _log;
    }
}