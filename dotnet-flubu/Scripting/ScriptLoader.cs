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
        private readonly IFileLoader fileLoader;

        public ScriptLoader(IFileLoader fileLoader)
        {
            this.fileLoader = fileLoader;
        }

        public async Task<IBuildScript> FindAndCreateBuildScriptInstance(string fileName)
        {
            var code = fileLoader.LoadFile(fileName);

            var opts = ScriptOptions.Default
                .WithReferences(LoadAssembly<object>())
                .WithReferences(LoadAssembly<DefaultBuildScript>())
                .WithReferences(LoadAssembly<IBuildScript>());
            //// .AddReferences("d:\\flubu.buildscript.dll")
            //// .WithImports("flubu.buildscript");

            Script script = CSharpScript
                //// .Create(@"var sc = new MyBuildScript();", opts);
                .Create(code, opts)
                .ContinueWith("var sc = new MyBuildScript();");

            var result = await script.RunAsync();

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