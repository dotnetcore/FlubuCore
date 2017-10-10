using System;
using System.Collections.Generic;
using System.Text;

namespace FlubuCore.Tasks.NetCore
{
    public class ExecuteDotnetTask : ExecuteDotnetTaskBase<ExecuteDotnetTask>
    {
        public ExecuteDotnetTask(string command) : base(command)
        {
        }

        public ExecuteDotnetTask(StandardDotnetCommands command) : base(command)
        {
        }
    }
}
