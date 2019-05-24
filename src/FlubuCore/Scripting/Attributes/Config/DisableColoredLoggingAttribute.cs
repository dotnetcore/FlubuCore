using System;
using System.Collections.Generic;
using System.Text;

namespace FlubuCore.Scripting.Attributes.Config
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class DisableColoredLoggingAttribute : Attribute
    {
    }
}
