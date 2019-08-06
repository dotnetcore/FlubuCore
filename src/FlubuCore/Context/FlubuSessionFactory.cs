using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace FlubuCore.Context
{
    public class FlubuSessionFactory : IFlubuSessionFactory
    {
        private readonly IServiceProvider _sp;

        public FlubuSessionFactory(IServiceProvider sp)
        {
            _sp = sp;
        }

        public IFlubuSession OpenTaskSession()
        {
            return _sp.GetRequiredService<IFlubuSession>();
        }
    }
}
