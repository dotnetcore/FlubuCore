using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.InteropServices;
#if !NET462
using System.Runtime.Loader;
#endif
using System.Text;
using System.Threading.Tasks;
using FlubuCore.Infrastructure;
using FlubuCore.IO.Wrappers;
using FlubuCore.Scripting.Analysis;
using FlubuCore.Scripting.Attributes.Config;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.CodeAnalysis.Text;
using Microsoft.Extensions.Logging;
using TypeInfo = System.Reflection.TypeInfo;

namespace FlubuCore.Scripting
{
    public class ScriptLoader : IScriptLoader
    {
        public static readonly string[] DefaultScriptReferencesLocations =
        {
            "FlubuLib",
            "BuildScript/FlubuLib",
            "BuildScripts/FlubuLib",
            "build/FlubuLib",
            "Build/FlubuLib",
            "_Build/FlubuLib",
            "_BuildScript/FlubuLib",
            "_BuildScripts/FlubuLib"
        };

        private readonly IFileWrapper _file;
        private readonly IDirectoryWrapper _directory;
        private readonly IProjectFileAnalyzer _projectFileAnalyzer;
        private readonly IScriptAnalyzer _scriptAnalyzer;
        private readonly IBuildScriptLocator _buildScriptLocator;
        private readonly INugetPackageResolver _nugetPackageResolver;

        private readonly ILogger<ScriptLoader> _log;

        public ScriptLoader(
            IFileWrapper file,
            IDirectoryWrapper directory,
            IProjectFileAnalyzer projectFileAnalyzer,
            IScriptAnalyzer scriptAnalyzer,
            IBuildScriptLocator buildScriptLocator,
            INugetPackageResolver nugetPackageResolver,
            ILogger<ScriptLoader> log)
        {
            _file = file;
            _directory = directory;
            _projectFileAnalyzer = projectFileAnalyzer;
            _scriptAnalyzer = scriptAnalyzer;
            _log = log;
            _buildScriptLocator = buildScriptLocator;
            _nugetPackageResolver = nugetPackageResolver;
        }

        public string GetBuildScriptAssemblyFileName(string buildScriptFileName, bool fullPath)
        {
            var assemblyFileName = Path.Combine(Path.GetDirectoryName(buildScriptFileName), "bin", Path.GetFileName(buildScriptFileName));
            assemblyFileName = Path.ChangeExtension(assemblyFileName, "dll");
            if (fullPath)
            {
                return Path.GetFullPath(assemblyFileName);
            }

            return assemblyFileName;
        }

        public async Task<IBuildScript> FindAndCreateBuildScriptInstanceAsync(CommandArguments args)
        {
            string buildScriptFilePath = _buildScriptLocator.FindBuildScript(args);
            var buildScriptAssemblyPath = GetBuildScriptAssemblyFileName(buildScriptFilePath, fullPath: false);

            List<string> code = _file.ReadAllLines(buildScriptFilePath);
            ScriptAnalyzerResult scriptAnalyzerResult = _scriptAnalyzer.Analyze(code);
            ProjectFileAnalyzerResult projectFileAnalyzerResult = _projectFileAnalyzer.Analyze(
                disableAnalysis: scriptAnalyzerResult.ScriptAttributes.Contains(ScriptConfigAttributes
                    .DisableLoadScriptReferencesAutomatically));

            ProcessConfigAttributes(scriptAnalyzerResult);
            ProcessAddedCsFilesToBuildScript(scriptAnalyzerResult, code);
            ProcessPartialBuildScriptClasses(scriptAnalyzerResult, code, buildScriptFilePath);

            bool oldWay = scriptAnalyzerResult.ScriptAttributes.Contains(ScriptConfigAttributes
                .CreateBuildScriptInstanceOldWayAttribute);

#if NET462
            oldWay = true;
#endif
            var references = GetBuildScriptReferences(args, projectFileAnalyzerResult, scriptAnalyzerResult,
                oldWay, buildScriptFilePath);

            if (oldWay)
            {
                return await CreateBuildScriptInstanceOldWay(buildScriptFilePath, references, code,
                    scriptAnalyzerResult);
            }

            Assembly assembly;
            if (!scriptAnalyzerResult.ScriptAttributes.Contains(ScriptConfigAttributes.AlwaysRecompileScript))
            {
                assembly = TryLoadBuildScriptFromAssembly(buildScriptAssemblyPath, buildScriptFilePath,
                    scriptAnalyzerResult, projectFileAnalyzerResult);

                if (assembly != null)
                {
                    return CreateBuildScriptInstance(assembly, buildScriptFilePath);
                }
            }

            code.Insert(0, $"#line 1 \"{buildScriptFilePath}\"");
            assembly = CompileBuildScript(buildScriptAssemblyPath, buildScriptFilePath, references, string.Join("\r\n", code));
            return CreateBuildScriptInstance(assembly, buildScriptFilePath);
        }

