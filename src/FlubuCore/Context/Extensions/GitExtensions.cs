namespace FlubuCore.Context
{
    public class Git
    {
        private readonly ITaskContext _taskContext;

        public Git(ITaskContext taskContext)
        {
            _taskContext = taskContext;
        }

        public string BranchName()
        {
            var task = _taskContext.Tasks().RunProgramTask("git");

            int resultCode = task.WithArguments("rev-parse", "--abbrev-ref", "HEAD")
                .CaptureOutput()
                .NoLog()
                .DoNotLogTaskExecutionInfo()
                .Execute(_taskContext);

            if (resultCode == 0)
            {
                return task.GetOutput();
            }

            return null;
        }
    }
}
