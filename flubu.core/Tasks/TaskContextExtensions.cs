namespace flubu
{
    public static class TaskContextExtensions
    {
        public static void Fail(this ITaskContext context, string message)
        {
            if (message == null)
                return;

            context.Fail(message);
        }

        public static void WriteMessage(this ITaskContext context, string message)
        {
            if (message == null)
                return;

            context.WriteMessage(message);
        }
    }
}