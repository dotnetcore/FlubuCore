using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using FlubuCore.Context.FluentInterface.Interfaces;
using FlubuCore.Targeting;
using FlubuCore.Tasks;

namespace FlubuCore.Context.FluentInterface
{
    public class TargetFluentInterface : ITargetFluentInterface
    {
        public ITarget Target { get; set; }

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

        public ITargetFluentInterface Do(Action<ITaskContextInternal> targetAction)
        {
            Target.Do(targetAction);
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

        public ITargetFluentInterface AddTask(params ITask[] tasks)
        {
            Target.AddTask(tasks);
            return this;
        }
    }
}
