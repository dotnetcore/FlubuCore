using System;
using System.Collections.Generic;
using System.Text;
using FlubuCore.Context;
using FlubuCore.Tasks;

namespace FlubuCore.Targeting
{
    public class TaskGroup
    {
        public TaskGroup()
        {
            Tasks = new List<(ITask task, TaskExecutionMode taskExecutionMode)>();
        }

        public string GroupId { get; set; }

        public List<(ITask task, TaskExecutionMode taskExecutionMode)> Tasks { get; set; }

        public Action<ITaskContext> FinallyAction { get; set; }

        public bool CleanupOnCancel { get; set; }

        public Action<ITaskContext, Exception> OnErrorAction { get; set; }
    }
}
