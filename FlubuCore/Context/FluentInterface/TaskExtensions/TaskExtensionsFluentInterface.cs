using FlubuCore.Context.FluentInterface.Interfaces;

namespace FlubuCore.Context.FluentInterface.TaskExtensions
{
    public partial class TaskExtensionsFluentInterface : ITaskExtensionsFluentInterface
    {
        public TargetFluentInterface Target { get; set; }

        public ITaskContextInternal Context { protected get; set; }

        public ITargetFluentInterface BackToTarget()
        {
            return Target;
        }
    }
}
