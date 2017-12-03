using System;
using System.Collections.Generic;
using System.Text;

namespace FlubuCore.Tasks
{
    public  abstract class DoTaskBase<TResult, TTask> : TaskBase<TResult, TTask> where TTask : class, ITask
    {
        public TTask SetTaskName(string taskName)
        {
            TaskName = taskName;
            return this as TTask;
        }

        internal void SetBaseParameters(string taskName, string taskDescription, int retryTimes, int retryDelay, bool noLog, bool doNotFailOnError)
        {
            if (taskName != null)
            {
               SetTaskName(taskName);
            }

            if (taskDescription != null)
            {
                SetDescription(taskDescription);
            }

            if (retryTimes != 0)
            {
               Retry(retryTimes, retryDelay);
            }

            if (noLog)
            {
                NoLog();
            }

            if (doNotFailOnError)
            {
                DoNotFailOnError();
            }
        }
    }
}
