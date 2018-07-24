using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace FlubuCore.Context
{
    public class TaskSessionFactory : ITaskSessionFactory
    {
        private readonly IServiceProvider _sp;

        public TaskSessionFactory(IServiceProvider sp)
        {
            _sp = sp;
        }

        public ITaskSession OpenTaskSession()
        {
            return _sp.GetRequiredService<ITaskSession>();
        }
    }
}
