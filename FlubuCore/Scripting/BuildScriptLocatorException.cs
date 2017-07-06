using System;

namespace FlubuCore.Scripting
{
    public class BuildScriptLocatorException : Exception
    {
        public BuildScriptLocatorException(string message)
            : base(message)
        {
        }
    }
}