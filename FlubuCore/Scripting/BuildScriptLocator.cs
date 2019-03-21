using System.Text;
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
            "DeploymentScript.cs",
            "Build.cs",
            "_Build/Build.cs",
            "Build/Build.cs",
            "_BuildScript/BuildScript.cs",
            "_BuildScripts/BuildScript.cs",
            "BuildScript/BuildScript.cs",
            "BuildScripts/BuildScript.cs",
            "BuildScript/DeployScript.cs",
            "BuildScripts/DeployScript.cs",
            "BuildScript/DeploymentScript.cs",
            "BuildScripts/DeploymentScript.cs",
        };

        private readonly ILogger<BuildScriptLocator> _log;

        private readonly IFileWrapper _file;
        private readonly IPathWrapper _path;

        public BuildScriptLocator(
            IFileWrapper file,
            IPathWrapper path,
            ILogger<BuildScriptLocator> log)
        {
            _file = file;
            _path = path;
            _log = log;
        }

        public string FindBuildScript(CommandArguments args)
        {
            string fileName = GetFileName(args);

            if (fileName == null)
            {
                ReportUnspecifiedBuildScript();
            }

            return fileName;
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
                var extension = _path.GetExtension(args.Script);
                if (string.IsNullOrEmpty(extension) || !extension.Equals(".cs"))
                {
                    throw new BuildScriptLocatorException($"The file specified ('{args.Script}') is not a csharp source code (.cs) file. See getting started on https://github.com/flubu-core/flubu.core/wiki");
                }

                return args.Script;
            }

            throw new BuildScriptLocatorException($"The build script file specified ('{args.Script}') does not exist.");
        }

        private void ReportUnspecifiedBuildScript()
        {
            var errorMsg =
                new StringBuilder(
                    "The build script file was not specified. Please specify it as the argument(-s={PathToBuildScript}) or use some of the default paths for script file: ");
            foreach (var defaultScriptLocation in BuildScriptLocator.DefaultScriptLocations)
                errorMsg.AppendLine(defaultScriptLocation);

            throw new BuildScriptLocatorException(errorMsg.ToString());
        }
    }
}