using System.Collections.Generic;
using System.IO;
using System.Xml.XPath;
using FlubuCore.IO.Wrappers;
using Microsoft.Build.Framework;

namespace FlubuCore.Scripting.Analysis
{
    public class ProjectFileAnalyzer : IProjectFileAnalyzer
    {
        private static readonly List<string> _defaultCsprojLocations = new List<string>()
        {
            "BuildScript.csproj",
            "BuildScripts.csproj",
            "Build.csproj",
            "BuildScript/Build.csproj",
            "BuildScript/BuildScript.csproj",
            "BuildScripts/Build.csproj",
            "BuildScripts/BuildScript.csproj",
            "BuildScripts/BuildScripts.csproj",
            "Build/BuildScript.csproj",
            "build/BuildScript.csproj",
            "Build/Build.csproj",
            "_Build/Build.csproj",
            "_build/Build.csproj",
            "_build/BuildScript.csproj",
            "_BuildScript/Build.csproj",
            "_BuildScript/BuildScript.csproj",
            "_BuildScripts/Build.csproj",
            "_BuildScripts/BuildScript.csproj",
            "_BuildScripts/BuildScripts.csproj"
        };

        private readonly IFileWrapper _file;

        private readonly IBuildScriptLocator _buildScriptLocator;

        public ProjectFileAnalyzer(IFileWrapper file, IBuildScriptLocator buildScriptLocator)
        {
            _file = file;
            _buildScriptLocator = buildScriptLocator;
        }

        public ProjectFileAnalyzerResult Analyze(string location = null, bool disableAnalysis = false)
        {
            if (disableAnalysis)
            {
                return new ProjectFileAnalyzerResult
                {
                    ProjectFileFound = false
                };
            }

            var result = LocateCsproj(location);

            if (!result.ProjectFileFound)
            {
                return result;
            }

            XPathDocument doc = new XPathDocument(result.ProjectFileLocation);
            XPathNavigator nav = doc.CreateNavigator();
            var packageReferences = nav.Select("//PackageReference");
            while (packageReferences.MoveNext())
            {
                NugetPackageReference reference = new NugetPackageReference();
                reference.Id = packageReferences.Current.GetAttribute("Include", string.Empty);
                reference.Version = packageReferences.Current.GetAttribute("Version", string.Empty);
                result.NugetReferences.Add(reference);
            }

            var assemblyReferences = nav.Select("//Reference");
            while (assemblyReferences.MoveNext())
            {
                AssemblyReference reference = new AssemblyReference();
                reference.Name = assemblyReferences.Current.GetAttribute("Include", string.Empty);
                var packageNode = assemblyReferences.Current.Clone();
                if (packageNode.MoveToChild("HintPath", string.Empty))
                {
                    reference.Path = Path.Combine(Path.GetDirectoryName(result.ProjectFileLocation), packageNode.InnerXml);
                }

                result.AssemblyReferences.Add(reference);
            }

            return result;
        }

        private ProjectFileAnalyzerResult LocateCsproj(string location)
        {
            ProjectFileAnalyzerResult result = new ProjectFileAnalyzerResult();
            if (location == null)
            {
                if (_file.Exists("./.flubu"))
                {
                    var lines = _file.ReadAllLines("./.flubu");
                    if (lines.Count > 1 && !string.IsNullOrEmpty(lines[1]) && _file.Exists(lines[1]))
                    {
                        result.ProjectFileFound = true;
                        result.ProjectFileLocation = Path.GetFullPath(lines[1]);
                        return result;
                    }
                }

                foreach (var defaultCsprojLocation in _defaultCsprojLocations)
                {
                    if (_file.Exists(defaultCsprojLocation))
                    {
                        result.ProjectFileFound = true;
                        result.ProjectFileLocation = Path.GetFullPath(defaultCsprojLocation);
                        return result;
                    }

                    var defaultCsprojLocationSrc = Path.Combine("src", defaultCsprojLocation);
                    if (_file.Exists(defaultCsprojLocationSrc))
                    {
                        result.ProjectFileFound = true;
                        result.ProjectFileLocation = Path.GetFullPath(defaultCsprojLocationSrc);
                        return result;
                    }
                }

                var flubuFile = _buildScriptLocator.FindFlubuFile();

                if (string.IsNullOrEmpty(flubuFile))
                {
                    return result;
                }

                var flubuFileLines = _file.ReadAllLines(flubuFile);
                if (flubuFileLines.Count > 1 && !string.IsNullOrEmpty(flubuFileLines[1]))
                {
                    var flubuFileDir = Path.GetDirectoryName(flubuFile);
                    var buildCsprojPath = Path.Combine(flubuFileDir, flubuFileLines[1]);
                    result.ProjectFileFound = true;
                    result.ProjectFileLocation = buildCsprojPath;
                }
                else
                {
                    //// empty flubu file or csproj location is not set in flubu file. Search in default locations relative to .flubu file.
                    foreach (var defaultCsprojLocation in _defaultCsprojLocations)
                    {
                        var flubuFileDirectory = Path.GetDirectoryName(flubuFile);
                        var csprojLocation = Path.Combine(flubuFileDirectory, defaultCsprojLocation);
                        if (_file.Exists(csprojLocation))
                        {
                            result.ProjectFileFound = true;
                            result.ProjectFileLocation = Path.GetFullPath(csprojLocation);
                            return result;
                        }
                    }
                }
            }
            else
            {
                if (_file.Exists(location))
                {
                    result.ProjectFileFound = true;
                    result.ProjectFileLocation = location;
                }
            }

            return result;
        }
    }
}
