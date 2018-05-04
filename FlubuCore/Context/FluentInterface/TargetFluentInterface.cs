using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlubuCore.Context.FluentInterface.Interfaces;
using FlubuCore.Context.FluentInterface.TaskExtensions;
using FlubuCore.Context.FluentInterface.TaskExtensions.Core;
using FlubuCore.Targeting;
using FlubuCore.Tasks;

namespace FlubuCore.Context.FluentInterface
{
    public class TargetFluentInterface : TargetBaseFluentInterface<ITargetFluentInterface>, ITargetFluentInterface
    {
        public ITargetFluentInterface DependsOn(params string[] targetNames)
        {
            Target.DependsOn(targetNames);
            return this;
        }

        public ITargetFluentInterface DependsOn(params ITarget[] targets)
        {
            Target.DependsOn(targets);
            return this;
        }

        public ITargetFluentInterface DependsOn(params ITargetFluentInterface[] targets)
        {
            foreach (var t in targets)
            {
                var target = (TargetFluentInterface)t;
                Target.DependsOn(target.Target);
            }

            return this;
        }

        public ITargetFluentInterface DependsOnAsync(params ITarget[] targets)
        {
            Target.DependsOnAsync(targets);
            return this;
        }

        public ITargetFluentInterface DependsOnAsync(params ITargetFluentInterface[] targets)
        {
            foreach (var t in targets)
            {
                var target = (TargetFluentInterface)t;
                Target.DependsOnAsync(target.Target);
            }

            return this;
        }

        public ITargetFluentInterface SetAsDefault()
        {
            Target.SetAsDefault();
            return this;
        }

        public ITargetFluentInterface SetDescription(string description)
        {
            Target.SetDescription(description);
            return this;
        }

        public ITargetFluentInterface SetAsHidden()
        {
            Target.SetAsHidden();
            return this;
        }

        public ITaskExtensionsFluentInterface TaskExtensions()
        {
            TaskExtensionsFluentInterface taskExtensionFluent = (TaskExtensionsFluentInterface)TaskExtensionsFluent;
            taskExtensionFluent.Target = this;
            taskExtensionFluent.Context = Context;
            return TaskExtensionsFluent;
        }

        public ICoreTaskExtensionsFluentInterface CoreTaskExtensions()
        {
            CoreTaskExtensionsFluentInterface coreTaskExtensionsFluent = (CoreTaskExtensionsFluentInterface)CoreTaskExtensionsFluent;
            coreTaskExtensionsFluent.Target = this;
            coreTaskExtensionsFluent.Context = Context;
            return coreTaskExtensionsFluent;
        }

        public ITargetFluentInterface Do(Action<ITargetFluentInterface> action)
        {
            action?.Invoke(this);
            return this;
        }

        public ITargetFluentInterface Do<T>(Action<ITargetFluentInterface, T> action, T param)
        {
            action?.Invoke(this, param);
            return this;
        }

        public ITargetFluentInterface Do<T, T2>(Action<ITargetFluentInterface, T, T2> action, T param, T2 param2)
        {
            action?.Invoke(this, param, param2);
            return this;
        }

        public ITargetFluentInterface Do<T, T2, T3>(Action<ITargetFluentInterface, T, T2, T3> action, T param, T2 param2, T3 param3)
        {
            action?.Invoke(this, param, param2, param3);
            return this;
        }

        public ITargetFluentInterface Do<T, T2, T3, T4>(Action<ITargetFluentInterface, T, T2, T3, T4> action, T param, T2 param2, T3 param3, T4 param4)
        {
            action?.Invoke(this, param, param2, param3, param4);
            return this;
        }

        public ITargetFluentInterface Do<T, T2, T3, T4, T5>(Action<ITargetFluentInterface, T, T2, T3, T4, T5> action, T param, T2 param2, T3 param3, T4 param4, T5 param5)
        {
            action?.Invoke(this, param, param2, param3, param4, param5);
            return this;
        }

        public ITargetFluentInterface Do<T, T2, T3, T4, T5, T6>(Action<ITargetFluentInterface, T, T2, T3, T4, T5, T6> action, T param, T2 param2, T3 param3, T4 param4, T5 param5,
            T6 param6)
        {
            action?.Invoke(this, param, param2, param3, param4, param5, param6);
            return this;
        }

        public ITargetFluentInterface Do<T, T2, T3, T4, T5, T6, T7>(Action<ITargetFluentInterface, T, T2, T3, T4, T5, T6, T7> action, T param, T2 param2, T3 param3, T4 param4, T5 param5,
            T6 param6, T7 param7)
        {
            action?.Invoke(this, param, param2, param3, param4, param5, param6, param7);
            return this;
        }

