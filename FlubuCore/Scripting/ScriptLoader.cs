using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
#if NETSTANDARD1_6
using System.Runtime.Loader;
#endif
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FlubuCore.IO.Wrappers;
using FlubuCore.Scripting.Analysis;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
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
            "flubulib",
            "buildscript\\flubulib",
            "buildscripts\\flubulib",
        };

        private readonly IFileWrapper _file;
        private readonly IDirectoryWrapper _directory;
        private readonly IScriptAnalyser _analyser;
        private readonly IBuildScriptLocator _buildScriptLocator;
        private readonly INugetPackageResolver _nugetPackageResolver;

        private readonly ILogger<ScriptLoader> _log;

        public ScriptLoader(
            IFileWrapper file,
            IDirectoryWrapper directory,
            IScriptAnalyser analyser,
            IBuildScriptLocator buildScriptLocator,
            INugetPackageResolver nugetPackageResolver,
            ILogger<ScriptLoader> log)
        {
            _file = file;
            _directory = directory;
            _analyser = analyser;
            _log = log;
            _buildScriptLocator = buildScriptLocator;
            _nugetPackageResolver = nugetPackageResolver;
        }

        public async Task<IBuildScript> FindAndCreateBuildScriptInstanceAsync(CommandArguments args)
        {
            Stopwatch sw = Stopwatch.StartNew();
            var coreDir = Path.GetDirectoryName(typeof(object).GetTypeInfo().Assembly.Location);
            var flubuPath = typeof(DefaultBuildScript).GetTypeInfo().Assembly.Location;

            List<string> assemblyReferenceLocations = new List<string>
            {
                Path.Combine(coreDir, "mscorlib.dll"),
                typeof(object).GetTypeInfo().Assembly.Location,
                flubuPath,
                typeof(File).GetTypeInfo().Assembly.Location,
                typeof(ILookup<string, string>).GetTypeInfo().Assembly.Location,
                typeof(Expression).GetTypeInfo().Assembly.Location,
                typeof(MethodInfo).GetTypeInfo().Assembly.Location
        };

#if NETSTANDARD1_6
           assemblyReferenceLocations.Add(Assembly.Load(new AssemblyName("System.Reflection, Version=4.0.10.0")).Location);
#endif
#if NETSTANDARD2_0
            assemblyReferenceLocations.Add(typeof(Console).GetTypeInfo().Assembly.Location);
            assemblyReferenceLocations.Add(Assembly.Load(new AssemblyName("netstandard, Version=2.0.0.0")).Location);
#endif

            // Enumerate all assemblies referenced by this executing assembly
            // and provide them as references to the build script we're about to
            // compile.
            AssemblyName[] referencedAssemblies = Assembly.GetEntryAssembly().GetReferencedAssemblies();
            foreach (var referencedAssembly in referencedAssemblies)
            {
                Assembly loadedAssembly = Assembly.Load(referencedAssembly);
                if (string.IsNullOrEmpty(loadedAssembly.Location))
                    continue;

                assemblyReferenceLocations.Add(loadedAssembly.Location);
            }

            string fileName = _buildScriptLocator.FindBuildScript(args);

            List<string> code = _file.ReadAllLines(fileName);

            AnalyserResult analyserResult = _analyser.Analyze(code);
            assemblyReferenceLocations.AddRange(analyserResult.References);
            assemblyReferenceLocations.AddRange(_nugetPackageResolver.ResolveNugetPackages(analyserResult.NugetPackages));
            foreach (var file in analyserResult.CsFiles)
            {
                if (_file.Exists(file))
                {
                    _log.LogInformation($"File found: {file}");
                    List<string> additionalCode = _file.ReadAllLines(file);

                    AnalyserResult additionalCodeAnalyserResult = _analyser.Analyze(additionalCode);
                    if (additionalCodeAnalyserResult.CsFiles.Count > 0)
                    {
                        throw new NotSupportedException("//#imp is only supported in main buildscript .cs file.");
                    }

                    var usings = additionalCode.Where(x => x.StartsWith("using"));

                    assemblyReferenceLocations.AddRange(additionalCodeAnalyserResult.References);
                    code.InsertRange(0, usings);
                    code.AddRange(additionalCode.Where(x => !x.StartsWith("using")));
                }
                else
                {
                    _log.LogInformation($"File was not found: {file}");
                }
            }

            assemblyReferenceLocations.AddRange(FindAssemblyReferencesInDirectories(args.AssemblyDirectories));
            assemblyReferenceLocations = assemblyReferenceLocations.Distinct().ToList();

            List<MetadataReference> references = new List<MetadataReference>();
            references.AddRange(assemblyReferenceLocations.Select(i => MetadataReference.CreateFromFile(i)));

            try
            {
                Assembly assembly = TryLoadBuildScriptFromAssembly() ?? CompileBuildScriptToAssembly(fileName, references, string.Join("\r\n", code));

                TypeInfo type = assembly.DefinedTypes.FirstOrDefault(i => i.BaseType == typeof(DefaultBuildScript));

                object obj = Activator.CreateInstance(type.AsType());
                if (!(obj is IBuildScript))
                {
                    throw new ScriptLoaderExcetpion($"Class in file: {fileName} must inherit from DefaultBuildScript or implement IBuildScipt interface. See getting started on https://github.com/flubu-core/flubu.core/wiki");
                }

                Debug.WriteLine($"Script loaded in Miliseconds: {sw.ElapsedMilliseconds}");
                Console.WriteLine($"Script loaded in Miliseconds: {sw.ElapsedMilliseconds}");

                return obj as IBuildScript;

                throw new ScriptLoaderExcetpion($"Class in file: {fileName} must inherit from DefaultBuildScript or implement IBuildScipt interface. See getting started on https://github.com/flubu-core/flubu.core/wiki");
            }
            catch (CompilationErrorException e)
            {
                Console.WriteLine(e);
                if (e.Message.Contains("CS0234"))
                {
                    throw new ScriptLoaderExcetpion($"Csharp source code file: {fileName} has some compilation errors. {e.Message}. If u are using flubu script correctly you have to add assembly reference with #ref directive in build script. See build script fundamentals section 'Referencing other assemblies in build script' in https://github.com/flubu-core/flubu.core/wiki for more details.Otherwise if u think u are not using flubu correctly see Getting started section in wiki.", e);
                }

                throw new ScriptLoaderExcetpion($"Csharp source code file: {fileName} has some compilation errors. {e.Message}. See getting started and build script fundamentals in https://github.com/flubu-core/flubu.core/wiki", e);
            }
        }

        private Assembly TryLoadBuildScriptFromAssembly()
        {
            if (!File.Exists("file.dll"))
                return null;
#if NETSTANDARD1_6
            return AssemblyLoadContext.Default.LoadFromAssemblyPath(Path.GetFullPath("file.dll"));
#else
            return Assembly.Load(File.ReadAllBytes("file.dll"));
#endif
        }

        private Assembly CompileBuildScriptToAssembly(string fileName, List<MetadataReference> references, string code)
        {
            SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(SourceText.From(string.Join("\r\n", code), Encoding.UTF8));
            CSharpCompilation compilation = CSharpCompilation.Create(
                "file.dll",
                syntaxTrees: new[] { syntaxTree },
                references: references,
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            using (var ms = new MemoryStream())
            {
                EmitResult result = compilation.Emit(ms);

                if (!result.Success)
                {
                    IEnumerable<Diagnostic> failures = result.Diagnostics.Where(diagnostic =>
                        diagnostic.IsWarningAsError ||
                        diagnostic.Severity == DiagnosticSeverity.Error);

                    foreach (Diagnostic diagnostic in failures)
                    {
                        Console.WriteLine("ScriptError:{0}: {1}", diagnostic.Id, diagnostic.GetMessage());
                        Debug.WriteLine("ScriptError:{0}: {1}", diagnostic.Id, diagnostic.GetMessage());
                    }

                    throw new ScriptLoaderExcetpion("Script has errors!");
                }

                ms.Seek(0, SeekOrigin.Begin);
                var data = ms.ToArray();
                File.WriteAllBytes("file.dll", data);

#if  NETSTANDARD1_6
                ms.Seek(0, SeekOrigin.Begin);
                return AssemblyLoadContext.Default.LoadFromStream(ms);
#else
                return Assembly.Load(data);
#endif
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
    }
}