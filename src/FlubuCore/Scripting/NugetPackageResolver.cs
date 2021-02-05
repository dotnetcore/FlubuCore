using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Runtime.Versioning;
using System.Text;
using System.Xml.Linq;
using FlubuCore.IO.Wrappers;
using FlubuCore.Scripting.Analysis;
using FlubuCore.Services;
using FlubuCore.Tasks.NetCore;
using Microsoft.DotNet.Cli.Utils;
using Microsoft.Extensions.DependencyModel;

namespace FlubuCore.Scripting
{
    public class NugetPackageResolver : INugetPackageResolver
    {
        private readonly ICommandFactory _commandFactory;
        private readonly IFileWrapper _file;

        private readonly IFlubuEnvironmentService _flubuEnvironmentService;

        private bool _packagesRestored;

        private List<CompilationLibrary> _resolvedDependencies = new List<CompilationLibrary>();

        public NugetPackageResolver(ICommandFactory commandFactory, IFileWrapper file, IFlubuEnvironmentService flubuEnvironmentService)
        {
            _commandFactory = commandFactory;
            _file = file;
            _flubuEnvironmentService = flubuEnvironmentService;
        }

        public List<AssemblyInfo> ResolveNugetPackagesFromDirectives(List<NugetPackageReference> packageReferences, string pathToBuildScript)
        {
            List<AssemblyInfo> assemblyReferences = new List<AssemblyInfo>();

            if (packageReferences == null || packageReferences.Count == 0)
            {
                return assemblyReferences;
            }

            ResolveNugetPackagesFromDirectives(packageReferences, pathToBuildScript, assemblyReferences);

            return assemblyReferences;
        }

        public List<AssemblyInfo> ResolveNugetPackagesFromFlubuCsproj(ProjectFileAnalyzerResult analyzerResult)
        {
            List<AssemblyInfo> assemblyReferences = new List<AssemblyInfo>();
            var nugetReferences = analyzerResult.NugetReferences.Where(x =>
                            !x.Id.Equals("FlubuCore", StringComparison.OrdinalIgnoreCase) &&
                            !x.Id.Equals("dotnet-flubu", StringComparison.OrdinalIgnoreCase) &&
                            !x.Id.Equals("FlubuCore.Runner", StringComparison.OrdinalIgnoreCase) &&
                            !x.Id.StartsWith("StyleCop", StringComparison.OrdinalIgnoreCase)).ToList();

            if (nugetReferences.Count == 0)
            {
                return assemblyReferences;
            }

            string csprojLocation = analyzerResult.ProjectFileLocation;

            var csprojDir = Path.GetDirectoryName(csprojLocation);
            var csprojfileName = Path.GetFileName(csprojLocation);
            var nugetPropsLocation = Path.Combine(csprojDir, "obj", csprojfileName + ".nuget.g.props");
            bool nugetPropsFileExists = _file.Exists(nugetPropsLocation);
            bool mustRestoreNugetPackages = true;

            if (nugetPropsFileExists)
            {
#pragma warning disable SA1305 // Field names should not use Hungarian notation
                var csProjModifiedTime = File.GetLastWriteTime(csprojLocation);
#pragma warning restore SA1305 // Field names should not use Hungarian notation
                var nugetProsModifiedTime = File.GetLastWriteTime(nugetPropsLocation);
                if (nugetProsModifiedTime > csProjModifiedTime)
                {
                    mustRestoreNugetPackages = false;
                }
            }

            var targetFramework = GetTargetFramework();

            if (mustRestoreNugetPackages)
            {
                if (nugetPropsFileExists)
                {
                    File.Delete(nugetPropsLocation);
                }

                RestoreNugetPackages(csprojLocation);
            }

            var document = XDocument.Load(nugetPropsLocation);
            var packageFolders = document.Descendants().Single(d => d.Name.LocalName == "NuGetPackageFolders").Value
                .Split(';');

            var dependencyContext = ReadDependencyContext(Path.Combine(csprojDir, "obj", "project.assets.json"));

            var compileLibraries = dependencyContext.CompileLibraries.ToList();
            foreach (var packageReference in nugetReferences)
            {
                CompilationLibrary compileLibrary = compileLibraries.FirstOrDefault(x =>
                    x.Name.Equals(packageReference.Id, StringComparison.OrdinalIgnoreCase));

                if (compileLibrary == null)
                {
                    throw new ScriptException(
                        $"Nuget package '{packageReference.Id}' '{packageReference.Version}' not found.");
                }

                if (compileLibrary.Assemblies.Count != 0)
                {
                    bool packageFound = AddAssemblyReference(packageFolders, compileLibrary, assemblyReferences);

                    if (!packageFound)
                    {
                        throw new ScriptException($"Nuget package {packageReference.Id} not found.");
                    }

                    ResolveDependencies(compileLibrary, compileLibraries, packageFolders, assemblyReferences);
                }
                else
                {
                    if (compileLibrary.Dependencies.Count != 0)
                    {
                        ResolveDependencies(compileLibrary, compileLibraries, packageFolders, assemblyReferences);
                    }
                    else
                    {
                        throw new ScriptException($"Nuget package '{packageReference.Id}' '{packageReference.Version}' not found for framework {targetFramework} ");
                    }
                }
            }

            return assemblyReferences;
        }

