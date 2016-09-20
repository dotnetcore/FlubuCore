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
                .AddImports("flubu.Scripting", "System", "System.Collections.Generic", "flubu.Targeting",
                "flubu", "System.Object")
                //.AddReferences("d:\\System.Runtime.dll")
                .AddReferences(LoadAssembly<IBuildScript>());

            //todo how to find class name??
            Script<IBuildScript> script = CSharpScript.Create<IBuildScript>($"{code} \r\nvar obj = new MyBuildScript();", opts);

            ScriptState<IBuildScript> result = await script.RunAsync();

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