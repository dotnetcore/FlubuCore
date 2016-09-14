using Microsoft.CodeAnalysis.CSharp.Scripting;

namespace flubu
{
    public class ScriptCompiler
    {
        public void Compile()
        {

            CSharpScript.EvaluateAsync(@"using System;Console.WriteLine(""Hello Roslyn."");").Wait();
        }
    }
}