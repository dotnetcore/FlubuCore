using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
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

        private void WithArgumentsKeyFromAttribute(string value, [CallerMemberName] string memberName = "")
        {
            var method = GetType().GetRuntimeMethods().FirstOrDefault(x => x.Name == memberName);
            if (method == null) return;

            var attribute = method.GetCustomAttribute<ArgKey>();
            WithArguments(attribute.Keys[0], value);
        }
    }
}