        public ITargetFluentInterface Do<T, T2, T3, T4, T5, T6, T7, T8>(Action<ITargetFluentInterface, T, T2, T3, T4, T5, T6, T7, T8> action, T param, T2 param2, T3 param3, T4 param4, T5 param5,
            T6 param6, T7 param7, T8 param8)
        {
            action?.Invoke(this, param, param2, param3, param4, param5, param6, param7, param8);
            return this;
        }

        public ITargetFluentInterface When(Func<ITaskContextInternal, bool> condition, Action<ITargetFluentInterface> action)
        {
            var conditionMeet = condition?.Invoke(Context) ?? true;

            if (!conditionMeet)
            {
                return this;
            }

            action?.Invoke(this);
            return this;
        }

        public ITargetFluentInterface When<T>(Func<ITaskContextInternal, bool> condition, Action<ITargetFluentInterface, T> action, T param)
        {
            var conditionMeet = condition?.Invoke(Context) ?? true;

            if (!conditionMeet)
            {
                return this;
            }

            action?.Invoke(this, param);
            return this;
        }

        public ITargetFluentInterface When<T, T2>(Func<ITaskContextInternal, bool> condition, Action<ITargetFluentInterface, T, T2> action, T param, T2 param2)
        {
            var conditionMeet = condition?.Invoke(Context) ?? true;

            if (!conditionMeet)
            {
                return this;
            }

            action?.Invoke(this, param, param2);
            return this;
        }

        public ITargetFluentInterface When<T, T2, T3>(Func<ITaskContextInternal, bool> condition, Action<ITargetFluentInterface, T, T2, T3> action, T param, T2 param2, T3 param3)
        {
            var conditionMeet = condition?.Invoke(Context) ?? true;

            if (!conditionMeet)
            {
                return this;
            }

            action?.Invoke(this, param, param2, param3);
            return this;
        }

        public ITargetFluentInterface When<T, T2, T3, T4>(Func<ITaskContextInternal, bool> condition, Action<ITargetFluentInterface, T, T2, T3, T4> action, T param, T2 param2, T3 param3, T4 param4)
        {
            var conditionMeet = condition?.Invoke(Context) ?? true;

            if (!conditionMeet)
            {
                return this;
            }

            action?.Invoke(this, param, param2, param3, param4);
            return this;
        }

        public ITargetFluentInterface When<T, T2, T3, T4, T5>(Func<ITaskContextInternal, bool> condition, Action<ITargetFluentInterface, T, T2, T3, T4, T5> action, T param, T2 param2, T3 param3, T4 param4, T5 param5)
        {
            var conditionMeet = condition?.Invoke(Context) ?? true;

            if (!conditionMeet)
            {
                return this;
            }

            action?.Invoke(this, param, param2, param3, param4, param5);
            return this;
        }

        public ITargetFluentInterface When<T, T2, T3, T4, T5, T6>(Func<ITaskContextInternal, bool> condition, Action<ITargetFluentInterface, T, T2, T3, T4, T5, T6> action, T param, T2 param2, T3 param3, T4 param4, T5 param5,
            T6 param6)
        {
            var conditionMeet = condition?.Invoke(Context) ?? true;

            if (!conditionMeet)
            {
                return this;
            }

            action?.Invoke(this, param, param2, param3, param4, param5, param6);
            return this;
        }

        public ITargetFluentInterface When<T, T2, T3, T4, T5, T6, T7>(Func<ITaskContextInternal, bool> condition, Action<ITargetFluentInterface, T, T2, T3, T4, T5, T6, T7> action, T param, T2 param2, T3 param3, T4 param4, T5 param5,
            T6 param6, T7 param7)
        {
            var conditionMeet = condition?.Invoke(Context) ?? true;

            if (!conditionMeet)
            {
                return this;
            }

            action?.Invoke(this, param, param2, param3, param4, param5, param6, param7);
            return this;
        }

        public ITargetFluentInterface When<T, T2, T3, T4, T5, T6, T7, T8>(Func<ITaskContextInternal, bool> condition, Action<ITargetFluentInterface, T, T2, T3, T4, T5, T6, T7, T8> action, T param, T2 param2, T3 param3, T4 param4, T5 param5,
            T6 param6, T7 param7, T8 param8)
        {
            var conditionMeet = condition?.Invoke(Context) ?? true;

            if (!conditionMeet)
            {
                return this;
            }

            action?.Invoke(this, param, param2, param3, param4, param5, param6, param7, param8);
            return this;
        }
    }
}