        private static IBuildScript CreateBuildScriptInstance(Assembly assembly, string fileName)
        {
            TypeInfo type = assembly.DefinedTypes.FirstOrDefault(i => i.ImplementedInterfaces.Any(x => x == typeof(IBuildScript)));

            if (type == null)
            {
                throw new ScriptLoaderExcetpion($"Class in file: {fileName} must inherit from DefaultBuildScript or implement IBuildScipt interface. See getting started on https://flubucore.dotnetcore.xyz/getting-started/");
            }

            var obj = Activator.CreateInstance(type.AsType());

            var buildScript = obj as IBuildScript ?? throw new ScriptLoaderExcetpion($"Class in file: {fileName} must inherit from DefaultBuildScript or implement IBuildScipt interface. See getting started on https://flubucore.dotnetcore.xyz/getting-started/");
            return buildScript;
        }

        private Assembly TryLoadBuildScriptFromAssembly(string buildScriptAssemblyPath, string buildScriptFilePath, ScriptAnalyzerResult scriptAnalyzerResult, ProjectFileAnalyzerResult projectFileAnalyzerResult)
        {
            if (!File.Exists(buildScriptAssemblyPath))
            {
                return null;
            }

            var buildScriptAssemblyModified = File.GetLastWriteTime(buildScriptAssemblyPath);
            var buildScriptFileModified = File.GetLastWriteTime(buildScriptFilePath);

            if (buildScriptFileModified > buildScriptAssemblyModified)
            {
                return null;
            }

            if (projectFileAnalyzerResult.ProjectFileFound)
            {
                var projectFileModified = File.GetLastWriteTime(projectFileAnalyzerResult.ProjectFileLocation);
                if (projectFileModified > buildScriptAssemblyModified)
                {
                    return null;
                }
            }

            foreach (var csFile in scriptAnalyzerResult.CsFiles)
            {
                if (File.Exists(csFile))
                {
                    var csFileModifiedTime = File.GetLastWriteTime(csFile);
                    if (csFileModifiedTime > buildScriptAssemblyModified)
                    {
                        return null;
                    }
                }
            }

            foreach (var partialCsFile in scriptAnalyzerResult.PartialCsFiles)
            {
                var csFileModifiedTime = File.GetLastWriteTime(partialCsFile);
                if (csFileModifiedTime > buildScriptAssemblyModified)
                {
                    return null;
                }
            }

            return Assembly.Load(File.ReadAllBytes(buildScriptAssemblyPath), File.ReadAllBytes(Path.ChangeExtension(buildScriptAssemblyPath, "pdb")));
        }

