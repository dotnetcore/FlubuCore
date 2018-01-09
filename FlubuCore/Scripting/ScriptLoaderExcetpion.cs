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
    }
}
