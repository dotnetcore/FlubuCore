using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Versioning;
using System.Xml.Linq;
using FlubuCore.IO.Wrappers;
using FlubuCore.Scripting.Analysis;
using FlubuCore.Services;
using FlubuCore.Tasks.NetCore;
using Microsoft.DotNet.Cli.Utils;
using NuGet.Frameworks;
using NuGet.ProjectModel;

namespace FlubuCore.Scripting
{
    public class NugetPackageResolver : INugetPackageResolver
    {
        private readonly ICommandFactory _commandFactory;
        private readonly IFileWrapper _file;

        private readonly IFlubuEnvironmentService _flubuEnvironmentService;

        private bool _packagesRestored;

        public NugetPackageResolver(ICommandFactory commandFactory, IFileWrapper file, IFlubuEnvironmentService flubuEnvironmentService)
        {
            _commandFactory = commandFactory;
            _file = file;
            _flubuEnvironmentService = flubuEnvironmentService;
        }

        public List<AssemblyInfo> ResolveNugetPackagesFromDirectives(List<NugetPackageReference> packageReferences, string pathToBuildScript)
        {
            if (packageReferences == null || packageReferences.Count == 0)
            {
                return new List<AssemblyInfo>();
            }

            var hostFramework = GetHostFramework();
            var targetFrameworkShortName = hostFramework.GetShortFolderName();
            const string nugetPropsFile = "./obj/FlubuGen.csproj.nuget.g.props";
            const string projectAssetsJsonFile = "./obj/project.assets.json";
            bool nugetPropsFileExists = _file.Exists(nugetPropsFile);
            bool mustRestore = true;

            if (!string.IsNullOrEmpty(pathToBuildScript) && nugetPropsFileExists && _file.Exists(projectAssetsJsonFile))
            {
                var buildScriptModifiedTime = File.GetLastWriteTime(pathToBuildScript);
                var nugetPropsModifiedTime = File.GetLastWriteTime(nugetPropsFile);
                if (nugetPropsModifiedTime > buildScriptModifiedTime)
                {
                    mustRestore = false;
                }
            }

            if (mustRestore)
            {
                if (nugetPropsFileExists)
                {
                    File.Delete(nugetPropsFile);
                }

                CreateNugetProjectFile(targetFrameworkShortName, packageReferences);
                RestoreNugetPackages(NugetPackageResolveConstants.GeneratedProjectFileName);
            }

            try
            {
                return ResolvePackagesFromLockFile(projectAssetsJsonFile, packageReferences, hostFramework);
            }
            finally
            {
                if (File.Exists(NugetPackageResolveConstants.GeneratedProjectFileName))
                {
                    File.Delete(NugetPackageResolveConstants.GeneratedProjectFileName);
                }
            }
        }

        public List<AssemblyInfo> ResolveNugetPackagesFromFlubuCsproj(ProjectFileAnalyzerResult analyzerResult)
        {
            var nugetReferences = analyzerResult.NugetReferences.Where(x =>
                            !x.Id.Equals("FlubuCore", StringComparison.OrdinalIgnoreCase) &&
                            !x.Id.Equals("FlubuCore.Tool", StringComparison.OrdinalIgnoreCase) &&
                            !x.Id.Equals("FlubuCore.Runner", StringComparison.OrdinalIgnoreCase) &&
                            !x.Id.StartsWith("StyleCop", StringComparison.OrdinalIgnoreCase)).ToList();

            if (nugetReferences.Count == 0)
            {
                return new List<AssemblyInfo>();
            }

            string csprojLocation = analyzerResult.ProjectFileLocation;
            var csprojDir = Path.GetDirectoryName(csprojLocation);
            var csprojFileName = Path.GetFileName(csprojLocation);
            var nugetPropsLocation = Path.Combine(csprojDir, "obj", csprojFileName + ".nuget.g.props");
            var projectAssetsJsonPath = Path.Combine(csprojDir, "obj", "project.assets.json");
            bool nugetPropsFileExists = _file.Exists(nugetPropsLocation);
            bool mustRestore = true;

            if (nugetPropsFileExists && _file.Exists(projectAssetsJsonPath))
            {
#pragma warning disable SA1305 // Field names should not use Hungarian notation
                var csProjModifiedTime = File.GetLastWriteTime(csprojLocation);
#pragma warning restore SA1305 // Field names should not use Hungarian notation
                var nugetPropsModifiedTime = File.GetLastWriteTime(nugetPropsLocation);
                if (nugetPropsModifiedTime > csProjModifiedTime)
                {
                    mustRestore = false;
                }
            }

            if (mustRestore)
            {
                if (nugetPropsFileExists)
                {
                    File.Delete(nugetPropsLocation);
                }

                RestoreNugetPackages(csprojLocation);
            }

            var hostFramework = GetHostFramework();
            return ResolvePackagesFromLockFile(projectAssetsJsonPath, nugetReferences, hostFramework);
        }