        private Assembly CompileBuildScript(string buildScriptAssemblyPath, string buildScriptFilePath, IEnumerable<MetadataReference> references, string code)
        {
            SyntaxTree syntaxTree =
                CSharpSyntaxTree.ParseText(SourceText.From(string.Join("\r\n", code), Encoding.UTF8));
            CSharpCompilation compilation = CSharpCompilation.Create(
                Path.GetFileName(buildScriptAssemblyPath),
                syntaxTrees: new[] { syntaxTree },
                references: references,
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
                    .WithOptimizationLevel(OptimizationLevel.Debug)
                    .WithAssemblyIdentityComparer(AssemblyIdentityComparer.Default));

            using (var dllStream = new MemoryStream())
            {
                using (var pdbStream = new MemoryStream())
                {
                    var emitOptions = new EmitOptions(false, DebugInformationFormat.PortablePdb);
                    EmitResult result = compilation.Emit(dllStream, pdbStream, options: emitOptions);

                    if (!result.Success)
                    {
                        var errorMsg = $"Csharp source code file: {buildScriptFilePath} has some compilation errors!";
                        IEnumerable<Diagnostic> failures = result.Diagnostics.Where(diagnostic =>
                            diagnostic.IsWarningAsError ||
                            diagnostic.Severity == DiagnosticSeverity.Error);
                        bool errorMsgDefined = false;
                        foreach (Diagnostic diagnostic in failures)
                        {
                            _log.LogWarning($"ScriptError:{diagnostic.Id}: {diagnostic.GetMessage()}");
                            if (errorMsgDefined)
                            {
                                continue;
                            }

                            switch (diagnostic.Id)
                            {
                                case "CS0012":
                                {
                                    errorMsg = $"{errorMsg} If your script doesn't have compilation errors in VS or VSCode script is probably missing some assembly references. To resolve this issue you should see build script fundamentals, section 'Referencing other assemblies in build script': https://flubucore.dotnetcore.xyz/referencing-external-assemblies/ more details.";
                                    errorMsgDefined = true;
                                    break;
                                }

                                case "CS0246":
                                {
                                    errorMsg = $"{errorMsg} If your script doesn't have compilation errors in VS or VSCode script is probably missing some assembly references or script doesn't include some .cs files. To resolve this issue you should see build script fundamentals, section 'Referencing other assemblies in build script' and section 'Adding other .cs files to script' for more details: {Environment.NewLine} https://flubucore.dotnetcore.xyz/referencing-external-assemblies/ {Environment.NewLine} https://flubucore.dotnetcore.xyz/referencing-external-assemblies/#adding-other-cs-files-to-script";
                                    errorMsgDefined = true;
                                    break;
                                }

                                case "CS0103":
                                {
                                    errorMsgDefined = true;
                                    errorMsg = $"{errorMsg} If your script doesn't have compilation errors in VS or VSCode script probably doesn't include some .cs files. To resolve this issue you should see build script fundamentals section 'Adding other .cs files to script' for more details: https://flubucore.dotnetcore.xyz/referencing-external-assemblies/#adding-other-cs-files-to-script";
                                    break;
                                }
                            }
                        }

                        throw new ScriptLoaderExcetpion(errorMsg);
                    }

                    dllStream.Seek(0, SeekOrigin.Begin);
                    pdbStream.Seek(0, SeekOrigin.Begin);
                    Directory.CreateDirectory(Path.GetDirectoryName(buildScriptAssemblyPath));
                    var dllData = dllStream.ToArray();
                    var pdbData = pdbStream.ToArray();
                    File.WriteAllBytes(buildScriptAssemblyPath, dllData);
                    File.WriteAllBytes(Path.ChangeExtension(buildScriptAssemblyPath, "pdb"), pdbData);

                    return Assembly.Load(dllData, pdbData);
                }
            }
        }

