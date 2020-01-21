using System;
using System.Collections.Generic;
using System.Text;
using FlubuCore.Context;
using Microsoft.Extensions.DependencyInjection;

namespace FlubuCore.Scripting
{
    public class ScriptFactory : IScriptFactory
    {
        private readonly IServiceProvider _serviceRrovider;

        public ScriptFactory(IServiceProvider serviceRrovider)
        {
            _serviceRrovider = serviceRrovider;
        }

        public IScriptProperties CreateScriptProperties()
        {
            return _serviceRrovider.GetRequiredService<IScriptProperties>();
        }
    }
}
