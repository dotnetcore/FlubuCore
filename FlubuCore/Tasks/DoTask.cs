using System;
using FlubuCore.Context;

namespace FlubuCore.Tasks
{
    public class DoTask : DoTaskBase<int, DoTask>
    {
        private readonly Action<ITaskContextInternal> _taskAction;

        public DoTask(Action<ITaskContextInternal> taskAction)
        {
            _taskAction = taskAction;
        }

        protected override string Description { get; set; }

        protected override int DoExecute(ITaskContextInternal context)
        {
            _taskAction.Invoke(context);
            return 0;
        }
    }

    public class DoTask2<T> : DoTaskBase<int, DoTask2<T>>
    {
        private readonly Action<ITaskContextInternal, T> _taskAction;

        public DoTask2(Action<ITaskContextInternal, T> taskAction, T param)
        {
            _taskAction = taskAction;
            Param = param;
        }

        public T Param { get; set; }

        protected override string Description { get; set; }

        protected override int DoExecute(ITaskContextInternal context)
        {
            _taskAction.Invoke(context, Param);
            return 0;
        }
    }

    public class DoTask3<T, T2> : DoTaskBase<int, DoTask3<T, T2>>
    {
        private readonly Action<ITaskContextInternal, T, T2> _taskAction;

        private readonly T _param;

        private readonly T2 _param2;

        public DoTask3(Action<ITaskContextInternal, T, T2> taskAction, T param, T2 param2)
        {
            _taskAction = taskAction;
            _param = param;
            _param2 = param2;
        }

        protected override string Description { get; set; }

        protected override int DoExecute(ITaskContextInternal context)
        {
            _taskAction.Invoke(context, _param, _param2);
            return 0;
        }
    }

    public class DoTask4<T, T2, T3> : DoTaskBase<int, DoTask4<T, T2, T3>>
    {
        private readonly Action<ITaskContextInternal, T, T2, T3> _taskAction;

        private readonly T _param;

        private readonly T2 _param2;

        private readonly T3 _param3;

        public DoTask4(Action<ITaskContextInternal, T, T2, T3> taskAction, T param, T2 param2, T3 param3)
        {
            _taskAction = taskAction;
            _param = param;
            _param2 = param2;
            _param3 = param3;
        }

        protected override string Description { get; set; }

        protected override int DoExecute(ITaskContextInternal context)
        {
            _taskAction.Invoke(context, _param, _param2, _param3);
            return 0;
        }
    }

    public class DoTask5<T, T2, T3, T4> : DoTaskBase<int, DoTask5<T, T2, T3, T4>>
    {
        private readonly Action<ITaskContextInternal, T, T2, T3, T4> _taskAction;

        private readonly T _param;

        private readonly T2 _param2;

        private readonly T3 _param3;

        private readonly T4 _param4;

        public DoTask5(Action<ITaskContextInternal, T, T2, T3, T4> taskAction, T param, T2 param2, T3 param3, T4 param4)
        {
            _taskAction = taskAction;
            _param = param;
            _param2 = param2;
            _param3 = param3;
            _param4 = param4;
        }

        protected override string Description { get; set; }

        protected override int DoExecute(ITaskContextInternal context)
        {
            _taskAction.Invoke(context, _param, _param2, _param3, _param4);
            return 0;
        }
    }

    public class DoTask6<T, T2, T3, T4, T5> : DoTaskBase<int, DoTask6<T, T2, T3, T4, T5>>
    {
        private readonly Action<ITaskContextInternal, T, T2, T3, T4, T5> _taskAction;

        private readonly T _param;

        private readonly T2 _param2;

        private readonly T3 _param3;

        private readonly T4 _param4;

        private readonly T5 _param5;

        public DoTask6(Action<ITaskContextInternal, T, T2, T3, T4, T5> taskAction, T param, T2 param2, T3 param3, T4 param4, T5 param5)
        {
            _taskAction = taskAction;
            _param = param;
            _param2 = param2;
            _param3 = param3;
            _param4 = param4;
            _param5 = param5;
        }

        protected override string Description { get; set; }

        protected override int DoExecute(ITaskContextInternal context)
        {
            _taskAction.Invoke(context, _param, _param2, _param3, _param4, _param5);
            return 0;
        }
    }
}
