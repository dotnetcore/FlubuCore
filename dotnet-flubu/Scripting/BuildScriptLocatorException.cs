using System;

namespace Flubu.Scripting
{
    public class BuildScriptLocatorException : Exception
    {
        public BuildScriptLocatorException(string message)
            : base(message)
        {
        }
    }
}