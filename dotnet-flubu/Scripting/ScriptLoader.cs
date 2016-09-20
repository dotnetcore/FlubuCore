using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using System;
using System.Reflection;
using System.Reflection.Metadata;
using System.Threading.Tasks;

namespace flubu.Scripting
{
    public interface IScriptLoader
    {
        Task<IBuildScript> FindAndCreateBuildScriptInstance(string fileName);
    }

    public class ScriptLoader : IScriptLoader
    {
        private readonly IFileLoader _fileLoader;

        public ScriptLoader(IFileLoader fileLoader)
        {
            _fileLoader = fileLoader;
        }

        public async Task<IBuildScript> FindAndCreateBuildScriptInstance(string fileName)
        {
            string code = _fileLoader.LoadFile(fileName);

            ScriptOptions opts = ScriptOptions.Default
                .WithReferences(LoadAssembly<System.Object>())
                .WithReferences(LoadAssembly<DefaultBuildScript>())
                .WithReferences(LoadAssembly<IBuildScript>())
                .AddReferences("d:\\flubu.buildscript.dll")
                .WithImports("flubu.buildscript");



            //todo how to find class name??
            Script script = CSharpScript
                .Create(@"var sc = new flubu.buildscript.MyBuildScript();", opts);
                //.Create(code, opts, globalsType: typeof(Globals))
                //.ContinueWith("Script.Run(null)");

            ScriptState result = await script.RunAsync();

            return result.Variables[0].Value as IBuildScript;
        }

        private static unsafe PortableExecutableReference LoadAssembly<T>()
        {
            byte* fl;
            int length;
            Assembly assembly = typeof(T).GetTypeInfo().Assembly; //let's grab the current in-memory assembly
            assembly.TryGetRawMetadata(out fl, out length);
            ModuleMetadata moduleMetadata = ModuleMetadata.CreateFromMetadata((IntPtr)fl, length);
            AssemblyMetadata assemblyMetadata = AssemblyMetadata.Create(moduleMetadata);
            return assemblyMetadata.GetReference();
        }
    }
}