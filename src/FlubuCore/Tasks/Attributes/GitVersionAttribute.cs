using System;
using System.Collections.Generic;
using System.Text;
using FlubuCore.Tasks.Versioning;

namespace FlubuCore.Tasks.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class GitVersionAttribute : Attribute
    {
        public GitVersionAttribute(Action<GitVersionTask> options = null)
        {
            Options = options;
        }

        public Action<GitVersionTask> Options { get; }
    }
}
