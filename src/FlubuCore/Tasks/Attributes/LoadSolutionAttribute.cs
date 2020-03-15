using System;
using System.Collections.Generic;
using System.Text;
using FlubuCore.Tasks.Solution;

namespace FlubuCore.Tasks.Attributes
{
    /// <summary>
    /// Executes <see cref="LoadSolutionTask"/> and injects Build version into property.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class LoadSolutionAttribute : Attribute
    {
        public string SolutionName { get; set; }
    }
}
