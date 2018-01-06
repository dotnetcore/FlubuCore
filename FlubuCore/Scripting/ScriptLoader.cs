using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            List<string> referencesLocation = new List<string>
            {
                Path.Combine(coreDir, "mscorlib.dll"),
                typeof(object).GetTypeInfo().Assembly.Location,
                flubuPath,
                typeof(File).GetTypeInfo().Assembly.Location
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

                referencesLocation.Add(loadedAssembly.Location);
            }

            string fileName = _buildScriptLocator.FindBuildScript(args);

            List<string> code = _file.ReadAllLines(fileName);

            AnalyserResult analyserResult = _analyser.Analyze(code);
            referencesLocation.AddRange(analyserResult.References);

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

                    referencesLocation.AddRange(additionalCodeAnalyserResult.References);
                    code.InsertRange(0, usings);
                    code.AddRange(additionalCode.Where(x => !x.StartsWith("using")));
                }
                else
                {
                    _log.LogInformation($"File was not found: {file}");
                }
            }

            referencesLocation.AddRange(FindAssemblyReferencesInDirectories(args.AssemblyDirectories));

            referencesLocation = referencesLocation.Distinct().ToList();
            references.AddRange(referencesLocation.Select(i => MetadataReference.CreateFromFile(i)));
            var opts = ScriptOptions.Default
                .WithReferences(references);

            Script script = CSharpScript
                .Create(string.Join("\r\n", code), opts)
                .ContinueWith(string.Format("var sc = new {0}();", analyserResult.ClassName));

            ScriptState result = await script.RunAsync();

            return result.Variables[0].Value as IBuildScript;
        }

        private List<string> FindAssemblyReferencesInDirectories(List<string> directories)
        {
            List<string> assemblieLocations = new List<string>();
            directories.AddRange(DefaultScriptReferencesLocations);
            foreach (var referencesLocation in directories)
            {
                if (_directory.Exists(referencesLocation))
                {
                    assemblieLocations.AddRange(_directory.GetFiles(referencesLocation, "*.dll", SearchOption.AllDirectories));
                }
            }

            return assemblieLocations;
        }
    }
}