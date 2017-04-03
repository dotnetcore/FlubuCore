using System;
using FlubuCore.Context.FluentInterface.Interfaces;
using FlubuCore.Tasks.NetCore;

namespace FlubuCore.Context.FluentInterface.TaskExtensions.Core
{
    public partial class CoreTaskExtensionsFluentInterface : ICoreTaskExtensionsFluentInterface
    {
        public TargetFluentInterface Target { get; set; }

        public ITaskContextInternal Context { protected get; set; }

        /// <summary>
        /// Moves back to target fluent interface. 
        /// </summary>
        /// <returns></returns>
        public ITargetFluentInterface BackToTarget()
        {
            return Target;
        }
    }
}
