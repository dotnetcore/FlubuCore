using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlubuCore.Scripting;

namespace FlubuCore.WebApi
{
    public class WebApiScriptProvider : IScriptProvider
    {
        private readonly IScriptLoader _scriptLoader;

        public WebApiScriptProvider(IScriptLoader scriptLoader)
        {
            _scriptLoader = scriptLoader;
        }

        public async Task<IBuildScript> GetBuildScriptAsync(CommandArguments commandArguments, bool forceReload = false)
        {
          return await _scriptLoader.FindAndCreateBuildScriptInstanceAsync(commandArguments);
        }
    }
}
