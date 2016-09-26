using System;
using System.Reflection;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace Flubu.Scripting
{
    public class ScriptLoader : IScriptLoader
    {
        private readonly IFileLoader _fileLoader;
        private readonly CommandArguments _args;

        public ScriptLoader(IFileLoader fileLoader, CommandArguments args)
        {
            _fileLoader = fileLoader;
            _args = args;
        }

        public async Task<IBuildScript> FindAndCreateBuildScriptInstanceAsync(string fileName)
        {
            var opts = ScriptOptions.Default
                .WithReferences(LoadAssembly<object>())
                .WithReferences(LoadAssembly<DefaultBuildScript>())
                .WithReferences(LoadAssembly<IBuildScript>());

            // todo create correct class
            Script script;

            if (!string.IsNullOrEmpty(_args.ScriptAssembly))
            {
                opts
                    .AddReferences(_args.ScriptAssembly)
                    .AddImports("Flubu.BuildScript");

                script = CSharpScript
                    .Create(@"var sc = new MyBuildScript();", opts);
            }
            else
            {
                string code = _fileLoader.LoadFile(fileName);

                script = CSharpScript
                    .Create(code, opts)
                    .ContinueWith("var sc = new MyBuildScript();");
            }

            ScriptState result = await script.RunAsync();

            return result.Variables[0].Value as IBuildScript;
        }

        private static unsafe PortableExecutableReference LoadAssembly<T>()
        {
            byte* fl;
            int length;
            Assembly assembly = typeof(T).GetTypeInfo().Assembly; ////let's grab the current in-memory assembly
            assembly.TryGetRawMetadata(out fl, out length);
            ModuleMetadata moduleMetadata = ModuleMetadata.CreateFromMetadata((IntPtr)fl, length);
            AssemblyMetadata assemblyMetadata = AssemblyMetadata.Create(moduleMetadata);
            return assemblyMetadata.GetReference();
        }
    }
}