        private static NuGetFramework GetHostFramework()
        {
            var entryAssembly = Assembly.GetEntryAssembly();
            if (entryAssembly == null)
            {
                throw new ScriptException("Could not determine host framework: entry assembly is null.");
            }

            var attr = entryAssembly.GetCustomAttribute<TargetFrameworkAttribute>();
            if (attr == null)
            {
                throw new ScriptException("Could not determine host framework: TargetFrameworkAttribute not found on entry assembly.");
            }

            return NuGetFramework.Parse(attr.FrameworkName);
        }

        private static List<AssemblyInfo> ResolvePackagesFromLockFile(
            string projectAssetsJsonPath,
            IEnumerable<NugetPackageReference> packageReferences,
            NuGetFramework hostFramework)
        {
            if (!File.Exists(projectAssetsJsonPath))
            {
                throw new ScriptException($"NuGet assets file not found at '{projectAssetsJsonPath}'. Run 'dotnet restore' first.");
            }

            var assemblyReferences = new List<AssemblyInfo>();
            var resolvedDependencies = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var lockFile = new LockFileFormat().Read(projectAssetsJsonPath);

            var target = lockFile.GetTarget(hostFramework, runtimeIdentifier: null);
            if (target == null)
            {
                var availableTargets = string.Join(", ", lockFile.Targets.Select(t => t.TargetFramework.GetShortFolderName()));
                throw new ScriptException(
                    $"No target found in {projectAssetsJsonPath} for framework '{hostFramework.GetShortFolderName()}'. Available targets: {availableTargets}");
            }

            var packageFolders = lockFile.PackageFolders
                .Select(pf => pf.Path)
                .ToArray();

            var targetLibraries = target.Libraries.ToList();

            foreach (var packageReference in packageReferences)
            {
                var targetLib = targetLibraries.FirstOrDefault(x =>
                    x.Name.Equals(packageReference.Id, StringComparison.OrdinalIgnoreCase));

                if (targetLib == null)
                {
                    throw new ScriptException(
                        $"Nuget package '{packageReference.Id}' '{packageReference.Version}' not found.");
                }

                bool hasCompileAssemblies = HasCompileAssemblies(targetLib);

                if (hasCompileAssemblies)
                {
                    bool packageFound = AddAssemblyReferences(targetLib, lockFile, packageFolders, assemblyReferences);
                    if (!packageFound)
                    {
                        throw new ScriptException($"Nuget package {packageReference.Id} not found.");
                    }
                }

                if (targetLib.Dependencies.Count != 0)
                {
                    ResolveDependencies(targetLib, targetLibraries, lockFile, packageFolders, assemblyReferences, resolvedDependencies);
                }
                else if (!hasCompileAssemblies)
                {
                    throw new ScriptException(
                        $"Nuget package '{packageReference.Id}' '{packageReference.Version}' not found for framework {hostFramework.GetShortFolderName()}.");
                }
            }

            return assemblyReferences;
        }

