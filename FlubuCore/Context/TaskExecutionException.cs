namespace FlubuCore.Context
{
    public class TaskExecutionException : FlubuException
    {
        public TaskExecutionException(string message, int errorCode)
            : base(message)
        {
            ErrorCode = errorCode;
        }

        public int ErrorCode { get; }
    }
}