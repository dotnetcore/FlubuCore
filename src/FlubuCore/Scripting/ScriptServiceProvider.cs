using System;
using System.Collections.Generic;
using System.Text;
using FlubuCore.Context;
using FlubuCore.Targeting;
using Microsoft.Extensions.DependencyInjection;

namespace FlubuCore.Scripting
{
    public class ScriptServiceProvider : IScriptServiceProvider
    {
        private readonly IServiceProvider _serviceProvider;

        public ScriptServiceProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IScriptProperties GetScriptProperties()
        {
            return _serviceProvider.GetRequiredService<IScriptProperties>();
        }

        public ITargetCreator GetTargetCreator()
        {
            return _serviceProvider.GetRequiredService<ITargetCreator>();
        }

        public FlubuConfiguration GetFlubuConfiguration()
        {
            return _serviceProvider.GetService<FlubuConfiguration>();
        }
    }
}
