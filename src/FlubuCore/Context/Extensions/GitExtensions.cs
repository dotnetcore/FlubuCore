using FlubuCore.Tasks;

namespace FlubuCore.Context
{
    public class Git
    {
        private readonly ITaskContext _taskContext;

        public Git(ITaskContext taskContext)
        {
            _taskContext = taskContext;
        }

        /// <summary>
        /// Name of the current selected branch.
        /// </summary>
        /// <returns></returns>
        public string CurrentBranchName()
        {
            var task = _taskContext.Tasks().RunProgramTask("git");

            int resultCode = task.WithArguments("rev-parse", "--abbrev-ref", "HEAD")
                .CaptureOutput()
                .WithLogLevel(LogLevel.None)
                .DoNotLogTaskExecutionInfo()
                .Execute(_taskContext);

            if (resultCode == 0)
            {
                return task.GetOutput();
            }

            return null;
        }

        /// <summary>
        ///  root directory of the current git repository.
        /// </summary>
        /// <returns></returns>
        public string RootDirectory()
        {
            var task = _taskContext.Tasks().RunProgramTask("git");

            int resultCode = task.WithArguments("rev-parse", "--show-toplevel")
                .CaptureOutput()
                .WithLogLevel(LogLevel.None)
                .DoNotLogTaskExecutionInfo()
                .Execute(_taskContext);

            if (resultCode == 0)
            {
                return task.GetOutput();
            }

            return null;
        }

        /// <summary>
        /// Remote url of the git repository.
        /// </summary>
        /// <returns></returns>
        public string RemoteUrl()
        {
            var task = _taskContext.Tasks().RunProgramTask("git");

            int resultCode = task.WithArguments("config", "--get", "remote.origin.url")
                .CaptureOutput()
                .WithLogLevel(LogLevel.None)
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
                .WithLogLevel(LogLevel.None)
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
