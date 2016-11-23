using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlubuCore.Context;
using FlubuCore.Tasks.Process;

namespace FlubuCore.Tasks.Linux
{
    public class SystemCtlTask : TaskBase<int>
    {
        private readonly string _command;
        private readonly string _service;

        public SystemCtlTask(string command, string service)
        {
            _command = command;
            _service = service;
        }

        protected override int DoExecute(ITaskContext context)
        {
            IRunProgramTask task = context.Tasks().RunProgramTask("systemctl")
                .WithArguments(_command, _service);

            return task.Execute(context);
        }
    }
}
