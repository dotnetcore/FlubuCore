using System.Collections.Generic;
using System.IO;
using System.Xml.XPath;
using FlubuCore.IO.Wrappers;

namespace FlubuCore.Scripting.Analysis
{
    public class ProjectFileAnalyzer : IProjectFileAnalyzer
    {
        private static readonly List<string> _defaultCsprojLocations = new List<string>()
        {
            "BuildScript.csproj",
            "BuildScripts.csproj",
            "Build.csproj",
            "BuildScript/BuildScript.csproj",
            "BuildScripts/BuildScripts.csproj",
            "Build/Build.csproj",
            "_Build/Build.csproj",
            "_BuildScript/BuildScript.csproj",
            "_BuildScripts/BuildScripts.csproj"
        };

        private readonly IFileWrapper _file;

        public ProjectFileAnalyzer(IFileWrapper file)
        {
            _file = file;
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

                foreach (var item in _defaultCsprojLocations)
                {
                    if (_file.Exists(item))
                    {
                        result.ProjectFileFound = true;
                        result.ProjectFileLocation = Path.GetFullPath(item);
                        break;
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