        private IEnumerable<MetadataReference> GetBuildScriptReferences(CommandArguments args, ProjectFileAnalyzerResult projectFileAnalyzerResult, ScriptAnalyzerResult scriptAnalyzerResult, bool oldWay, string pathToBuildScript)
        {
            var flubuCoreAssembly = typeof(DefaultBuildScript).GetTypeInfo().Assembly;

            //// Default assemblies that should be referenced.
            var assemblyReferences = oldWay
                ? GetBuildScriptReferencesForOldWayBuildScriptCreation()
                : GetDefaultReferences();

            // Enumerate all assemblies referenced by FlubuCore
            // and provide them as references to the build script we're about to
            // compile.
            AssemblyName[] flubuReferencedAssemblies = flubuCoreAssembly.GetReferencedAssemblies();
            foreach (var referencedAssembly in flubuReferencedAssemblies)
            {
                Assembly loadedAssembly = Assembly.Load(referencedAssembly);
                if (string.IsNullOrEmpty(loadedAssembly.Location))
                    continue;

                assemblyReferences.AddOrUpdateAssemblyInfo(new AssemblyInfo
                {
                    Name = referencedAssembly.Name,
                    Version = referencedAssembly.Version,
                    FullPath = loadedAssembly.Location,
                });
            }

            assemblyReferences.AddOrUpdateAssemblyInfo(scriptAnalyzerResult.AssemblyReferences);

            assemblyReferences.AddOrUpdateAssemblyInfo(projectFileAnalyzerResult.HasAnyNugetReferences
                ? _nugetPackageResolver.ResolveNugetPackagesFromFlubuCsproj(projectFileAnalyzerResult)
                : _nugetPackageResolver.ResolveNugetPackagesFromDirectives(scriptAnalyzerResult.NugetPackageReferences, pathToBuildScript));

            AddAssemblyReferencesFromCsproj(projectFileAnalyzerResult, assemblyReferences);

            var assemblyReferencesLocations = assemblyReferences.Select(x => x.FullPath).ToList();
            assemblyReferencesLocations.AddRange(FindAssemblyReferencesInDirectories(args.AssemblyDirectories));
            assemblyReferencesLocations =
                assemblyReferencesLocations.Distinct().Where(x => !string.IsNullOrEmpty(x)).ToList();

            var references = assemblyReferencesLocations.Select(i =>
            {
                return MetadataReference.CreateFromFile(i);
            });
#if !NET462
            foreach (var assemblyReferenceLocation in assemblyReferencesLocations)
            {
                try
                {
                    AssemblyLoadContext.Default.LoadFromAssemblyPath(assemblyReferenceLocation);
                }
                catch (Exception)
                {
                }
            }
#endif
            return references;
        }

        internal static List<AssemblyInfo> GetDefaultReferences()
        {
            var coreDir = Path.GetDirectoryName(typeof(object).GetTypeInfo().Assembly.Location);
            var flubuAss = typeof(DefaultBuildScript).GetTypeInfo().Assembly;
            var objAss = typeof(object).GetTypeInfo().Assembly;
            var linqAss = typeof(ILookup<string, string>).GetTypeInfo().Assembly;
#pragma warning disable SA1305 // Field names should not use Hungarian notation
            var ioAss = typeof(Stream).GetTypeInfo().Assembly;
#pragma warning restore SA1305 // Field names should not use Hungarian notation
            var linqExpAss = typeof(Expression).GetTypeInfo().Assembly;
            var reflectionAss = typeof(MethodInfo).GetTypeInfo().Assembly;
            var runtimeInteropAss = typeof(OSPlatform).GetTypeInfo().Assembly;
            var globalization = typeof(System.Globalization.CultureInfo).GetTypeInfo().Assembly;

            List<AssemblyInfo> assemblyReferenceLocations = new List<AssemblyInfo>
            {
               new AssemblyInfo
               {
                   Name = "mscorlib",
                   FullPath = Path.Combine(coreDir, "mscorlib.dll"),
                   VersionStatus = VersionStatus.Sealed,
               },
               new AssemblyInfo
               {
                   Name = "System.Runtime",
                   FullPath = Path.Combine(coreDir, "System.Runtime.dll"),
                   VersionStatus = VersionStatus.Sealed
               },
               flubuAss.ToAssemblyInfo(),
               objAss.ToAssemblyInfo(),
               linqAss.ToAssemblyInfo(),
               ioAss.ToAssemblyInfo(),
               linqExpAss.ToAssemblyInfo(),
               reflectionAss.ToAssemblyInfo(),
               runtimeInteropAss.ToAssemblyInfo(),
               globalization.ToAssemblyInfo()
            };

            assemblyReferenceLocations.AddReferenceByAssemblyName("System");
            assemblyReferenceLocations.AddReferenceByAssemblyName("System.Core");
            assemblyReferenceLocations.AddReferenceByAssemblyName("System.Data");
            assemblyReferenceLocations.AddReferenceByAssemblyName("System.Runtime.Extensions");
            assemblyReferenceLocations.AddReferenceByAssemblyName("System.Runtime.InteropServices");
            assemblyReferenceLocations.AddReferenceByAssemblyName("System.Collections");
            assemblyReferenceLocations.AddReferenceByAssemblyName("System.IO.FileSystem");
            assemblyReferenceLocations.AddReferenceByAssemblyName("System.IO.FileSystem.Primitives");
            assemblyReferenceLocations.AddReferenceByAssemblyName("System.Reflection");
            assemblyReferenceLocations.AddReferenceByAssemblyName("System.Reflection.Extensions");
            assemblyReferenceLocations.AddReferenceByAssemblyName("System.Reflection.Primitives");
            assemblyReferenceLocations.AddReferenceByAssemblyName("System.Text.Encoding");
            assemblyReferenceLocations.AddReferenceByAssemblyName("System.Text.RegularExpressions");
            assemblyReferenceLocations.AddReferenceByAssemblyName("System.Threading");
            assemblyReferenceLocations.AddReferenceByAssemblyName("System.Threading.Tasks");
            assemblyReferenceLocations.AddReferenceByAssemblyName("System.Threading.Tasks.Parallel");
            assemblyReferenceLocations.AddReferenceByAssemblyName("System.Threading.Thread");
            assemblyReferenceLocations.AddReferenceByAssemblyName("System.Globalization");
            assemblyReferenceLocations.AddReferenceByAssemblyName("System.Private.Uri");
            assemblyReferenceLocations.AddReferenceByAssemblyName("System.Drawing.Primitives");
            assemblyReferenceLocations.AddReferenceByAssemblyName("System.Net.Primitives");
            assemblyReferenceLocations.AddReferenceByAssemblyName("System.Net.WebClient");
            assemblyReferenceLocations.AddReferenceByAssemblyName("System.Net.Http");
            assemblyReferenceLocations.AddReferenceByAssemblyName("System.Private.Xml");
            assemblyReferenceLocations.AddReferenceByAssemblyName("System.Private.CoreLib");
            assemblyReferenceLocations.AddReferenceByAssemblyName("System.Text.Json");
            assemblyReferenceLocations.AddReferenceByAssemblyName("System.Text.Json.Serialization");
            assemblyReferenceLocations.AddReferenceByAssemblyName("System.Xml.XPath");
            assemblyReferenceLocations.AddReferenceByAssemblyName("System.Xml.Serialization");
            assemblyReferenceLocations.AddReferenceByAssemblyName("System.Data.Common");
#if NETSTANDARD2_0
            var systemAss = typeof(Console).GetTypeInfo().Assembly;
            assemblyReferenceLocations.Add(systemAss.ToAssemblyInfo());
#endif
            return assemblyReferenceLocations;
        }

