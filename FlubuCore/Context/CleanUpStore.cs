using System;
using System.Collections.Generic;
using System.Text;

namespace FlubuCore.Context
{
    public static class CleanUpStore
    {
        private static object _lockObj = new object();

        public static List<Action<ITaskContext>> TaskCleanUpActions { get; private set; } = new List<Action<ITaskContext>>();

        public static void AddCleanUpAction(Action<ITaskContext> action)
        {
            lock (_lockObj)
            {
                TaskCleanUpActions.Add(action);
            }
        }

        public static void RemoveCleanUpAction(Action<ITaskContext> action)
        {
            lock (_lockObj)
            {
                TaskCleanUpActions.Remove(action);
            }
        }
    }
}
