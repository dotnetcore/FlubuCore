using flubu.Scripting;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Metadata;
using Xunit;
using flubu.Targeting;

namespace flubu.tests.Scripting
{
    public class CompileTests
    {
        [Fact]
        public void AnotherTest()
        {
            var opts = ScriptOptions.Default
                .AddImports("flubu.Scripting.ExampleScript", "System",
                "System.Collections.Generic", "flubu.Targeting", "flubu.tests.Scripting", "flubu.Scripting")
                .AddReferences(LoadAssembly<IBuildScript>(), LoadAssembly<CompileTests>());

            Script<IBuildScript> script = CSharpScript.Create<IBuildScript>(@"
        public class MyBuildScript : IBuildScript
    {
        public int Run(string[] args)
        {
            Console.WriteLine(""1"");


return 0;
        }
    }


var foo = new MyBuildScript();", opts);

            ScriptState<IBuildScript> result = script.RunAsync().Result; //runs fine, possible to use main application types in the script
            var s = result.Variables[0].Value;
            //Assert.IsType<MyBuildScript>(s);
            //s.Run(new List<string>());
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
