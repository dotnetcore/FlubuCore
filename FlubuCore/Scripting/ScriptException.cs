using System;
using System.Collections.Generic;
using System.Text;

namespace FlubuCore.Scripting
{
    public class ScriptException : FlubuException
    {
        public ScriptException(string message)
            : base(message)
        {
        }

        public ScriptException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
