using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using FlubuCore.IO.Wrappers;
using Microsoft.Extensions.Logging;

namespace FlubuCore.Scripting
{
    public class BuildScriptLocator : IBuildScriptLocator
    {
        public static readonly List<string> DefaultScriptLocations = new List<string>
        {
            "buildscript.cs",
            "BuildScript.cs",
            "deployscript.cs",
            "DeployScript.cs",
            "DeploymentScript.cs",
            "Build.cs",
            "_Build/Build.cs",
            "build/Build.cs",
            "Build/Build.cs",
            "build/BuildScript.cs",
            "Build/BuildScript.cs",
            "_Build/BuildScript.cs",
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

        private string _flubuFile;

        private bool _searchedForFlubuFile = false;

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

        public virtual string FindFlubuFile()
        {
            if (_searchedForFlubuFile)
            {
                return _flubuFile;
            }

            _searchedForFlubuFile = true;

            var currentDirectory = Directory.GetCurrentDirectory();
            var directories = currentDirectory.Split(Path.DirectorySeparatorChar);
            string flubuFileDir = currentDirectory.StartsWith("/") ? "/" : string.Empty;
            foreach (var dir in directories)
            {
                string directory = dir;
                if (dir.EndsWith(":"))
                {
                    directory = $"{dir}{Path.DirectorySeparatorChar}";
                }

                flubuFileDir = _path.Combine(flubuFileDir, directory);
                var flubuFilePath = Path.Combine(flubuFileDir, ".flubu");
                if (File.Exists(flubuFilePath))
                {
                    _flubuFile = flubuFilePath;
                    return flubuFilePath;
                }
            }

            _flubuFile = null;
            return _flubuFile;
        }

        private string GetFileName(CommandArguments args)
        {
            if (!string.IsNullOrEmpty(args.Script))
            {
                return TakeExplicitBuildScriptName(args);
            }

            if (_file.Exists("./.flubu"))
            {
                var lines = _file.ReadAllLines("./.flubu");
                if (lines.Count != 0 && !string.IsNullOrEmpty(lines[0]) && _file.Exists(lines[0]))
                {
                    _log.LogInformation("using the build script file path from .flubu file. '{0}'.", lines[0]);
                    return lines[0];
                }
            }

            _log.LogInformation("Script file path was not explicitly specified, searching the default locations.");
            foreach (var defaultScriptLocation in DefaultScriptLocations)
            {
                if (_file.Exists(defaultScriptLocation))
                {
                    _log.LogInformation("Found it, using the build script file '{0}'.", defaultScriptLocation);
                    return defaultScriptLocation;
                }
            }

            var flubuFile = FindFlubuFile();

            if (flubuFile != null)
            {
                var lines = _file.ReadAllLines(flubuFile);
                var flubuFileDir = Path.GetDirectoryName(flubuFile);

                if (lines.Count == 0 || string.IsNullOrWhiteSpace(lines[0]))
                {
                    foreach (var defaultScriptLocation in DefaultScriptLocations)
                    {
                        var defaultScriptLocationFromFlubuFileLocation = Path.Combine(flubuFileDir, defaultScriptLocation);
                        if (_file.Exists(defaultScriptLocationFromFlubuFileLocation))
                        {
                            _log.LogInformation("Found it, using the build script file '{0}'.", defaultScriptLocationFromFlubuFileLocation);
                            return defaultScriptLocationFromFlubuFileLocation;
                        }
                    }

                    return null;
                }

                var buildScriptFullPath = Path.Combine(flubuFileDir, lines[0]);
                if (_file.Exists(buildScriptFullPath))
                {
                    _log.LogInformation("using the build script file from .flubu file. '{0}'.", lines[0]);
                    return buildScriptFullPath;
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
                    throw new BuildScriptLocatorException($"The file specified ('{args.Script}') is not a csharp source code (.cs) file. See getting started on https://flubucore.dotnetcore.xyz/getting-started/");
                }

                return args.Script;
            }

            throw new BuildScriptLocatorException($"The build script file specified ('{args.Script}') does not exist.");
        }

        private void ReportUnspecifiedBuildScript()
        {
            var errorMsg =
                new StringBuilder($"The build script file was not found.  Please specify location of the build script in the argument '-s={{PathToBuildScript}}' or run 'flubu setup' to store the location in a file or use some of the default paths for script file:{Environment.NewLine}");
            foreach (var defaultScriptLocation in DefaultScriptLocations)
                errorMsg.AppendLine(defaultScriptLocation);

            throw new BuildScriptLocatorException(errorMsg.ToString());
        }
    }
}
