using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using FlubuCore.IO.Wrappers;
using FlubuCore.Scripting.Analysis;
using FlubuCore.Scripting.Attributes;
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

        public async Task<IBuildScript> FindAndCreateBuildScriptInstanceAsync(CommandArguments args)
        {
            string buildScriptFilePath = _buildScriptLocator.FindBuildScript(args);
            var buildScriptAssemblyPath = Path.Combine("bin", Path.GetFileName(buildScriptFilePath));
            buildScriptAssemblyPath = Path.ChangeExtension(buildScriptAssemblyPath, "dll");

            List<string> code = _file.ReadAllLines(buildScriptFilePath);
            ScriptAnalyzerResult scriptAnalyzerResult = _scriptAnalyzer.Analyze(code);
            ProjectFileAnalyzerResult projectFileAnalyzerResult = _projectFileAnalyzer.Analyze(disableAnalysis: scriptAnalyzerResult.ScriptAttributes.Contains(ScriptAttributes.DisableLoadScriptReferencesAutomatically));
            bool oldWay = false;
#if NET462
          oldWay = true;
#endif
            var references = GetBuildScriptReferences(args, projectFileAnalyzerResult, scriptAnalyzerResult, code, oldWay, buildScriptFilePath);

            if (oldWay)
            {
                return await CreateBuildScriptInstanceOldWay(buildScriptFilePath, references, code, scriptAnalyzerResult);
            }

            var assembly = TryLoadBuildScriptFromAssembly(buildScriptAssemblyPath, buildScriptFilePath);

            if (assembly != null)
            {
                return CreateBuildScriptInstance(assembly, buildScriptFilePath);
            }

            code.Insert(0, $"#line 1 \"{buildScriptFilePath}\"");
            assembly = CompileBuildScriptToAssembly(buildScriptAssemblyPath, buildScriptFilePath, references, string.Join("\r\n", code));
            return CreateBuildScriptInstance(assembly, buildScriptFilePath);
        }

        private static IBuildScript CreateBuildScriptInstance(Assembly assembly, string fileName)
        {
            TypeInfo type = assembly.DefinedTypes.FirstOrDefault(i => i.ImplementedInterfaces.Any(x => x == typeof(IBuildScript)));

            if (type == null)
            {
                throw new ScriptLoaderExcetpion($"Class in file: {fileName} must inherit from DefaultBuildScript or implement IBuildScipt interface. See getting started on https://github.com/flubu-core/flubu.core/wiki");
            }

            var obj = Activator.CreateInstance(type.AsType());

            var buildScript = obj as IBuildScript ?? throw new ScriptLoaderExcetpion($"Class in file: {fileName} must inherit from DefaultBuildScript or implement IBuildScipt interface. See getting started on https://github.com/flubu-core/flubu.core/wiki");
            return buildScript;
        }

        private Assembly TryLoadBuildScriptFromAssembly(string buildScriptAssemblyPath, string buildScriptFilePath)
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

#if NETSTANDARD1_6
             using (FileStream dllStream = new FileStream(buildScriptAssemblyPath, FileMode.Open, FileAccess.Read))
             {
                using (FileStream pdbStream = new FileStream(Path.ChangeExtension(buildScriptAssemblyPath, "pdb"), FileMode.Open, FileAccess.Read))
                {
                   return AssemblyLoadContext.Default.LoadFromStream(dllStream, pdbStream);
                }
             }
#else
            return Assembly.Load(File.ReadAllBytes(buildScriptAssemblyPath), File.ReadAllBytes(Path.ChangeExtension(buildScriptAssemblyPath, "pdb")));
