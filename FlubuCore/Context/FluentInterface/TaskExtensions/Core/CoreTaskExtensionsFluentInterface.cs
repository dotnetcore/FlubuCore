using FlubuCore.Context.FluentInterface.Interfaces;

namespace FlubuCore.Context.FluentInterface.TaskExtensions.Core
{
    public partial class CoreTaskExtensionsFluentInterface : ICoreTaskExtensionsFluentInterface
    {
        public TargetFluentInterface Target { get; set; }

        public ITaskContextInternal Context { protected get; set; }

        public ITargetFluentInterface BackToTarget()
        {
            return Target;
        }
    }
}
