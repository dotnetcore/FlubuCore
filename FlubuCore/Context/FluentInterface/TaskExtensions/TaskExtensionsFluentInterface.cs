using FlubuCore.Context.FluentInterface.Interfaces;
using FlubuCore.Targeting;

namespace FlubuCore.Context.FluentInterface.TaskExtensions
{
    public partial class TaskExtensionsFluentInterface : ITaskExtensionsFluentInterface
    {
        public ITargetFluentInterface Target { protected get; set; }

        public TaskContextInternal Context { protected get; set; }
    }
}
