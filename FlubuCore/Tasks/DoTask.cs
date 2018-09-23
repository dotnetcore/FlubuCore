using System;
using System.Threading.Tasks;
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

        protected internal override string TaskName
        {
            get
            {
                if (_taskAction == null)
                {
                    return base.TaskName;
                }
#if !NETSTANDARD1_6
                return _taskAction?.Method.Name;
#endif
                return base.TaskName;
            }

            set => base.TaskName = value;
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

        protected internal override string TaskName
        {
            get
            {
                if (_taskAction == null)
                {
                    return base.TaskName;
                }
#if !NETSTANDARD1_6
                return _taskAction?.Method.Name;
#endif
                return base.TaskName;
            }

            set => base.TaskName = value;
        }

        protected override int DoExecute(ITaskContextInternal context)
        {
            _taskAction.Invoke(context, Param);
            return 0;
        }
    }

    public class DoTask3<T, T2> : DoTaskBase<int, DoTask3<T, T2>>
    {
        private readonly Action<ITaskContextInternal, T, T2> _taskAction;

        public DoTask3(Action<ITaskContextInternal, T, T2> taskAction, T param, T2 param2)
        {
            _taskAction = taskAction;
            Param = param;
            Param2 = param2;
        }

        public T Param { get; set; }

        public T2 Param2 { get; set; }

        protected override string Description { get; set; }

        protected internal override string TaskName
        {
            get
            {
                if (_taskAction == null)
                {
                    return base.TaskName;
                }
#if !NETSTANDARD1_6
                return _taskAction?.Method.Name;
#endif
                return base.TaskName;
            }

            set => base.TaskName = value;
        }

        protected override int DoExecute(ITaskContextInternal context)
        {
            _taskAction.Invoke(context, Param, Param2);
            return 0;
        }
    }

    public class DoTask4<T, T2, T3> : DoTaskBase<int, DoTask4<T, T2, T3>>
    {
        private readonly Action<ITaskContextInternal, T, T2, T3> _taskAction;

        public DoTask4(Action<ITaskContextInternal, T, T2, T3> taskAction, T param, T2 param2, T3 param3)
        {
            _taskAction = taskAction;
            Param = param;
            Param2 = param2;
            Param3 = param3;
        }

        public T Param { get; set; }

        public T2 Param2 { get; set; }

        public T3 Param3 { get; set; }

        protected override string Description { get; set; }

        protected internal override string TaskName
        {
            get
            {
                if (_taskAction == null)
                {
                    return base.TaskName;
                }
#if !NETSTANDARD1_6
                return _taskAction?.Method.Name;
#endif
                return base.TaskName;
            }

            set => base.TaskName = value;
        }

        protected override int DoExecute(ITaskContextInternal context)
        {
            _taskAction.Invoke(context, Param, Param2, Param3);
            return 0;
        }
    }

    public class DoTask5<T, T2, T3, T4> : DoTaskBase<int, DoTask5<T, T2, T3, T4>>
    {
        private readonly Action<ITaskContextInternal, T, T2, T3, T4> _taskAction;

        public DoTask5(Action<ITaskContextInternal, T, T2, T3, T4> taskAction, T param, T2 param2, T3 param3, T4 param4)
        {
            _taskAction = taskAction;
            Param = param;
            Param2 = param2;
            Param3 = param3;
            Param4 = param4;
        }

        public T Param { get; set; }

        public T2 Param2 { get; set; }

        public T3 Param3 { get; set; }

        public T4 Param4 { get; set; }

        protected override string Description { get; set; }

        protected internal override string TaskName
        {
            get
            {
                if (_taskAction == null)
                {
                    return base.TaskName;
                }
#if !NETSTANDARD1_6
                return _taskAction?.Method.Name;
#endif
                return base.TaskName;
            }

            set => base.TaskName = value;
        }

        protected override int DoExecute(ITaskContextInternal context)
        {
            _taskAction.Invoke(context, Param, Param2, Param3, Param4);
            return 0;
        }
    }

    public class DoTask6<T, T2, T3, T4, T5> : DoTaskBase<int, DoTask6<T, T2, T3, T4, T5>>
    {
        private readonly Action<ITaskContextInternal, T, T2, T3, T4, T5> _taskAction;

        public DoTask6(Action<ITaskContextInternal, T, T2, T3, T4, T5> taskAction, T param, T2 param2, T3 param3, T4 param4, T5 param5)
        {
            _taskAction = taskAction;
            Param = param;
            Param2 = param2;
            Param3 = param3;
            Param4 = param4;
            Param5 = param5;
        }

        public T Param { get; set; }

        public T2 Param2 { get; set; }

        public T3 Param3 { get; set; }

        public T4 Param4 { get; set; }

        public T5 Param5 { get; set; }

        protected override string Description { get; set; }

        protected internal override string TaskName
        {
            get
            {
                if (_taskAction == null)
                {
                    return base.TaskName;
                }
#if !NETSTANDARD1_6
                return _taskAction?.Method.Name;
#endif
                return base.TaskName;
            }

            set => base.TaskName = value;
        }

        protected override int DoExecute(ITaskContextInternal context)
        {
            _taskAction.Invoke(context, Param, Param2, Param3, Param4, Param5);
            return 0;
        }
    }
}
