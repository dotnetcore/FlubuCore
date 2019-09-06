namespace FlubuCore.Context.Extensions
{
    public class Git
    {
        private readonly ITaskContext _taskContext;

        public Git(ITaskContext taskContext)
        {
            _taskContext = taskContext;
        }
    }
}
