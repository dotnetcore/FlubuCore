using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using FlubuCore.IO.Wrappers;
using FlubuCore.Scripting.Analysis;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.Extensions.Logging;

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

        private readonly ILogger<ScriptLoader> _log;

        public ScriptLoader(IFileWrapper file, IDirectoryWrapper directory, IScriptAnalyser analyser, IBuildScriptLocator buildScriptLocator, ILogger<ScriptLoader> log)
        {
            _file = file;
            _directory = directory;
            _analyser = analyser;
            _log = log;
            _buildScriptLocator = buildScriptLocator;
        }

        public async Task<IBuildScript> FindAndCreateBuildScriptInstanceAsync(CommandArguments args)
        {
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
        };

            List<MetadataReference> references = new List<MetadataReference>();

#if NETSTANDARD2_0
            referencesLocation.Add(typeof(Console).GetTypeInfo().Assembly.Location);
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
            references.AddRange(assemblyReferenceLocations.Select(i => MetadataReference.CreateFromFile(i)));
            var opts = ScriptOptions.Default
                .WithReferences(references);

            Script script = CSharpScript
                .Create(string.Join("\r\n", code), opts)
                .ContinueWith(string.Format("var sc = new {0}();", analyserResult.ClassName));

            try
            {
                ScriptState result = await script.RunAsync();

                var buildScript = result.Variables[0].Value as IBuildScript;

                if (buildScript == null)
                {
                    throw new ScriptLoaderExcetpion($"Class in file: {fileName} must inherit from DefaultBuildScript or implement IBuildScipt interface. See getting started on https://github.com/flubu-core/flubu.core/wiki");
                }

                return buildScript;
            }
            catch (CompilationErrorException e)
            {
                if (e.Message.Contains("CS0234"))
                {
                    throw new ScriptLoaderExcetpion($"Csharp source code file: {fileName} has some compilation errors. {e.Message}. If u are using flubu script correctly you have to add assembly reference with #ref directive in build script. See build script fundamentals section 'Referencing other assemblies in build script' in https://github.com/flubu-core/flubu.core/wiki for more details.Otherwise if u think u are not using flubu correctly see Getting started section in wiki.", e);
                }

                throw new ScriptLoaderExcetpion($"Csharp source code file: {fileName} has some compilation errors. {e.Message}. See getting started and build script fundamentals in https://github.com/flubu-core/flubu.core/wiki", e);
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