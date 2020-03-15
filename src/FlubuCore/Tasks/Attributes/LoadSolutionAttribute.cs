using System;
using System.Collections.Generic;
using System.Text;

namespace FlubuCore.Tasks.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class LoadSolutionAttribute : Attribute
    {
        public string SolutionName { get; set; }
    }
}