#endif
        }

        private Assembly CompileBuildScriptToAssembly(string buildScriptAssemblyPath, string buildScriptFilePath, IEnumerable<MetadataReference> references, string code)
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
                        IEnumerable<Diagnostic> failures = result.Diagnostics.Where(diagnostic =>
                            diagnostic.IsWarningAsError ||
                            diagnostic.Severity == DiagnosticSeverity.Error);
                        bool possiblyMissingAssemblyRefDirective = false;
                        foreach (Diagnostic diagnostic in failures)
                        {
                            _log.LogWarning($"ScriptError:{diagnostic.Id}: {diagnostic.GetMessage()}");
                            switch (diagnostic.Id)
                            {
                                case "CS0246":
                                case "CS0012":
                                    possiblyMissingAssemblyRefDirective = true;
                                    break;
                            }
                        }

                        var errorMsg = $"Csharp source code file: {buildScriptFilePath} has some compilation errors!";
                        if (possiblyMissingAssemblyRefDirective)
                        {
                            errorMsg =
                                $"{errorMsg} If your script doesnt have compilation errors in VS or VSCode you are probably missing assembly directive in build script. You have to add reference to all assemblies that flubu doesn't add by default with #ass, #nuget or #ref directive in build script. See build script fundamentals section 'Referencing other assemblies in build script' in https://github.com/flubu-core/flubu.core/wiki/2-Build-script-fundamentals#Referencing-other-assemblies-in-build-script for more details.";
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

#if NETSTANDARD1_6
                    dllStream.Seek(0, SeekOrigin.Begin);
                    pdbStream.Seek(0, SeekOrigin.Begin);
                    return AssemblyLoadContext.Default.LoadFromStream(dllStream, pdbStream);
#else
                    return Assembly.Load(dllData, pdbData);
#endif
                }
            }
        }

        private IEnumerable<MetadataReference> GetBuildScriptReferences(CommandArguments args, ProjectFileAnalyzerResult projectFileAnalyzerResult, ScriptAnalyzerResult scriptAnalyzerResult, List<string> code, bool oldWay, string pathToBuildScript)
        {
            var coreDir = Path.GetDirectoryName(typeof(object).GetTypeInfo().Assembly.Location);
            var flubuCoreAssembly = typeof(DefaultBuildScript).GetTypeInfo().Assembly;
            var flubuPath = flubuCoreAssembly.Location;

            //// Default assemblies that should be referenced.
            var assemblyReferences = oldWay
                ? GetBuildScriptReferencesForOldWayBuildScriptCreation()
                : GetDefaultReferences(coreDir, flubuPath);

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

            if (projectFileAnalyzerResult.ProjectFileFound)
            {
                assemblyReferences.AddOrUpdateAssemblyInfo(_nugetPackageResolver.ResolveNugetPackagesFromFlubuCsproj(projectFileAnalyzerResult));
                AddAssemblyReferencesFromCsproj(projectFileAnalyzerResult, assemblyReferences);
            }
            else
            {
                assemblyReferences.AddOrUpdateAssemblyInfo(_nugetPackageResolver.ResolveNugetPackagesFromDirectives(scriptAnalyzerResult.NugetPackageReferences, pathToBuildScript));
            }

            AddOtherCsFilesToBuildScriptCode(scriptAnalyzerResult, assemblyReferences, code);
            var assemblyReferencesLocations = assemblyReferences.Select(x => x.FullPath).ToList();
            assemblyReferencesLocations.AddRange(FindAssemblyReferencesInDirectories(args.AssemblyDirectories));
            assemblyReferencesLocations =
                assemblyReferencesLocations.Distinct().Where(x => !string.IsNullOrEmpty(x)).ToList();
            IEnumerable<PortableExecutableReference> references = null;

            references = assemblyReferencesLocations.Select(i =>
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
                catch (Exception ex)
                {
                }
            }
#endif
            return references;
        }

        private List<AssemblyInfo> GetDefaultReferences(string coreDir, string flubuPath)
        {
            var flubuAss = typeof(DefaultBuildScript).GetTypeInfo().Assembly;
            var objAss = typeof(object).GetTypeInfo().Assembly;
            var linqAss = typeof(ILookup<string, string>).GetTypeInfo().Assembly;
#pragma warning disable SA1305 // Field names should not use Hungarian notation
            var ioAss = typeof(Stream).GetTypeInfo().Assembly;
#pragma warning restore SA1305 // Field names should not use Hungarian notation
            var linqExpAss = typeof(Expression).GetTypeInfo().Assembly;
            var reflectionAss = typeof(MethodInfo).GetTypeInfo().Assembly;
            var runtimeInteropAss = typeof(OSPlatform).GetTypeInfo().Assembly;
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

#if NETSTANDARD2_0
            var systemAss = typeof(Console).GetTypeInfo().Assembly;
            assemblyReferenceLocations.Add(systemAss.ToAssemblyInfo());
#endif
            return assemblyReferenceLocations;
        }

        private List<AssemblyInfo> GetBuildScriptReferencesForOldWayBuildScriptCreation()
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

        private void AddOtherCsFilesToBuildScriptCode(ScriptAnalyzerResult analyzerResult, List<AssemblyInfo> assemblyReferenceLocations, List<string> code)
        {
            foreach (var file in analyzerResult.CsFiles)
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

                    var usings = additionalCode.Where(x => x.StartsWith("using"));

                    assemblyReferenceLocations.AddOrUpdateAssemblyInfo(additionalCodeAnalyzerResult.AssemblyReferences);
                    code.InsertRange(1, usings);
                    code.AddRange(additionalCode.Where(x => !x.StartsWith("using")));
                }
                else
                {
                    _log.LogInformation($"File was not found: {file}");
                }
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
                    throw new ScriptLoaderExcetpion($"Csharp source code file: {buildScriptFIlePath} has some compilation errors. {e.Message}. If your script doesnt have compilation errors in VS or VSCode you are probably missing assembly directive in build script. You have to add reference to all assemblies that flubu doesn't add by default with #ass, #nuget or #ref directive in build script. See build script fundamentals section 'Referencing other assemblies in build script' in https://github.com/flubu-core/flubu.core/wiki/2-Build-script-fundamentals#Referencing-other-assemblies-in-build-script for more details.", e);
                }

                throw new ScriptLoaderExcetpion($"Csharp source code file: {buildScriptFIlePath} has some compilation errors. {e.Message}.", e);
            }
        }

#pragma warning disable SA1204 // Static elements should appear before instance elements
        private static void AddAssemblyReferencesFromCsproj(ProjectFileAnalyzerResult projectFileAnalyzerResult, List<AssemblyInfo> assemblyReferences)
#pragma warning restore SA1204 // Static elements should appear before instance elements
        {
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
    }
}