using System;
using System.Collections.Generic;
using System.Text;

namespace FlubuCore.Targeting
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class TargetAttribute : Attribute
    {
        public TargetAttribute(string targetName, params object[] methodParameters)
        {
            TargetName = targetName;
            MethodParameters = methodParameters;
        }

        public string TargetName { get; private set; }

        public object[] MethodParameters { get; set; }
    }
}
