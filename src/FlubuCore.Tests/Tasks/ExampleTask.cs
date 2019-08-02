using System;
using System.Collections.Generic;
using System.Text;
using FlubuCore.Tasks.Process;

namespace FlubuCore.Tests.Tasks
{
    public class ExampleTask : ExternalProcessTaskBase<int, ExampleTask>
    {
        private static List<string> _overriadbleArguments = new List<string>()
        {
            "-c",
            "--configuration",
            "-r",
            "--runtime",
        };

        protected override string Description { get; set; }

        protected internal override List<string> OverridableArguments => _overriadbleArguments;

        public ExampleTask Configuration(string value)
        {
            WithArguments("--configuration", value);
            return this;
        }

        public ExampleTask Runtime(string value)
        {
            WithArguments("--runtime", value);
            return this;
        }
    }
}
