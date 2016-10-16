using System;

namespace DotNet.Cli.Flubu.Scripting
{
    public class BuildScriptLocatorException : Exception
    {
        public BuildScriptLocatorException(string message)
            : base(message)
        {
        }
    }
}