using System;
using FlubuCore.Context.FluentInterface.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace FlubuCore.Context.FluentInterface
{
    public class FluentInterfaceFactory : IFluentInterfaceFactory
    {
        private readonly IServiceProvider _sp;

        public FluentInterfaceFactory(IServiceProvider sp)
        {
            _sp = sp;
        }

        public ICoreTaskFluentInterface GetCoreTaskFluentInterface()
        {
            return _sp.GetRequiredService<ICoreTaskFluentInterface>();
        }

        public ITaskFluentInterface GetTaskFluentInterface()
        {
            return _sp.GetRequiredService<ITaskFluentInterface>();
        }

        public ITargetFluentInterface GetTargetFluentInterface()
        {
            return _sp.GetRequiredService<ITargetFluentInterface>();
        }
    }
}
