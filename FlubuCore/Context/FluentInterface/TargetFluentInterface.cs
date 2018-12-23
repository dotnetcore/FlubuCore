using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlubuCore.Context.FluentInterface.Interfaces;
using FlubuCore.Context.FluentInterface.TaskExtensions;
using FlubuCore.Targeting;
using FlubuCore.Tasks;

namespace FlubuCore.Context.FluentInterface
{
    public class TargetFluentInterface : TargetBaseFluentInterface<ITarget>, ITarget
    {
        public ITarget DependsOn(params string[] targetNames)
        {
            LastTargetAction = TargetAction.AddDependency;
            ActionCount = targetNames.Length;
            Target.DependsOn(targetNames);
            return this;
        }

        public ITarget DependsOn(params ITargetInternal[] targets)
        {
            LastTargetAction = TargetAction.AddDependency;
            ActionCount = targets.Length;
            Target.DependsOn(targets);
            return this;
        }

        public ITarget DependsOn(params ITarget[] targets)
        {
            LastTargetAction = TargetAction.AddDependency;
            ActionCount = targets.Length;
            foreach (var t in targets)
            {
                var target = (TargetFluentInterface)t;
                Target.DependsOn(target.Target);
            }

            return this;
        }

        public ITarget DependsOnAsync(params ITargetInternal[] targets)
        {
            LastTargetAction = TargetAction.AddDependency;
            ActionCount = targets.Length;
            Target.DependsOnAsync(targets);
            return this;
        }

        public ITarget DependsOnAsync(params ITarget[] targets)
        {
            LastTargetAction = TargetAction.AddDependency;
            ActionCount = targets.Length;
            foreach (var t in targets)
            {
                var target = (TargetFluentInterface)t;
                Target.DependsOnAsync(target.Target);
            }

            return this;
        }

        public ITarget SetAsDefault()
        {
            LastTargetAction = TargetAction.Other;
            ActionCount = 0;
            Target.SetAsDefault();
            return this;
        }

        public ITarget SetDescription(string description)
        {
            LastTargetAction = TargetAction.Other;
            ActionCount = 0;
            Target.SetDescription(description);
            return this;
        }

        public ITarget SetAsHidden()
        {
            LastTargetAction = TargetAction.Other;
            ActionCount = 0;
            Target.SetAsHidden();
            return this;
        }

        public ITarget Group(Action<ITargetBaseFluentInterfaceOfT<ITarget>> targetAction, Action<ITaskContext> onFinally = null, Action<ITaskContext, Exception> onError = null, Func<ITaskContext, bool> when = null, bool cleanupOnCancel = false)
        {
            LastTargetAction = TargetAction.Other;
            ActionCount = 0;
            TaskGroup = new TaskGroup
            {
                GroupId = Guid.NewGuid().ToString(),
                OnErrorAction = onError,
                FinallyAction = onFinally,
                CleanupOnCancel = cleanupOnCancel
            };

            var conditionMeet = when?.Invoke(Context);
            if (conditionMeet.HasValue == false || conditionMeet.Value)
            {
                targetAction.Invoke(this);
            }

            TaskGroup = null;
            return this;
        }

        public ITarget AddTasks(Action<ITarget> action)
        {
            action?.Invoke(this);
            return this;
        }

        public ITarget AddTasks<T>(Action<ITarget, T> action, T param)
        {
            action?.Invoke(this, param);
            return this;
        }

        public ITarget AddTasks<T, T2>(Action<ITarget, T, T2> action, T param, T2 param2)
        {
            action?.Invoke(this, param, param2);
            return this;
        }

        public ITarget AddTasks<T, T2, T3>(Action<ITarget, T, T2, T3> action, T param,
            T2 param2, T3 param3)
        {
            action?.Invoke(this, param, param2, param3);
            return this;
        }

        public ITarget AddTasks<T, T2, T3, T4>(Action<ITarget, T, T2, T3, T4> action, T param,
            T2 param2, T3 param3, T4 param4)
        {
            action?.Invoke(this, param, param2, param3, param4);
            return this;
        }

        public ITarget AddTasks<T, T2, T3, T4, T5>(Action<ITarget, T, T2, T3, T4, T5> action,
            T param, T2 param2, T3 param3, T4 param4, T5 param5)
        {
            action?.Invoke(this, param, param2, param3, param4, param5);
            return this;
        }

        public ITarget AddTasks<T, T2, T3, T4, T5, T6>(Action<ITarget, T, T2, T3, T4, T5, T6> action, T param, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6)
        {
            action?.Invoke(this, param, param2, param3, param4, param5, param6);
            return this;
        }

        public ITarget AddTasks<T, T2, T3, T4, T5, T6, T7>(
            Action<ITarget, T, T2, T3, T4, T5, T6, T7> action, T param, T2 param2, T3 param3, T4 param4,
            T5 param5,
            T6 param6, T7 param7)
        {
            action?.Invoke(this, param, param2, param3, param4, param5, param6, param7);
            return this;
        }

        public ITarget AddTasks<T, T2, T3, T4, T5, T6, T7, T8>(
            Action<ITarget, T, T2, T3, T4, T5, T6, T7, T8> action, T param, T2 param2, T3 param3,
            T4 param4, T5 param5,
            T6 param6, T7 param7, T8 param8)
        {
            action?.Invoke(this, param, param2, param3, param4, param5, param6, param7, param8);
            return this;
        }
    }
}