        private static void ResolveDependencies(
            LockFileTargetLibrary library,
            List<LockFileTargetLibrary> targetLibraries,
            LockFile lockFile,
            string[] packageFolders,
            List<AssemblyInfo> assemblyReferences,
            HashSet<string> resolvedDependencies)
        {
            if (library.Name == "NETStandard.Library" ||
                library.Name == "FlubuCore" ||
                !resolvedDependencies.Add(library.Name))
            {
                return;
            }

            foreach (var dependency in library.Dependencies)
            {
                var dep = targetLibraries.FirstOrDefault(
                    x => x.Name.Equals(dependency.Id, StringComparison.OrdinalIgnoreCase));

                if (dep == null)
                {
                    continue;
                }

                if (HasCompileAssemblies(dep))
                {
                    bool packageFound = AddAssemblyReferences(dep, lockFile, packageFolders, assemblyReferences);
                    if (!packageFound)
                    {
                        throw new ScriptException($"Nuget package {dependency.Id} not found.");
                    }
                }

                if (dep.Dependencies.Count != 0)
                {
                    ResolveDependencies(dep, targetLibraries, lockFile, packageFolders, assemblyReferences, resolvedDependencies);
                }
            }
        }

        private static bool HasCompileAssemblies(LockFileTargetLibrary targetLib)
        {
            return targetLib.CompileTimeAssemblies != null &&
                   targetLib.CompileTimeAssemblies.Count != 0 &&
                   !targetLib.CompileTimeAssemblies.All(a => a.Path.EndsWith("_._"));
        }

        private static bool AddAssemblyReferences(
            LockFileTargetLibrary targetLib,
            LockFile lockFile,
            string[] packageFolders,
            List<AssemblyInfo> assemblyReferences)
        {
            var library = lockFile.Libraries.FirstOrDefault(l =>
                l.Name.Equals(targetLib.Name, StringComparison.OrdinalIgnoreCase));

            if (library == null)
            {
                return false;
            }

            bool anyFound = false;

            foreach (var compileAsm in targetLib.CompileTimeAssemblies)
            {
                if (compileAsm.Path.EndsWith("_._"))
                {
                    continue;
                }

                var assemblyName = Path.GetFileNameWithoutExtension(compileAsm.Path);

                if (assemblyName == "System.Runtime")
                {
                    anyFound = true;
                    continue;
                }

                var existingRef = assemblyReferences.FirstOrDefault(x =>
                    x.Name.Equals(assemblyName, StringComparison.OrdinalIgnoreCase));

                if (existingRef != null)
                {
                    anyFound = true;
                    continue;
                }

                foreach (var packageFolder in packageFolders)
                {
                    var assemblyPath = Path.Combine(packageFolder, library.Path, compileAsm.Path);

                    if (File.Exists(assemblyPath))
                    {
                        var version = targetLib.Version != null
                            ? new Version(
                                targetLib.Version.Major,
                                targetLib.Version.Minor,
                                targetLib.Version.Patch,
                                targetLib.Version.Revision > 0 ? targetLib.Version.Revision : 0)
                            : new Version(0, 0, 0);

                        bool isRefAssembly = compileAsm.Path.Contains("/ref/") || compileAsm.Path.Contains("\\ref\\");
                        string runtimePath = null;

                        if (isRefAssembly)
                        {
                            runtimePath = ResolveRuntimeAssembly(targetLib, library, assemblyName, packageFolders);
                        }

                        assemblyReferences.Add(new AssemblyInfo
                        {
                            Name = assemblyName,
                            FullPath = assemblyPath,
                            Version = version,
                            IsCompileOnly = isRefAssembly,
                            RuntimePath = runtimePath,
                        });

                        anyFound = true;
                        break;
                    }
                }
            }

            return anyFound;
        }

        private static string ResolveRuntimeAssembly(
            LockFileTargetLibrary targetLib,
            LockFileLibrary library,
            string assemblyName,
            string[] packageFolders)
        {
            if (targetLib.RuntimeAssemblies == null)
            {
                return null;
            }

            var runtimeAsm = targetLib.RuntimeAssemblies.FirstOrDefault(
                a => !a.Path.EndsWith("_._") &&
                     Path.GetFileNameWithoutExtension(a.Path)
                         .Equals(assemblyName, StringComparison.OrdinalIgnoreCase));

            if (runtimeAsm == null)
            {
                return null;
            }

            foreach (var packageFolder in packageFolders)
            {
                var runtimePath = Path.Combine(packageFolder, library.Path, runtimeAsm.Path);
                if (File.Exists(runtimePath))
                {
                    return runtimePath;
                }
            }

            return null;
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

            var msBuildPath = Path.Combine(msbuild.Value, "msbuild.exe");
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
