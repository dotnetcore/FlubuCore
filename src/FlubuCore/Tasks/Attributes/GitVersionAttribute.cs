using System;
using System.Collections.Generic;
using System.Text;
using FlubuCore.Tasks.Versioning;

namespace FlubuCore.Tasks.Attributes
{
    /// <summary>
    /// Executes <see cref="GitVersionTask"/> and injects Build version into property.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class GitVersionAttribute : Attribute
    {
        public GitVersionAttribute()
        {
        }
    }
}
