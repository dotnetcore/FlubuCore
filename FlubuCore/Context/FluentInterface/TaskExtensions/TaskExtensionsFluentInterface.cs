using FlubuCore.Context.FluentInterface.Interfaces;

namespace FlubuCore.Context.FluentInterface.TaskExtensions
{
    public partial class TaskExtensionsFluentInterface : ITaskExtensionsFluentInterface
    {
        public ITargetFluentInterface Target { protected get; set; }

        public TaskContextInternal Context { protected get; set; }
    }
}
