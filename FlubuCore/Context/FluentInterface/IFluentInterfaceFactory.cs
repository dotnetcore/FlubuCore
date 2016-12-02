using FlubuCore.Context.FluentInterface.Interfaces;

namespace FlubuCore.Context.FluentInterface
{
    public interface IFluentInterfaceFactory
    {
        ITargetFluentInterface GetTargetFluentInterface();

        ITaskFluentInterface GetTaskFluentInterface();

        ICoreTaskFluentInterface GetCoreTaskFluentInterface();
    }
}