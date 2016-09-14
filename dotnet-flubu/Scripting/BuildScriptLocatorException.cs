using System;

namespace flubu.Scripting
{
    public class BuildScriptLocatorException : Exception
    {
        public BuildScriptLocatorException(string message)
            : base(message)
        {
        }
    }
}