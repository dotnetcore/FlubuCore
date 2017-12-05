using FlubuCore.Context;
using FlubuCore.Tasks.Process;

namespace FlubuCore.Tasks.Linux
{
    public class SystemCtlTask : TaskBase<int, SystemCtlTask>
    {
        private readonly string _command;
        private readonly string _service;

        public SystemCtlTask(string command, string service)
        {
            _command = command;
            _service = service;
        }

        protected override string Description { get; set; }

        protected override int DoExecute(ITaskContextInternal context)
        {
            IRunProgramTask task = context
                .Tasks()
                .RunProgramTask("systemctl")
                .WithArguments(_command, _service);

            if (DoNotFail)
                task.DoNotFailOnError();

            return task.Execute(context);
        }
    }
}
