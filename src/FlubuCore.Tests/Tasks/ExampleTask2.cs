using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using FlubuCore.Tasks.Attributes;
using FlubuCore.Tasks.Process;

namespace FlubuCore.Tests.Tasks
{
    public class ExampleTask2 : ExternalProcessTaskBase<int, ExampleTask>
    {
        protected override string Description { get; set; }

        [ArgKey("-c", "configuration")]
        public ExampleTask2 Configuration(string value)
        {
            WithArgumentsKeyFromAttribute(value);
            return this;
        }

        [ArgKey("-r", "--runtime")]
        public ExampleTask2 Runtime(string value)
        {
            WithArgumentsKeyFromAttribute(value);
            return this;
        }
    }
}