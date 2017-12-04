using System;
using System.Collections.Generic;
using System.Text;

namespace FlubuCore.Tasks
{
    public  abstract class DoTaskBase<TResult, TTask> : TaskBase<TResult, TTask> where TTask : class, ITask
    {
        /// <summary>
        /// Name of the task that is displayed in help.
        /// </summary>
        /// <param name="taskName"></param>
        /// <returns></returns>
        public TTask SetTaskName(string taskName)
        {
            TaskName = taskName;
            return this as TTask;
        }

        /// <summary>
        /// Adds help for specified argument.
        /// </summary>
        /// <param name="argKey"></param>
        /// <param name="help"></param>
        /// <returns></returns>
        public TTask AddArgumentHelp(string argKey, string help)
        {
            ArgumentHelp.Add(argKey, help);
            return this as TTask;
        }
    }
}