        private List<AssemblyInfo> ResolveNugetPackagesFromDirectives(List<NugetPackageReference> packageReferences, string pathToBuildScript, List<AssemblyInfo> assemblyReferences)
        {
            var scriptDir = Path.GetDirectoryName(pathToBuildScript);
            const string nugetPropsFile = "./obj/FlubuGen.csproj.nuget.g.props";
            bool nugetPropsFileExists = _file.Exists(nugetPropsFile);
            bool mustRestoreNugetPackages = true;

            if (!string.IsNullOrEmpty(pathToBuildScript) && nugetPropsFileExists)
            {
                var buildScriptModifiedTime = File.GetLastWriteTime(pathToBuildScript);
                var nugetProsModifiedTime = File.GetLastWriteTime(nugetPropsFile);
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
                    File.Delete(nugetPropsFile);
                }

                CreateNugetProjectFile(targetFramework, packageReferences);
                RestoreNugetPackages(NugetPackageResolveConstants.GeneratedProjectFileName);
            }

            var document = XDocument.Load(nugetPropsFile);
            var packageFolders = document.Descendants().Single(d => d.Name.LocalName == "NuGetPackageFolders").Value.Split(';');
            const string projectAssetsJsonFile = "./obj/project.assets.json";
            var dependencyContext = ReadDependencyContext(projectAssetsJsonFile);

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

                if (compileLibrary.Assemblies.Count != 0)
                {
                    bool packageFound = AddAssemblyReference(packageFolders, compileLibrary, assemblyReferences);

                    if (!packageFound)
                    {
                        throw new ScriptException($"Nuget package {packageReference.Id} not found.");
                    }

                    ResolveDependencies(compileLibrary, compileLibraries, packageFolders, assemblyReferences);
                }
                else
                {
                    if (compileLibrary.Dependencies.Count != 0)
                    {
                        ResolveDependencies(compileLibrary, compileLibraries, packageFolders, assemblyReferences);
                    }
                    else
                    {
                        throw new ScriptException(
                            $"Nuget package '{packageReference.Id}' '{packageReference.Version}' not found for framework {targetFramework} ");
                    }
                }
            }

            File.Delete(NugetPackageResolveConstants.GeneratedProjectFileName);

            return assemblyReferences;
        }

        public void ResolveDependencies(CompilationLibrary library, List<CompilationLibrary> compileLibraries, string[] packageFolders, List<AssemblyInfo> assemblyReferences)
        {
            if (library.Name == "NETStandard.Library" || library.Name == "FlubuCore" || _resolvedDependencies.Any(x => x.Name == library.Name))
            {
                return;
            }

            _resolvedDependencies.Add(library);

            foreach (var dependency in library.Dependencies)
            {
                var dep = compileLibraries.FirstOrDefault(x => x.Name.Equals(dependency.Name));
                if (dep?.Assemblies != null && dep.Assemblies.Count != 0 && !dep.Assemblies[0].EndsWith("_._"))
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

        private static DependencyContext ReadDependencyContext(string assetsFileLocation)
        {
            using (FileStream fs = new FileStream(assetsFileLocation, FileMode.Open, FileAccess.Read))
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

        private void RestoreNugetPackages(string csprojLocation)
        {
            try
            {
                var dotnetExecutable = ExecuteDotnetTask.FindDotnetExecutable();
                ICommand command = _commandFactory.Create(dotnetExecutable, new List<string>() { "restore", csprojLocation });
                command.CaptureStdErr().WorkingDirectory(Directory.GetCurrentDirectory()).Execute();
                _packagesRestored = true;
            }
            catch (InvalidOperationException e)
            {
                RestoreNugetPackagesMsBuildFallback(csprojLocation);
                if (!_packagesRestored)
                {
                    throw new ScriptException("Can not restore nuget packages. dotnet core sdk/runtime not installed and msbuild 15 or higher not found. See for more information",
                        e);
                }
            }
        }

        private void RestoreNugetPackagesMsBuildFallback(string csprojLocation)
        {
            var msbuilds = _flubuEnvironmentService.ListAvailableMSBuildToolsVersions();
            KeyValuePair<Version, string> msbuild = msbuilds.FirstOrDefault(x => x.Key >= new Version(15, 0, 0));
            if (msbuild.Equals(default(KeyValuePair<Version, string>)))
            {
                return;
            }

            var msBuildPath = Path.Combine(msbuilds.Last().Value, "msbuild.exe");
            ICommand command = _commandFactory.Create(msBuildPath, new List<string>() { "/t:restore", csprojLocation });
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
