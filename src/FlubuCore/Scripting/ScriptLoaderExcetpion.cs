using System;
using System.Collections.Generic;
using System.Text;

namespace FlubuCore.Scripting
{
    public class ScriptLoaderExcetpion : FlubuException
    {
        public ScriptLoaderExcetpion(string message)
            : base(message)
        {
        }

        public ScriptLoaderExcetpion(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
