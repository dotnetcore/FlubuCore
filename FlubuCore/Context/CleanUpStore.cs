using System;
using System.Collections.Generic;
using System.Text;

namespace FlubuCore.Context
{
    public static class CleanUpStore
    {
        private static readonly List<Action<ITaskContext>> _taskCleanUpActions = new List<Action<ITaskContext>>();

        private static object _lockObj = new object();

        public static bool StoreAccessed { get; private set; }

        public static List<Action<ITaskContext>> TaskCleanUpActions
        {
            get
            {
                lock (_lockObj)
                {
                    StoreAccessed = true;
                    return _taskCleanUpActions;
                }
            }
        }

        public static void AddCleanupAction(Action<ITaskContext> action)
        {
            if (!StoreAccessed)
            {
                lock (_lockObj)
                {
                    _taskCleanUpActions.Add(action);
                }
            }
        }

        public static void RemoveCleanupAction(Action<ITaskContext> action)
        {
            if (!StoreAccessed)
            {
                lock (_lockObj)
                {
                    _taskCleanUpActions.Remove(action);
                }
            }
        }
    }
}
