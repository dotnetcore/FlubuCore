using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FlubuCore.Context;

namespace FlubuCore.Tasks
{
    public class DoTaskAsync : DoTaskBase<int, DoTask>
    {
        private readonly Func<ITaskContextInternal, Task> _taskAction;

        public DoTaskAsync(Func<ITaskContextInternal, Task> taskAction)
        {
            _taskAction = taskAction;
        }

        protected override string Description { get; set; }

        protected override int DoExecute(ITaskContextInternal context)
        {
            throw new NotSupportedException("Synchronus method not supported in DoTaskAsync. Use DoTask instead.");
        }

        protected override async Task<int> DoExecuteAsync(ITaskContextInternal context)
        {
            await _taskAction(context);
            return 0;
        }
    }

    public class DoTaskAsync2<T> : DoTaskBase<int, DoTaskAsync2<T>>
    {
        private readonly Func<ITaskContextInternal, T, Task> _taskAction;

        private readonly T _param;

        public DoTaskAsync2(Func<ITaskContextInternal, T, Task> taskAction, T param)
        {
            _taskAction = taskAction;
            _param = param;
        }

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
            throw new NotSupportedException("Synchronus method not supported in DoTaskAsync. Use DoTask instead.");
        }

        protected override async Task<int> DoExecuteAsync(ITaskContextInternal context)
        {
            await _taskAction(context, _param);
            return 0;
        }
    }

    public class DoTaskAsync3<T, T2> : DoTaskBase<int, DoTaskAsync3<T, T2>>
    {
        private readonly Func<ITaskContextInternal, T, T2, Task> _taskAction;

        public DoTaskAsync3(Func<ITaskContextInternal, T, T2, Task> taskAction, T param, T2 param2)
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
            throw new NotSupportedException("Synchronus method not supported in DoTaskAsync. Use DoTask instead.");
        }

        protected override async Task<int> DoExecuteAsync(ITaskContextInternal context)
        {
            await _taskAction(context, Param, Param2);
            return 0;
        }
    }

    public class DoTaskAsync4<T, T2, T3> : DoTaskBase<int, DoTaskAsync4<T, T2, T3>>
    {
        private readonly Func<ITaskContextInternal, T, T2, T3, Task> _taskAction;

        public DoTaskAsync4(Func<ITaskContextInternal, T, T2, T3, Task> taskAction, T param, T2 param2, T3 param3)
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
            throw new NotSupportedException("Synchronus method not supported in DoTaskAsync. Use DoTask instead.");
        }

        protected override async Task<int> DoExecuteAsync(ITaskContextInternal context)
        {
            await _taskAction(context, Param, Param2, Param3);
            return 0;
        }
    }

    public class DoTaskAsync5<T, T2, T3, T4> : DoTaskBase<int, DoTaskAsync5<T, T2, T3, T4>>
    {
        private readonly Func<ITaskContextInternal, T, T2, T3, T4, Task> _taskAction;

        public DoTaskAsync5(Func<ITaskContextInternal, T, T2, T3, T4, Task> taskAction, T param, T2 param2, T3 param3, T4 param4)
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
            throw new NotSupportedException("Synchronus method not supported in DoTaskAsync. Use DoTask instead.");
        }

        protected override async Task<int> DoExecuteAsync(ITaskContextInternal context)
        {
            await _taskAction(context, Param, Param2, Param3, Param4);
            return 0;
        }
    }

    public class DoTaskAsync6<T, T2, T3, T4, T5> : DoTaskBase<int, DoTaskAsync6<T, T2, T3, T4, T5>>
    {
        private readonly Func<ITaskContextInternal, T, T2, T3, T4, T5, Task> _taskAction;

        public DoTaskAsync6(Func<ITaskContextInternal, T, T2, T3, T4, T5, Task> taskAction, T param, T2 param2, T3 param3, T4 param4, T5 param5)
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
            throw new NotSupportedException("Synchronus method not supported in DoTaskAsync. Use DoTask instead.");
        }

        protected override async Task<int> DoExecuteAsync(ITaskContextInternal context)
        {
            await _taskAction(context, Param, Param2, Param3, Param4, Param5);
            return 0;
        }
    }
}