        private static List<AssemblyInfo> GetBuildScriptReferencesForOldWayBuildScriptCreation()
        {
            var coreDir = Path.GetDirectoryName(typeof(object).GetTypeInfo().Assembly.Location);
            var flubuAss = typeof(DefaultBuildScript).GetTypeInfo().Assembly;

            var objAss = typeof(object).GetTypeInfo().Assembly;
#pragma warning disable SA1305 // Field names should not use Hungarian notation
            var ioAss = typeof(File).GetTypeInfo().Assembly;
#pragma warning restore SA1305 // Field names should not use Hungarian notation
            var linqAss = typeof(ILookup<string, string>).GetTypeInfo().Assembly;
            var linqExpAss = typeof(Expression).GetTypeInfo().Assembly;
            var runtimeInteropAss = typeof(OSPlatform).GetTypeInfo().Assembly;
            List<AssemblyInfo> assemblyReferenceLocations = new List<AssemblyInfo>
            {
                new AssemblyInfo
                {
                    Name = "mscorlib",
                    FullPath = Path.Combine(coreDir, "mscorlib.dll"),
                    VersionStatus = VersionStatus.Sealed,
                },
                flubuAss.ToAssemblyInfo(),
                objAss.ToAssemblyInfo(),
                ioAss.ToAssemblyInfo(),
                linqAss.ToAssemblyInfo(),
                linqExpAss.ToAssemblyInfo(),
                runtimeInteropAss.ToAssemblyInfo(),
            };

#if NETSTANDARD2_0
            var systemAss = typeof(Console).GetTypeInfo().Assembly;
            assemblyReferenceLocations.Add(systemAss.ToAssemblyInfo());
#endif

            return assemblyReferenceLocations;
        }

