using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Emit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace flubu
{
    public static class Compiler
    {
        public static void Compile()
        {

            CSharpScript.EvaluateAsync(@"using System;Console.WriteLine(""Hello Roslyn."");").Wait();
        }
    }
}