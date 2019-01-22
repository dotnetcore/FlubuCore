using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
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

        private List<CompilationLibrary> _resolvedDependencies = new List<CompilationLibrary>();

        public NugetPackageResolver(ICommandFactory commandFactory)
        {
            _commandFactory = commandFactory;
        }

        public List<AssemblyInfo> ResolveNugetPackages(List<NugetPackageReference> packageReferences, string pathToBuildScript)
        {
            if (packageReferences == null || packageReferences.Count == 0)
            {
                return new List<AssemblyInfo>();
            }

            bool nugetPropsFileExists = File.Exists("./obj/FlubuGen.csproj.nuget.g.props");
            bool mustRestoreNugetPackages = true;

            if (!string.IsNullOrEmpty(pathToBuildScript) && nugetPropsFileExists)
            {
                var buildScriptModifiedTime = File.GetLastWriteTime(pathToBuildScript);
                var nugetProsModifiedTime = File.GetLastWriteTime("./obj/FlubuGen.csproj.nuget.g.props");
                if (nugetProsModifiedTime > buildScriptModifiedTime)
                {
                    mustRestoreNugetPackages = false;
                }
            }

            var targetFramework = GetTargetFramework();

            if (mustRestoreNugetPackages)
            {
                if (nugetPropsFileExists)
                {
                    File.Delete("./obj/FlubuGen.csproj.nuget.g.props");
                }

                CreateNugetProjectFile(targetFramework, packageReferences);
                RestoreNugetPackages();
            }

            var document = XDocument.Load("./obj/FlubuGen.csproj.nuget.g.props");
            var packageFolders = document.Descendants().Single(d => d.Name.LocalName == "NuGetPackageFolders").Value.Split(';');

            var dependencyContext = ReadDependencyContext();

            List<AssemblyInfo> assemblyReferences = new List<AssemblyInfo>();
            var compileLibraries = dependencyContext.CompileLibraries.ToList();
            foreach (var packageReference in packageReferences)
            {
                CompilationLibrary compileLibrary = compileLibraries.FirstOrDefault(x =>
                    x.Name.Equals(packageReference.Id, StringComparison.OrdinalIgnoreCase));

                if (compileLibrary == null)
                {
                    throw new ScriptException(
                        $"Nuget package '{packageReference.Id}' '{packageReference.Version}' not found.");
                }

                if (compileLibrary.Assemblies.Count == 0)
                {
                    throw new ScriptException(
                        $"Nuget package '{packageReference.Id}' '{packageReference.Version}' not found for framework {targetFramework} ");
                }

                bool packageFound = AddAssemblyReference(packageFolders, compileLibrary, assemblyReferences);

                if (!packageFound)
                {
                    throw new ScriptException($"Nuget package {packageReference.Id} not found.");
                }

                ResolveDependencies(compileLibrary, compileLibraries, packageFolders, assemblyReferences);
            }

            File.Delete(NugetPackageResolveConstants.GeneratedProjectFileName);

            return assemblyReferences;
        }

        public void ResolveDependencies(CompilationLibrary library, List<CompilationLibrary> compileLibraries, string[] packageFolders, List<AssemblyInfo> assemblyReferences)
        {
            if (library.Name == "NETStandard.Library" || _resolvedDependencies.Any(x => x.Name == library.Name))
            {
                return;
            }

            _resolvedDependencies.Add(library);

            foreach (var dependency in library.Dependencies)
            {
                var dep = compileLibraries.FirstOrDefault(x => x.Name.Equals(dependency.Name));
                if (dep.Assemblies != null && dep.Assemblies.Count() != 0 && !dep.Assemblies[0].EndsWith("_._"))
                {
                    bool packageFound = AddAssemblyReference(packageFolders, dep, assemblyReferences);

                    if (!packageFound)
                    {
                        throw new ScriptException($"Nuget package {dependency.Name} not found.");
                    }

                    ResolveDependencies(dep, compileLibraries, packageFolders, assemblyReferences);
                }
            }
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

        private static bool AddAssemblyReference(string[] packageFolders, CompilationLibrary compileLibrary,
            List<AssemblyInfo> assemblyReferences)
        {
            var assemblyRefrence = assemblyReferences.FirstOrDefault(x => x.Name == compileLibrary.Name);
            if (assemblyRefrence != null || compileLibrary.Name == "System.Runtime")
            {
                return true;
            }

            foreach (var packageFolder in packageFolders)
            {
                var packagePath = Path.Combine(packageFolder, compileLibrary.Path, compileLibrary.Assemblies[0]);

                if (File.Exists(packagePath))
                {
                    var versions = compileLibrary.Version.Split('.');
                    int.TryParse(versions[0], out var major);
                    int.TryParse(versions[1], out var minor);
                    int.TryParse(versions[2], out var build);
                    int revision = 0;
                    if (versions.Length > 3)
                    {
                        int.TryParse(versions[3], out revision);
                    }

                    assemblyReferences.Add(new AssemblyInfo
                    {
                        Name = compileLibrary.Name,
                        FullPath = packagePath,
                        Version = new Version(major, minor, build, revision)
                    });
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
