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
    }
}
