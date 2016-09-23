using System.Collections.Generic;

namespace Flubu.Tasks
{
    public static class TaskContextExtensions
    {
        public static void Fail(this ITaskContext context, string message)
        {
            if (message == null)
            {
                return;
            }

            context.Fail(message);
        }

        public static void WriteMessage(this ITaskContext context, string message)
        {
            if (message == null)
            {
                return;
            }

            context.WriteMessage(message);
        }

        public static string ListToString<T>(this IList<T> list)
        {
            return string.Join(",", list);
        }
    }
}