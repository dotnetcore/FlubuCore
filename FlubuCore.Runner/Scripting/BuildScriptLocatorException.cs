using System;

namespace FlubuCore.Runner.Scripting
{
    public class BuildScriptLocatorException : Exception
    {
        public BuildScriptLocatorException(string message)
            : base(message)
        {
        }
    }
}