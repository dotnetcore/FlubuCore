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
            bool mustRestoreNugetPackages = true;

            if (!string.IsNullOrEmpty(pathToBuildScript) && nugetPropsFileExists)
            {
                var buildScriptModifiedTime = File.GetLastWriteTime(pathToBuildScript);
                var nugetPropsModifiedTime = File.GetLastWriteTime(nugetPropsFile);
                if (nugetPropsModifiedTime > buildScriptModifiedTime)
                {
                    mustRestoreNugetPackages = false;
                }
            }

            if (mustRestoreNugetPackages)
            {
                if (nugetPropsFileExists)
                {
                    File.Delete(nugetPropsFile);
                }

                CreateNugetProjectFile(targetFrameworkShortName, packageReferences);
                RestoreNugetPackages(NugetPackageResolveConstants.GeneratedProjectFileName);
            }

            var assemblyReferences = ResolvePackagesFromLockFile(
                projectAssetsJsonFile, packageReferences, hostFramework);

            File.Delete(NugetPackageResolveConstants.GeneratedProjectFileName);

            return assemblyReferences;
        }

        public List<AssemblyInfo> ResolveNugetPackagesFromFlubuCsproj(ProjectFileAnalyzerResult analyzerResult)
        {
            var nugetReferences = analyzerResult.NugetReferences.Where(x =>
                            !x.Id.Equals("FlubuCore", StringComparison.OrdinalIgnoreCase) &&
                            !x.Id.Equals("dotnet-flubu", StringComparison.OrdinalIgnoreCase) &&
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
            bool nugetPropsFileExists = _file.Exists(nugetPropsLocation);
            bool mustRestoreNugetPackages = true;

            if (nugetPropsFileExists)
            {
#pragma warning disable SA1305 // Field names should not use Hungarian notation
                var csProjModifiedTime = File.GetLastWriteTime(csprojLocation);
#pragma warning restore SA1305 // Field names should not use Hungarian notation
                var nugetPropsModifiedTime = File.GetLastWriteTime(nugetPropsLocation);
                if (nugetPropsModifiedTime > csProjModifiedTime)
                {
                    mustRestoreNugetPackages = false;
                }
            }

            if (mustRestoreNugetPackages)
            {
                if (nugetPropsFileExists)
                {
                    File.Delete(nugetPropsLocation);
                }

                RestoreNugetPackages(csprojLocation);
            }

            var hostFramework = GetHostFramework();
            var projectAssetsJsonPath = Path.Combine(csprojDir, "obj", "project.assets.json");

            return ResolvePackagesFromLockFile(projectAssetsJsonPath, nugetReferences, hostFramework);
        }

        private static NuGetFramework GetHostFramework()
        {
            var attr = Assembly
                .GetEntryAssembly()
                .GetCustomAttribute<TargetFrameworkAttribute>();

            return NuGetFramework.Parse(attr.FrameworkName);
        }

        private static List<AssemblyInfo> ResolvePackagesFromLockFile(
            string projectAssetsJsonPath,
            IEnumerable<NugetPackageReference> packageReferences,
            NuGetFramework hostFramework)
        {
            var assemblyReferences = new List<AssemblyInfo>();
            var resolvedDependencies = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var lockFile = new LockFileFormat().Read(projectAssetsJsonPath);

            var target = lockFile.GetTarget(hostFramework, runtimeIdentifier: null);
            if (target == null)
            {
                target = lockFile.Targets.FirstOrDefault();
            }

            if (target == null)
            {
                throw new ScriptException(
                    $"No targets found in {projectAssetsJsonPath} for framework {hostFramework.GetShortFolderName()}.");
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

                if (targetLib.CompileTimeAssemblies != null && targetLib.CompileTimeAssemblies.Count != 0 &&
                    !targetLib.CompileTimeAssemblies.All(a => a.Path.EndsWith("_._")))
                {
                    bool packageFound = AddAssemblyReference(targetLib, lockFile, packageFolders, assemblyReferences);

                    if (!packageFound)
                    {
                        throw new ScriptException($"Nuget package {packageReference.Id} not found.");
                    }

                    ResolveDependencies(targetLib, targetLibraries, lockFile, packageFolders, assemblyReferences, resolvedDependencies);
                }
                else
                {
                    if (targetLib.Dependencies.Count != 0)
                    {
                        ResolveDependencies(targetLib, targetLibraries, lockFile, packageFolders, assemblyReferences, resolvedDependencies);
                    }
                    else
                    {
                        throw new ScriptException(
                            $"Nuget package '{packageReference.Id}' '{packageReference.Version}' not found for framework {hostFramework.GetShortFolderName()}.");
                    }
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

                if (dep?.CompileTimeAssemblies != null &&
                    dep.CompileTimeAssemblies.Count != 0 &&
                    !dep.CompileTimeAssemblies.All(a => a.Path.EndsWith("_._")))
                {
                    bool packageFound = AddAssemblyReference(dep, lockFile, packageFolders, assemblyReferences);

                    if (!packageFound)
                    {
                        throw new ScriptException($"Nuget package {dependency.Id} not found.");
                    }

                    ResolveDependencies(dep, targetLibraries, lockFile, packageFolders, assemblyReferences, resolvedDependencies);
                }
            }
        }

        private static bool AddAssemblyReference(
            LockFileTargetLibrary targetLib,
            LockFile lockFile,
            string[] packageFolders,
            List<AssemblyInfo> assemblyReferences)
        {
            var existingRef = assemblyReferences.FirstOrDefault(x =>
                x.Name.Equals(targetLib.Name, StringComparison.OrdinalIgnoreCase));

            if (existingRef != null || targetLib.Name == "System.Runtime")
            {
                return true;
            }

            var library = lockFile.Libraries.FirstOrDefault(l =>
                l.Name.Equals(targetLib.Name, StringComparison.OrdinalIgnoreCase));

            if (library == null)
            {
                return false;
            }

            foreach (var compileAsm in targetLib.CompileTimeAssemblies)
            {
                if (compileAsm.Path.EndsWith("_._"))
                {
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

                        assemblyReferences.Add(new AssemblyInfo
                        {
                            Name = targetLib.Name,
                            FullPath = assemblyPath,
                            Version = version,
                        });

                        return true;
                    }
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