        private void ProcessAddedCsFilesToBuildScript(ScriptAnalyzerResult analyzerResult, List<string> code)
        {
            List<string> csFiles = new List<string>();

            foreach (var csDirectory in analyzerResult.CsDirectories)
            {
                if (Directory.Exists(csDirectory.Item1.path))
                {
                    var searchOption = csDirectory.Item1.includeSubDirectories
                        ? SearchOption.AllDirectories
                        : SearchOption.TopDirectoryOnly;

                    csFiles.AddRange(Directory.GetFiles(csDirectory.Item1.path, "*.cs", searchOption));
                }
                else
                {
                    _log.LogInformation($"Directory not found: '{csDirectory.Item1.path}'.");
                }
            }

            csFiles.AddRange(analyzerResult.CsFiles);
            List<string> namespaces = new List<string>();
            foreach (var file in csFiles)
            {
                if (_file.Exists(file))
                {
                    _log.LogInformation($"File found: {file}");
                    List<string> additionalCode = _file.ReadAllLines(file);

                    ScriptAnalyzerResult additionalCodeAnalyzerResult = _scriptAnalyzer.Analyze(additionalCode);
                    if (additionalCodeAnalyzerResult.CsFiles.Count > 0)
                    {
                        throw new NotSupportedException("//#imp is only supported in main buildscript .cs file.");
                    }

                    namespaces.Add(additionalCodeAnalyzerResult.Namespace);

                    var usings = additionalCode.Where(x => x.StartsWith("using"));

                    analyzerResult.AssemblyReferences.AddRange(additionalCodeAnalyzerResult.AssemblyReferences);

                    code.InsertRange(1, usings);
                    code.AddRange(additionalCode.Where(x => !x.StartsWith("using")));
                }
                else
                {
                    _log.LogInformation($"File was not found: {file}");
                }
            }

            namespaces = namespaces.Distinct().ToList();
            foreach (var ns in namespaces)
            {
                var usng = $"using {ns};";
                code.Remove(usng);
            }
        }

