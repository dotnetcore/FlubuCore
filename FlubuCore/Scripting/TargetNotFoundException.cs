using System;
using System.Collections.Generic;
using System.Text;

namespace FlubuCore.Scripting
{
    public class TargetNotFoundException : Exception
    {
        public TargetNotFoundException(string message) : base(message)
        {
        }
    }
}
