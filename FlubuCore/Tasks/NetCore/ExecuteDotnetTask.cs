using System;
using System.Collections.Generic;
using System.Text;

namespace FlubuCore.Tasks.NetCore
{
    public class ExecuteDotnetTask : ExecuteDotnetTaskBase<ExecuteDotnetTask>
    {
        private string _description;

        public ExecuteDotnetTask(string command) : base(command)
        {
        }

        public ExecuteDotnetTask(StandardDotnetCommands command) : base(command)
        {
        }

        protected override string Description
        {
            get
            {
                if (string.IsNullOrEmpty(_description))
                {
                    return $"Execute dotnet command '{Command}'";
                }
                return _description;
            }
            set { _description = value; }
        }
    }
}
