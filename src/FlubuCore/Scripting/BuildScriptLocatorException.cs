using System;

namespace FlubuCore.Scripting
{
    public class BuildScriptLocatorException : FlubuException
    {
        public BuildScriptLocatorException(string message)
            : base(message)
        {
        }
    }
}