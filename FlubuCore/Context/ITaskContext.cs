using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlubuCore.Context.FluentInterface;
using FlubuCore.Context.FluentInterface.Interfaces;
using FlubuCore.Targeting;

namespace FlubuCore.Context
{
    public interface ITaskContext : IBuildPropertiesContext
    {
        TargetTree TargetTree { get; }

        ICoreTaskFluentInterface CoreTasks();

        ITaskFluentInterface Tasks();

        ITargetFluentInterface CreateTarget(string name);
    }
}
