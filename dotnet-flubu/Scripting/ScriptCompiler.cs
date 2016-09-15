using Microsoft.CodeAnalysis.CSharp.Scripting;

namespace flubu.Scripting
{
    public class ScriptCompiler
    {
        public void Compile()
        {

            CSharpScript.EvaluateAsync(@"using System;Console.WriteLine(""Hello Roslyn."");").Wait();
        }
    }
}