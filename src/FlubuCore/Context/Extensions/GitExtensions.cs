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

        public string RootDirectory()
        {
            var task = _taskContext.Tasks().RunProgramTask("git");

            int resultCode = task.WithArguments("rev-parse", "--show-toplevel")
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

        public string RemoteUrl()
        {
            var task = _taskContext.Tasks().RunProgramTask("git");

            int resultCode = task.WithArguments("config", "--get", "remote.origin.url")
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

        public string CommitId(string branchName = "HEAD")
        {
            var task = _taskContext.Tasks().RunProgramTask("git");

            int resultCode = task.WithArguments("rev-parse", branchName)
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