        private void ProcessPartialBuildScriptClasses(ScriptAnalyzerResult analyzerResult, List<string> code, string buildScriptLocation)
        {
            if (!analyzerResult.IsPartial)
            {
                return;
            }

            var buildScriptDir = Path.GetDirectoryName(Path.GetFullPath(buildScriptLocation));
            var fileName = Path.GetFileNameWithoutExtension(buildScriptLocation);
            var scriptFiles = Directory.GetFiles(buildScriptDir, "*.cs");

            foreach (var scriptFile in scriptFiles)
            {
                if (Path.GetFileNameWithoutExtension(scriptFile).Equals(fileName, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                List<string> additionalCode = _file.ReadAllLines(scriptFile);

                ScriptAnalyzerResult additionalCodeAnalyzerResult = _scriptAnalyzer.Analyze(additionalCode);

                if (!additionalCodeAnalyzerResult.IsPartial || additionalCodeAnalyzerResult.ClassName != analyzerResult.ClassName)
                {
                    continue;
                }

                _log.LogInformation($"Loading Partial class of build script: {scriptFile}");

                var usings = additionalCode.Where(x => x.StartsWith("using"));
                analyzerResult.PartialCsFiles.Add(scriptFile);
                analyzerResult.AssemblyReferences.AddRange(additionalCodeAnalyzerResult.AssemblyReferences);
                code.InsertRange(1, usings);
                code.AddRange(additionalCode.Where(x => !x.StartsWith("using")));
            }
        }

        private List<string> FindAssemblyReferencesInDirectories(List<string> directories)
        {
            List<string> assemblyLocations = new List<string>();
            directories.AddRange(DefaultScriptReferencesLocations);
            foreach (var assemblyReferencesLocation in directories)
            {
                if (_directory.Exists(assemblyReferencesLocation))
                {
                    DirectoryInfo folder = new DirectoryInfo(assemblyReferencesLocation);
                    var files = folder.GetFiles("*.dll", SearchOption.AllDirectories);
                    assemblyLocations.AddRange(files.Select(x => x.FullName));
                }
            }

            return assemblyLocations;
        }

        private async Task<IBuildScript> CreateBuildScriptInstanceOldWay(string buildScriptFIlePath, IEnumerable<MetadataReference> references, List<string> code,  ScriptAnalyzerResult analyzerResult)
        {
            ScriptOptions opts = ScriptOptions.Default
                .WithEmitDebugInformation(true)
                .WithFilePath(buildScriptFIlePath)
                .WithFileEncoding(Encoding.UTF8)
                .WithReferences(references);

            Script script = CSharpScript
                .Create(string.Join("\r\n", code), opts)
                .ContinueWith(string.Format("var sc = new {0}();", analyzerResult.ClassName));

            try
            {
                ScriptState result = await script.RunAsync();

                var buildScript = result.Variables[0].Value as IBuildScript;

                if (buildScript == null)
                {
                    throw new ScriptLoaderExcetpion($"Class in file: {buildScriptFIlePath} must inherit from DefaultBuildScript or implement IBuildScipt interface. See getting started on https://github.com/flubu-core/flubu.core/wiki");
                }

                return buildScript;
            }
            catch (CompilationErrorException e)
            {
                if (e.Message.Contains("CS0234"))
                {
                    throw new ScriptLoaderExcetpion($"Csharp source code file: {buildScriptFIlePath} has some compilation errors. {e.Message}. If your script doesnt have compilation errors in VS or VSCode script is probably missing some assembly reference. To resolve this issue you should see build script fundamentals, section 'Referencing other assemblies in build script': https://github.com/flubu-core/flubu.core/wiki/2-Build-script-fundamentals#Referencing-other-assemblies-in-build-script for more details.", e);
                }

                if (e.Message.Contains("CS0246"))
                {
                    throw new ScriptLoaderExcetpion($"Csharp source code file: {buildScriptFIlePath} has some compilation errors. {e.Message}. If your script doesnt have compilation errors in VS or VSCode script is probably missing some assembly reference or script doesnt include .cs file. To resolve this issue you should see build script fundamentals, section 'Referencing other assemblies in build script' and section 'Adding other .cs files to script' for more details: {Environment.NewLine} https://github.com/flubu-core/flubu.core/wiki/2-Build-script-fundamentals#Referencing-other-assemblies-in-build-script {Environment.NewLine} https://github.com/flubu-core/flubu.core/wiki/2-Build-Script-Fundamentals#Adding-other-cs-files-to-build-script", e);
                }

                if (e.Message.Contains("CS0103"))
                {
                    throw new ScriptLoaderExcetpion($"Csharp source code file: {buildScriptFIlePath} has some compilation errors. {e.Message}. If your script doesn't have compilation errors in VS or VSCode script probably doesn't include .cs file. To resolve this issue you should see build script fundamentals section 'Adding other .cs files to script' for more details: https://github.com/flubu-core/flubu.core/wiki/2-Build-Script-Fundamentals#Adding-other-cs-files-to-build-script", e);
                }

                throw new ScriptLoaderExcetpion($"Csharp source code file: {buildScriptFIlePath} has some compilation errors. {e.Message}.", e);
            }
        }

#pragma warning disable SA1204 // Static elements should appear before instance elements
        private static void AddAssemblyReferencesFromCsproj(ProjectFileAnalyzerResult projectFileAnalyzerResult, List<AssemblyInfo> assemblyReferences)
#pragma warning restore SA1204 // Static elements should appear before instance elements
        {
            if (!projectFileAnalyzerResult.HasAnyAssemblyReferences)
            {
                return;
            }

            foreach (var reference in projectFileAnalyzerResult.AssemblyReferences)
            {
                if (!string.IsNullOrEmpty(reference.Path))
                {
                    assemblyReferences.AddOrUpdateAssemblyInfo(new AssemblyInfo
                    {
                        VersionStatus = VersionStatus.NotAvailable,
                        FullPath = reference.Path,
                        Name = reference.Name
                    });
                }
                else
                {
                    assemblyReferences.AddReferenceByAssemblyName(reference.Name);
                }
            }
        }

        private void ProcessConfigAttributes(ScriptAnalyzerResult scriptAnalyzerResult)
        {
            if (scriptAnalyzerResult.ScriptAttributes.Contains(ScriptConfigAttributes.DisableColoredLogging))
            {
                FlubuConsoleLogger.DisableColloredLogging = true;
            }
        }
    }
}