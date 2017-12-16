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
            throw new NotSupportedException("Synchronus method not supported in DoTaskAsync");
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

        protected override int DoExecute(ITaskContextInternal context)
        {
            throw new NotSupportedException("Synchronus method not supported in DoTaskAsync");
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

        private readonly T _param;

        private readonly T2 _param2;

        public DoTaskAsync3(Func<ITaskContextInternal, T, T2, Task> taskAction, T param, T2 param2)
        {
            _taskAction = taskAction;
            _param = param;
            _param2 = param2;
        }

        protected override string Description { get; set; }

        protected override int DoExecute(ITaskContextInternal context)
        {
            throw new NotSupportedException("Synchronus method not supported in DoTaskAsync");
        }

        protected override async Task<int> DoExecuteAsync(ITaskContextInternal context)
        {
            await _taskAction(context, _param, _param2);
            return 0;
        }
    }

    public class DoTaskAsync4<T, T2, T3> : DoTaskBase<int, DoTaskAsync4<T, T2, T3>>
    {
        private readonly Func<ITaskContextInternal, T, T2, T3, Task> _taskAction;

        private readonly T _param;

        private readonly T2 _param2;

        private readonly T3 _param3;

        public DoTaskAsync4(Func<ITaskContextInternal, T, T2, T3, Task> taskAction, T param, T2 param2, T3 param3)
        {
            _taskAction = taskAction;
            _param = param;
            _param2 = param2;
            _param3 = param3;
        }

        protected override string Description { get; set; }

        protected override int DoExecute(ITaskContextInternal context)
        {
            throw new NotSupportedException("Synchronus method not supported in DoTaskAsync");
        }

        protected override async Task<int> DoExecuteAsync(ITaskContextInternal context)
        {
            await _taskAction(context, _param, _param2, _param3);
            return 0;
        }
    }

    public class DoTaskAsync5<T, T2, T3, T4> : DoTaskBase<int, DoTaskAsync5<T, T2, T3, T4>>
    {
        private readonly Func<ITaskContextInternal, T, T2, T3, T4, Task> _taskAction;

        private readonly T _param;

        private readonly T2 _param2;

        private readonly T3 _param3;

        private readonly T4 _param4;

        public DoTaskAsync5(Func<ITaskContextInternal, T, T2, T3, T4, Task> taskAction, T param, T2 param2, T3 param3, T4 param4)
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
            throw new NotSupportedException("Synchronus method not supported in DoTaskAsync");
        }

        protected override async Task<int> DoExecuteAsync(ITaskContextInternal context)
        {
            await _taskAction(context, _param, _param2, _param3, _param4);
            return 0;
        }
    }

    public class DoTaskAsync6<T, T2, T3, T4, T5> : DoTaskBase<int, DoTaskAsync6<T, T2, T3, T4, T5>>
    {
        private readonly Func<ITaskContextInternal, T, T2, T3, T4, T5, Task> _taskAction;

        private readonly T _param;

        private readonly T2 _param2;

        private readonly T3 _param3;

        private readonly T4 _param4;

        private readonly T5 _param5;

        public DoTaskAsync6(Func<ITaskContextInternal, T, T2, T3, T4, T5, Task> taskAction, T param, T2 param2, T3 param3, T4 param4, T5 param5)
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
            throw new NotSupportedException("Synchronus method not supported in DoTaskAsync");
        }

        protected override async Task<int> DoExecuteAsync(ITaskContextInternal context)
        {
            await _taskAction(context, _param, _param2, _param3, _param4, _param5);
            return 0;
        }
    }
}
