using System;
using System.Collections.Generic;
using System.Text;
using FlubuCore.IO.Wrappers;
using FlubuCore.Scripting;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;

namespace FlubuCore.Commanding
{
    public class FlubuCommandParserFactory : IFlubuCommandParserFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public FlubuCommandParserFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IFlubuCommandParser GetFlubuCommandParser()
        {
            var cmdApp = new CommandLineApplication()
            {
                UnrecognizedArgumentHandling = UnrecognizedArgumentHandling.CollectAndContinue
            };
            return new FlubuCommandParser(cmdApp, _serviceProvider.GetRequiredService<IFlubuConfigurationProvider>(), _serviceProvider.GetRequiredService<IBuildScriptLocator>(), _serviceProvider.GetRequiredService<IFileWrapper>());
        }
    }
}
