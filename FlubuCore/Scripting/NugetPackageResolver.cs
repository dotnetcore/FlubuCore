using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Versioning;
using System.Text;
using System.Xml.Linq;
using FlubuCore.Tasks.NetCore;
using Microsoft.DotNet.Cli.Utils;
using Microsoft.Extensions.DependencyModel;

namespace FlubuCore.Scripting
{
    public class NugetPackageResolver : INugetPackageResolver
    {
        private readonly ICommandFactory _commandFactory;

        private bool _packagesRestored;

        public NugetPackageResolver(ICommandFactory commandFactory)
        {
            _commandFactory = commandFactory;
        }

        public List<string> ResolveNugetPackages(List<NugetPackageReference> packageReferences)
        {
            if (packageReferences == null || packageReferences.Count == 0)
            {
                return new List<string>();
            }

            var targetFramework = GetTargetFramework();
            CreateNugetProjectFile(targetFramework, packageReferences);
            RestoreNugetPackages();

            var document = XDocument.Load("./obj/FlubuGen.csproj.nuget.g.props");
            var packageFolders = document.Descendants().Single(d => d.Name.LocalName == "NuGetPackageFolders").Value.Split(';');

            var dependencyContext = ReadDependencyContext();

            List<string> pathToReferences = new List<string>();
            var compileLibraries = dependencyContext.CompileLibraries.ToList();
            foreach (var packageReference in packageReferences)
            {
                var compileLibrary = compileLibraries.FirstOrDefault(x => x.Name.Equals(packageReference.Id, StringComparison.OrdinalIgnoreCase));

                if (compileLibrary == null)
                {
                    throw new ScriptException($"Nuget package '{packageReference.Id}' not found in project.assets.json. Please report a bug to FlubuCore team.");
                }

                if (compileLibrary.Assemblies.Count == 0)
                {
                    throw new ScriptException($"Nuget package '{packageReference.Id}' Version '{packageReference.Version}' not found for framework {targetFramework} ");
                }

                bool packageFound = GetPackageFullPath(packageFolders, compileLibrary, pathToReferences);

                if (!packageFound)
                {
                    throw new ScriptException($"Nuget package {packageReference.Id} not found.");
                }
            }

            File.Delete(NugetPackageResolveConstants.GeneratedProjectFileName);

            return pathToReferences;
        }

        private static string GetTargetFramework()
        {
            TargetFrameworkAttribute at;

            at = Assembly
                .GetEntryAssembly()
                .GetCustomAttribute<TargetFrameworkAttribute>();

            var tf = at.FrameworkName.Split(',');
            string targetFramework;
            if (tf[0].EndsWith("framework", StringComparison.OrdinalIgnoreCase))
            {
                targetFramework = $"net{tf[1].Substring(9).Replace(".", string.Empty)}";
            }
            else
            {
                targetFramework = $"{tf[0].Substring(1)}{tf[1].Substring(9)}";
            }

            return targetFramework;
        }

        private static DependencyContext ReadDependencyContext()
        {
            var assetsFile = "./obj/project.assets.json";
            using (FileStream fs = new FileStream(assetsFile, FileMode.Open, FileAccess.Read))
            {
                using (var contextReader = new DependencyContextJsonReader())
                {
                    return contextReader.Read(fs);
                }
            }
        }

        private static bool GetPackageFullPath(string[] packageFolders, CompilationLibrary compileLibrary, List<string> pathToReferences)
        {
            bool packageFound = false;
            foreach (var packageFolder in packageFolders)
            {
                var packagePath = Path.Combine(packageFolder, compileLibrary.Path, compileLibrary.Assemblies[0]);

                if (File.Exists(packagePath))
                {
                    pathToReferences.Add(packagePath);
                    return true;
                }
            }

            return false;
        }

        private void CreateNugetProjectFile(string targetFramework, List<NugetPackageReference> scriptPackageReferences)
        {
            XDocument csprojDocument = XDocument.Parse(NugetPackageResolveConstants.ProjectFileTemplate);
            var targetFrameworkElement = csprojDocument.Descendants("TargetFramework").Single();
            targetFrameworkElement.Value = targetFramework;
            var itemGroupElement = csprojDocument.Descendants("ItemGroup").Single();
            foreach (var scriptPackageReference in scriptPackageReferences)
            {
                var packageReferenceElement = new XElement("PackageReference");
                packageReferenceElement.Add(new XAttribute("Include", scriptPackageReference.Id));
                packageReferenceElement.Add(new XAttribute("Version", scriptPackageReference.Version));
                itemGroupElement.Add(packageReferenceElement);
            }

            using (var fileStream = new FileStream(NugetPackageResolveConstants.GeneratedProjectFileName, FileMode.Create, FileAccess.Write))
            {
                csprojDocument.Save(fileStream);
            }
        }

        private void RestoreNugetPackages()
        {
            ICommand command = _commandFactory.Create("dotnet", new List<string>() { "restore", NugetPackageResolveConstants.GeneratedProjectFileName });
            command.CaptureStdErr().WorkingDirectory(Directory.GetCurrentDirectory()).Execute();
            _packagesRestored = true;
        }

        private static class NugetPackageResolveConstants
        {
            public const string GeneratedProjectFileName = "FlubuGen.csproj";

            public const string ProjectFileTemplate = "<Project Sdk = \"Microsoft.NET.Sdk\">\r\n  <PropertyGroup>\r\n  <TargetFramework></TargetFramework>\r\n  </PropertyGroup>\r\n  <ItemGroup>    \r\n  </ItemGroup>\r\n</Project>";
        }
    }
}
