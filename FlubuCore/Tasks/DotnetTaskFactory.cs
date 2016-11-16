using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace FlubuCore.Tasks
{
    public class DotnetTaskFactory : ITaskFactory
    {
        private readonly IServiceProvider _provider;

        public DotnetTaskFactory(IServiceProvider provider)
        {
            _provider = provider;
        }

        public T Create<T>()
            where T : ITask
        {
            return _provider.GetRequiredService<T>();
        }

        public T Create<T>(params object[] constructorArgs)
            where T : ITask
        {
            return ActivatorUtilities.CreateInstance<T>(_provider, constructorArgs);
        }

        public IReadOnlyList<ITask> ListAll()
        {
            return _provider.GetServices<ITask>().ToList();
        }
    }
}
