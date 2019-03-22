using System;
using System.Collections.Generic;
using System.Text;

namespace FlubuCore.Scripting.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ReferenceAttribute : Attribute
    {
        public ReferenceAttribute(string reference)
        {
        }
    }
}
