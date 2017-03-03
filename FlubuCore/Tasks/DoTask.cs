using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FlubuCore.Context;

namespace FlubuCore.Tasks
{
    public class DoTask : TaskBase<int>
    {
        private Action<ITaskContextInternal> _taskAction;

        public DoTask(Action<ITaskContextInternal> taskAction)
        {
            _taskAction = taskAction;
        }

        protected override int DoExecute(ITaskContextInternal context)
        {
            _taskAction.Invoke(context);
            return 0;
        }
    }
}
