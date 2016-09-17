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
            Console.WriteLine("0");

            string code = _fileLoader.LoadFile(fileName);

            Console.WriteLine("1");
            ScriptOptions opts = ScriptOptions.Default
                .AddImports("flubu.Scripting", "System", "System.Collections.Generic", "flubu.Targeting",
                "flubu")
                .AddReferences(LoadAssembly<IBuildScript>(),
                LoadAssembly<ScriptLoader>(),
                LoadAssembly<DefaultBuildScript>(),
                LoadAssembly<Script>());
            Console.WriteLine("2");

            //todo how to find class name??
            Script<IBuildScript> script = CSharpScript.Create<IBuildScript>($"{code} \r\nvar obj = new MyBuildScript();", opts);
            Console.WriteLine("3");

            ScriptState<IBuildScript> result = await script.RunAsync();
            Console.WriteLine("4");

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