using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FlubuCore.Context;

namespace FlubuCore.Tasks
{
    public class DoTask : TaskBase<int, DoTask>
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

    public class DoTask2<T> : TaskBase<int, DoTask2<T>>
    {
        private Action<ITaskContextInternal, T> _taskAction;

        private T _param;

        public DoTask2(Action<ITaskContextInternal, T> taskAction, T param)
        {
            _taskAction = taskAction;
            _param = param;
        }

        protected override int DoExecute(ITaskContextInternal context)
        {
            _taskAction.Invoke(context, _param);
            return 0;
        }
    }

    public class DoTask3<T, T2> : TaskBase<int, DoTask3<T, T2>>
    {
        private Action<ITaskContextInternal, T, T2> _taskAction;

        private T _param;

        private T2 _param2;

        public DoTask3(Action<ITaskContextInternal, T, T2> taskAction, T param, T2 param2)
        {
            _taskAction = taskAction;
            _param = param;
            _param2 = param2;
        }

        protected override int DoExecute(ITaskContextInternal context)
        {
            _taskAction.Invoke(context, _param, _param2);
            return 0;
        }
    }

    public class DoTask4<T, T2, T3> : TaskBase<int, DoTask4<T, T2, T3>>
    {
        private Action<ITaskContextInternal, T, T2, T3> _taskAction;

        private T _param;

        private T2 _param2;

        private T3 _param3;

        public DoTask4(Action<ITaskContextInternal, T, T2, T3> taskAction, T param, T2 param2, T3 param3)
        {
            _taskAction = taskAction;
            _param = param;
            _param2 = param2;
            _param3 = param3;
        }

        protected override int DoExecute(ITaskContextInternal context)
        {
            _taskAction.Invoke(context, _param, _param2, _param3);
            return 0;
        }
    }

    public class DoTask5<T, T2, T3, T4> : TaskBase<int, DoTask5<T, T2, T3, T4>>
    {
        private Action<ITaskContextInternal, T, T2, T3, T4> _taskAction;

        private T _param;

        private T2 _param2;

        private T3 _param3;

        private T4 _param4;

        public DoTask5(Action<ITaskContextInternal, T, T2, T3, T4> taskAction, T param, T2 param2, T3 param3, T4 param4)
        {
            _taskAction = taskAction;
            _param = param;
            _param2 = param2;
            _param3 = param3;
            _param4 = param4;
        }

        protected override int DoExecute(ITaskContextInternal context)
        {
            _taskAction.Invoke(context, _param, _param2, _param3, _param4 );
            return 0;
        }
    }

    public class DoTask6<T, T2, T3, T4, T5> : TaskBase<int, DoTask6<T, T2, T3, T4, T5>>
    {
        private Action<ITaskContextInternal, T, T2, T3, T4, T5> _taskAction;

        private T _param;

        private T2 _param2;

        private T3 _param3;

        private T4 _param4;

        private T5 _param5;

        public DoTask6(Action<ITaskContextInternal, T, T2, T3, T4, T5> taskAction, T param, T2 param2, T3 param3, T4 param4, T5 param5)
        {
            _taskAction = taskAction;
            _param = param;
            _param2 = param2;
            _param3 = param3;
            _param4 = param4;
            _param5 = param5;
        }

        protected override int DoExecute(ITaskContextInternal context)
        {
            _taskAction.Invoke(context, _param, _param2, _param3, _param4, _param5);
            return 0;
        }
    }
}
